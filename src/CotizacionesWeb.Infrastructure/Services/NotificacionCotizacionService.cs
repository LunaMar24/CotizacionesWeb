using CotizacionesWeb.Application.Common.Email;
using CotizacionesWeb.Application.Notificaciones;
using CotizacionesWeb.Domain.Entities;
using CotizacionesWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de notificaciones de cotizaciones
/// </summary>
public class NotificacionCotizacionService : INotificacionCotizacionService
{
  private readonly DbContextCotizaciones _context;
  private readonly IEmailService _emailService;
  private readonly ILogger<NotificacionCotizacionService> _logger;

  // Constantes para tipos de notificación
  private const string TIPO_PENDIENTE_APROBACION = "PendienteAprobacion";
  private const string TIPO_SEGUIMIENTO_ENVIADA = "SeguimientoEnviada";
  private const string TIPO_ERROR_INTEGRACIONERP = "ErrorIntegracionERP";

  // Estados de notificación
  private const string ESTADO_PENDIENTE = "Pendiente";
  private const string ESTADO_ENVIADA = "Enviada";
  private const string ESTADO_ERROR = "Error";

  // Límite máximo de intentos
  private const int MAX_INTENTOS = 3; //TODO: Pasar parámetro a la base de datos

  public NotificacionCotizacionService(
      DbContextCotizaciones context,
      IEmailService emailService,
      ILogger<NotificacionCotizacionService> logger)
  {
    _context = context;
    _emailService = emailService;
    _logger = logger;
  }

  /// <summary>
  /// Procesa todas las notificaciones pendientes de envío
  /// </summary>
  public async Task<int> ProcesarNotificacionesPendientesAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      _logger.LogInformation("Iniciando procesamiento de notificaciones pendientes");

      // Verificar que el servicio de email esté disponible
      if (!await _emailService.EstaDisponibleAsync())
      {
        _logger.LogWarning("Servicio de email no disponible, saltando procesamiento de notificaciones");
        return 0;
      }

      // Buscar notificaciones pendientes
      var notificacionesPendientes = await ObtenerNotificacionesPendientesAsync(cancellationToken);

      if (!notificacionesPendientes.Any())
      {
        _logger.LogDebug("No se encontraron notificaciones pendientes para procesar");
        return 0;
      }

      _logger.LogInformation("Encontradas {Cantidad} notificaciones pendientes para procesar",
          notificacionesPendientes.Count);

      var resumen = new ResumenProcesamiento(notificacionesPendientes.Count, 0, 0);

      // Procesar cada notificación
      foreach (var notificacion in notificacionesPendientes)
      {
        if (cancellationToken.IsCancellationRequested)
        {
          _logger.LogInformation("Procesamiento cancelado por token de cancelación");
          break;
        }

        var resultado = await ProcesarNotificacionIndividualAsync(notificacion, cancellationToken);

        if (resultado.Exitoso)
        {
          resumen = resumen with { NotificacionesExitosas = resumen.NotificacionesExitosas + 1 };
        }
        else
        {
          resumen = resumen with { NotificacionesConError = resumen.NotificacionesConError + 1 };
        }

        // Pequeña pausa entre notificaciones para evitar saturar el servidor de email
        await Task.Delay(1000, cancellationToken);
      }

      _logger.LogInformation("Procesamiento completado. Enviadas: {Exitosas}, Errores: {Errores}, Total: {Total}",
          resumen.NotificacionesExitosas, resumen.NotificacionesConError, resumen.NotificacionesEncontradas);

      return resumen.NotificacionesExitosas;
    }
    catch (OperationCanceledException)
    {
      _logger.LogInformation("Procesamiento de notificaciones cancelado");
      return 0;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error inesperado durante el procesamiento de notificaciones");
      throw;
    }
  }

  /// <summary>
  /// Obtiene las notificaciones pendientes de envío
  /// </summary>
  private async Task<List<NotificacionCotizacionDto>> ObtenerNotificacionesPendientesAsync(CancellationToken cancellationToken)
  {
    var fechaActual = DateTime.Now;

    var query = from notif in _context.NotificacionesCotizacion
                join cotizacion in _context.Cotizaciones
                    on notif.CotizacionId equals cotizacion.CotizacionId
                join version in _context.CotizacionesVersiones
                    on notif.VersionId equals version.VersionId
                where notif.Estado == ESTADO_PENDIENTE
                      && notif.FechaProgramada <= fechaActual
                      && notif.Intentos < MAX_INTENTOS
                orderby notif.FechaProgramada
                select new NotificacionCotizacionDto(
                    notif.NotificacionId,
                    notif.CotizacionId,
                    notif.VersionId,
                    notif.TipoNotificacion,
                    notif.EmailDestino,
                    notif.FechaProgramada,
                    notif.Intentos,
                    notif.Asunto ?? "",
                    notif.Cuerpo ?? "",
                    cotizacion.EstadoActual.ToString(),
                    cotizacion.MontoCotizacion,
                    cotizacion.Moneda,
                    version.NumeroVersion,
                    version.NombreInteresado,
                    version.EmailInteresado,
                    version.EmpresaInteresado
                );

    return await query.ToListAsync(cancellationToken);
  }

  /// <summary>
  /// Procesa una notificación individual
  /// </summary>
  private async Task<ResultadoProcesamiento> ProcesarNotificacionIndividualAsync(
      NotificacionCotizacionDto notificacion,
      CancellationToken cancellationToken)
  {
    try
    {
      _logger.LogDebug("Procesando notificación {NotificacionId} tipo {Tipo} para cotización {CotizacionId}",
          notificacion.NotificacionId, notificacion.TipoNotificacion, notificacion.CotizacionId);

      // Generar contenido del email
      var (asunto, cuerpo) = GenerarContenidoEmail(notificacion);

      // Intentar enviar el email
      var emailEnviado = await _emailService.EnviarEmailAsync(
          notificacion.EmailDestino,
          asunto,
          cuerpo,
          esHtml: true);

      var fechaEnviada = DateTime.Now;

      if (emailEnviado)
      {
        // Marcar como enviada
        await ActualizarEstadoNotificacionAsync(
            notificacion.NotificacionId,
            ESTADO_ENVIADA,
            fechaEnviada,
            null,
            notificacion.Intentos);

        _logger.LogInformation("Notificación {NotificacionId} enviada exitosamente a {Email}",
            notificacion.NotificacionId, notificacion.EmailDestino);

        return new ResultadoProcesamiento(true, null, fechaEnviada);
      }
      else
      {
        // Incrementar intentos y marcar error o mantener pendiente
        var nuevosIntentos = notificacion.Intentos + 1;
        var nuevoEstado = nuevosIntentos >= MAX_INTENTOS ? ESTADO_ERROR : ESTADO_PENDIENTE;
        var mensajeError = $"Error al enviar email (intento {nuevosIntentos}/{MAX_INTENTOS})";

        await ActualizarEstadoNotificacionAsync(
            notificacion.NotificacionId,
            nuevoEstado,
            null,
            mensajeError,
            nuevosIntentos);

        _logger.LogWarning("Error al enviar notificación {NotificacionId} a {Email}. Intento {Intento}/{MaxIntentos}",
            notificacion.NotificacionId, notificacion.EmailDestino, nuevosIntentos, MAX_INTENTOS);

        return new ResultadoProcesamiento(false, mensajeError);
      }
    }
    catch (Exception ex)
    {
      var mensajeError = $"Excepción durante envío: {ex.Message}";
      var nuevosIntentos = notificacion.Intentos + 1;
      var nuevoEstado = nuevosIntentos >= MAX_INTENTOS ? ESTADO_ERROR : ESTADO_PENDIENTE;

      await ActualizarEstadoNotificacionAsync(
          notificacion.NotificacionId,
          nuevoEstado,
          null,
          mensajeError,
          nuevosIntentos);

      _logger.LogError(ex, "Excepción al procesar notificación {NotificacionId}",
          notificacion.NotificacionId);

      return new ResultadoProcesamiento(false, mensajeError);
    }
  }

  /// <summary>
  /// Genera el contenido (asunto y cuerpo) del email según el tipo de notificación
  /// </summary>
  private (string asunto, string cuerpo) GenerarContenidoEmail(NotificacionCotizacionDto notificacion)
  {
    return notificacion.TipoNotificacion switch
    {
      TIPO_PENDIENTE_APROBACION => GenerarContenidoPendienteAprobacion(notificacion),
      TIPO_SEGUIMIENTO_ENVIADA => GenerarContenidoSeguimientoEnviada(notificacion),
      TIPO_ERROR_INTEGRACIONERP => string.IsNullOrEmpty(notificacion.Asunto) ? GenerarContenidoGenerico(notificacion) : (notificacion.Asunto, notificacion.CuerpoMensaje),
      _ => GenerarContenidoGenerico(notificacion)
    };
  }

  /// <summary>
  /// Genera contenido para notificación de pendiente aprobación
  /// </summary>
  private (string asunto, string cuerpo) GenerarContenidoPendienteAprobacion(NotificacionCotizacionDto notificacion)
  {
    var simboloMoneda = ObtenerSimboloMoneda(notificacion.Moneda);
    var montoFormateado = $"{simboloMoneda}{notificacion.MontoCotizacion:N2}";

    var asunto = $"Cotización {notificacion.CotizacionId} v{notificacion.NumeroVersion:0.0} - Pendiente de Aprobación";

    var cuerpo = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <h2 style='color: #333; border-bottom: 2px solid #007bff; padding-bottom: 10px;'>
                    Cotización Pendiente de Aprobación
                </h2>
                
                <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                    <h3 style='color: #007bff; margin-top: 0;'>Información de la Cotización</h3>
                    <p><strong>ID:</strong> {notificacion.CotizacionId}</p>
                    <p><strong>Versión:</strong> {notificacion.NumeroVersion:0.0}</p>
                    <p><strong>Cliente:</strong> {notificacion.NombreInteresado}</p>
                    <p><strong>Empresa:</strong> {notificacion.EmpresaInteresado}</p>
                    <p><strong>Monto:</strong> {montoFormateado}</p>
                    <p><strong>Estado:</strong> <span style='color: #ffc107; font-weight: bold;'>Pendiente de Aprobación</span></p>
                </div>
                
                <p>Esta cotización ha sido enviada para aprobación y requiere su revisión.</p>
                
                <p style='margin-top: 30px; color: #666; font-size: 0.9em;'>
                    Este es un mensaje automático del sistema de cotizaciones.
                </p>
            </body>
            </html>";

    return (asunto, cuerpo);
  }

  /// <summary>
  /// Genera contenido para notificación de seguimiento de envío
  /// </summary>
  private (string asunto, string cuerpo) GenerarContenidoSeguimientoEnviada(NotificacionCotizacionDto notificacion)
  {
    var simboloMoneda = ObtenerSimboloMoneda(notificacion.Moneda);
    var montoFormateado = $"{simboloMoneda}{notificacion.MontoCotizacion:N2}";

    var asunto = $"Seguimiento: Cotización {notificacion.CotizacionId} v{notificacion.NumeroVersion:0.0} - Enviada al Cliente";

    var cuerpo = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <h2 style='color: #333; border-bottom: 2px solid #28a745; padding-bottom: 10px;'>
                    Seguimiento de Cotización Enviada
                </h2>
                
                <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                    <h3 style='color: #28a745; margin-top: 0;'>Información de la Cotización</h3>
                    <p><strong>ID:</strong> {notificacion.CotizacionId}</p>
                    <p><strong>Versión:</strong> {notificacion.NumeroVersion:0.0}</p>
                    <p><strong>Cliente:</strong> {notificacion.NombreInteresado}</p>
                    <p><strong>Empresa:</strong> {notificacion.EmpresaInteresado}</p>
                    <p><strong>Email Cliente:</strong> {notificacion.EmailInteresado}</p>
                    <p><strong>Monto:</strong> {montoFormateado}</p>
                    <p><strong>Estado:</strong> <span style='color: #17a2b8; font-weight: bold;'>Enviada al Cliente</span></p>
                </div>
                
                <p>Esta cotización ha sido enviada al cliente y está pendiente de su respuesta.</p>
                <p>Se recomienda hacer seguimiento directo con el cliente para conocer su decisión.</p>
                
                <p style='margin-top: 30px; color: #666; font-size: 0.9em;'>
                    Este es un mensaje automático del sistema de cotizaciones.
                </p>
            </body>
            </html>";

    return (asunto, cuerpo);
  }

  /// <summary>
  /// Genera contenido genérico para tipos no reconocidos
  /// </summary>
  private (string asunto, string cuerpo) GenerarContenidoGenerico(NotificacionCotizacionDto notificacion)
  {
    var asunto = $"Notificación: Cotización {notificacion.CotizacionId} v{notificacion.NumeroVersion:0.0}";

    var cuerpo = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <h2>Notificación de Cotización</h2>
                <p><strong>ID:</strong> {notificacion.CotizacionId}</p>
                <p><strong>Versión:</strong> {notificacion.NumeroVersion:0.0}</p>
                <p><strong>Cliente:</strong> {notificacion.NombreInteresado}</p>
                <p><strong>Tipo de Notificación:</strong> {notificacion.TipoNotificacion}</p>
            </body>
            </html>";

    return (asunto, cuerpo);
  }

  /// <summary>
  /// Actualiza el estado de una notificación
  /// </summary>
  private async Task ActualizarEstadoNotificacionAsync(
      int notificacionId,
      string nuevoEstado,
      DateTime? fechaEnviada,
      string? mensajeError,
      int intentos)
  {
    var notificacion = await _context.NotificacionesCotizacion
        .FirstOrDefaultAsync(n => n.NotificacionId == notificacionId);

    if (notificacion != null)
    {
      notificacion.Estado = nuevoEstado;
      notificacion.FechaEnviada = fechaEnviada;
      notificacion.MensajeError = mensajeError;
      notificacion.Intentos = intentos;

      await _context.SaveChangesAsync();
    }
  }

  /// <summary>
  /// Obtiene el símbolo de moneda
  /// </summary>
  private string ObtenerSimboloMoneda(string moneda) => moneda?.ToUpper() switch
  {
    "CRC" => "¢",
    "USD" => "$",
    "EUR" => "€",
    "GBP" => "£",
    "JPY" => "¥",
    _ => "$"
  };

  /// <summary>
  /// Crea una notificación para pendiente aprobación
  /// </summary>
  public async Task<bool> CrearNotificacionPendienteAprobacionAsync(
      string cotizacionId,
      int versionId,
      string emailDestino,
      DateTime? fechaProgramada = null)
  {
    return await CrearNotificacionAsync(
        TIPO_PENDIENTE_APROBACION,
        cotizacionId,
        versionId,
        emailDestino,
        fechaProgramada ?? DateTime.Now);
  }

  /// <summary>
  /// Crea una notificación de seguimiento enviada
  /// </summary>
  public async Task<bool> CrearNotificacionSeguimientoEnviadaAsync(
      string cotizacionId,
      int versionId,
      string emailDestino,
      DateTime? fechaProgramada = null)
  {
    return await CrearNotificacionAsync(
        TIPO_SEGUIMIENTO_ENVIADA,
        cotizacionId,
        versionId,
        emailDestino,
        fechaProgramada ?? DateTime.Now.AddHours(24)); // Por defecto 24 horas después
  }

  /// <summary>
  /// Método genérico para crear notificaciones
  /// </summary>
  private async Task<bool> CrearNotificacionAsync(
      string tipoNotificacion,
      string cotizacionId,
      int versionId,
      string emailDestino,
      DateTime fechaProgramada)
  {
    try
    {
      var notificacion = new NotificacionCotizacion
      {
        CotizacionId = cotizacionId,
        VersionId = versionId,
        TipoNotificacion = tipoNotificacion,
        EmailDestino = emailDestino,
        FechaProgramada = fechaProgramada,
        Estado = ESTADO_PENDIENTE,
        Intentos = 0
      };

      _context.NotificacionesCotizacion.Add(notificacion);
      await _context.SaveChangesAsync();

      _logger.LogInformation("Notificación {Tipo} creada para cotización {CotizacionId} v{VersionId}, destino: {Email}",
          tipoNotificacion, cotizacionId, versionId, emailDestino);

      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al crear notificación {Tipo} para cotización {CotizacionId}",
          tipoNotificacion, cotizacionId);
      return false;
    }
  }
}