using CotizacionesWeb.Application.Configuracion;
using CotizacionesWeb.Application.Integrations.Erp;
using CotizacionesWeb.Application.Notificaciones;
using CotizacionesWeb.Domain.Entities;
using CotizacionesWeb.Domain.Enums;
using CotizacionesWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Infrastructure.Services;

/// <summary>
/// Servicio en segundo plano para procesar integraciones pendientes con ERP
/// Procesa lotes en estados Pendiente y Error hasta un máximo de intentos
/// </summary>
public class IntegracionErpBackgroundService : BackgroundService
{
  private readonly IServiceProvider _serviceProvider;
  private readonly ILogger<IntegracionErpBackgroundService> _logger;

  // Configuración por defecto en caso de que falten parámetros
  private const int FRECUENCIA_DEFECTO_MINUTOS = 10;
  private const int MAX_INTENTOS_DEFECTO = 3;

  public IntegracionErpBackgroundService(
      IServiceProvider serviceProvider,
      ILogger<IntegracionErpBackgroundService> logger)
  {
    _serviceProvider = serviceProvider;
    _logger = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation("IntegracionErpBackgroundService iniciado");

    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        await ProcesarLotesIntegracionAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error general en IntegracionErpBackgroundService");
      }

      // Obtener frecuencia desde parámetros del sistema
      var frecuenciaMinutos = await ObtenerFrecuenciaAsync();
      var delay = TimeSpan.FromMinutes(frecuenciaMinutos);
      
      _logger.LogDebug("Esperando {Minutos} minutos antes del próximo ciclo", frecuenciaMinutos);
      await Task.Delay(delay, stoppingToken);
    }

    _logger.LogInformation("IntegracionErpBackgroundService detenido");
  }

  private async Task<int> ObtenerFrecuenciaAsync()
  {
    try
    {
      using var scope = _serviceProvider.CreateScope();
      var parametroService = scope.ServiceProvider.GetRequiredService<IParametroSistemaService>();
      
      var frecuencia = await parametroService.ObtenerValorParametroAsync<int?>("ERP_INTEGRACION_FRECUENCIA_MINUTOS");
      return frecuencia ?? FRECUENCIA_DEFECTO_MINUTOS;
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Error al obtener frecuencia de integración, usando valor por defecto: {FrecuenciaDefecto}", 
          FRECUENCIA_DEFECTO_MINUTOS);
      return FRECUENCIA_DEFECTO_MINUTOS;
    }
  }

  private async Task<int> ObtenerMaxIntentosAsync()
  {
    try
    {
      using var scope = _serviceProvider.CreateScope();
      var parametroService = scope.ServiceProvider.GetRequiredService<IParametroSistemaService>();
      
      var maxIntentos = await parametroService.ObtenerValorParametroAsync<int?>("ERP_INTEGRACION_MAX_INTENTOS");
      return maxIntentos ?? MAX_INTENTOS_DEFECTO;
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Error al obtener máximo de intentos, usando valor por defecto: {MaxIntentosDefecto}", 
          MAX_INTENTOS_DEFECTO);
      return MAX_INTENTOS_DEFECTO;
    }
  }

  private async Task ProcesarLotesIntegracionAsync()
  {
    using var scope = _serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DbContextCotizaciones>();
    var erpPedidoService = scope.ServiceProvider.GetRequiredService<IErpPedidoService>();

    var maxIntentos = await ObtenerMaxIntentosAsync();

    // Buscar registros pendientes o con error que no hayan superado el máximo de intentos
    var lotesPendientes = await context.IntegracionesPedidoErp
        .Where(i => (i.Estado == "Pendiente" || i.Estado == "Error") && 
                   i.Intentos < maxIntentos)
        .OrderBy(i => i.FechaCreacion) // FIFO
        .Take(10) // Procesar máximo 10 lotes por ciclo
        .ToListAsync();

    if (!lotesPendientes.Any())
    {
      _logger.LogDebug("No hay lotes pendientes para procesar");
      return;
    }

    _logger.LogInformation("Procesando {CantidadLotes} lotes de integración ERP", lotesPendientes.Count);

    foreach (var lote in lotesPendientes)
    {
      await ProcesarLoteAsync(lote, erpPedidoService, context, maxIntentos);
    }
  }

  private async Task ProcesarLoteAsync(
      IntegracionPedidoErp lote, 
      IErpPedidoService erpPedidoService, 
      DbContextCotizaciones context,
      int maxIntentos)
  {
    _logger.LogInformation("Procesando lote {LoteId} para cotización {CotizacionId} (Intento {Intento})", 
        lote.LoteId, lote.CotizacionId, lote.Intentos + 1);

    // PASO 1: Llamar al servicio ERP FUERA de la transacción (maneja su propia transaccionalidad)
    GenerarPedidoResult resultado;
    try
    {
      resultado = await erpPedidoService.GenerarPedidoAsync(lote.CotizacionId, lote.VersionId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al llamar GenerarPedidoAsync para lote {LoteId}", lote.LoteId);
      resultado = new GenerarPedidoResult(false, Message: $"Error del sistema: {ex.Message}");
    }

    // PASO 2: Usar transacción SOLO para actualizar estados y registro en CotizacionesWeb
    using var transaction = await context.Database.BeginTransactionAsync();

    try
    {
      if (resultado.Success)
      {
        // ? ÉXITO: Actualizar registro como procesado
        lote.Estado = "Procesado";
        lote.PedidoErp = resultado.PedidoErp;
        lote.FechaProcesado = DateTime.Now;
        lote.MensajeError = null;
        lote.Intentos++; // Incrementar intentos

        // Actualizar cotización con resultado exitoso
        await ActualizarCotizacionExitosaAsync(lote, context);

        _logger.LogInformation("? Lote {LoteId} procesado exitosamente. Pedido: {PedidoErp}", 
            lote.LoteId, resultado.PedidoErp);
      }
      else
      {
        // ? ERROR: Incrementar intentos y evaluar si supera el máximo
        lote.Intentos++;
        lote.MensajeError = resultado.Message;

        if (lote.Intentos >= maxIntentos)
        {
          // Máximo de intentos alcanzado: marcar como NoSincronizado y generar notificación
          await MarcarComoNoSincronizadoAsync(lote, context);
          _logger.LogWarning("? Lote {LoteId} alcanzó máximo de intentos ({MaxIntentos}). Marcado como NoSincronizado", 
              lote.LoteId, maxIntentos);
        }
        else
        {
          // Aún puede reintentarse: mantener estado Error
          lote.Estado = "Error";
          _logger.LogWarning("?? Lote {LoteId} falló (intento {Intento}/{MaxIntentos}). Error: {Error}", 
              lote.LoteId, lote.Intentos, maxIntentos, resultado.Message);
        }
      }

      await context.SaveChangesAsync();
      await transaction.CommitAsync();
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      _logger.LogError(ex, "Error en transacción de actualización para lote {LoteId}", lote.LoteId);
      
      // Incrementar intentos por error de sistema en transacción separada
      try
      {
        using var fallbackTransaction = await context.Database.BeginTransactionAsync();
        
        lote.Intentos++;
        lote.MensajeError = $"Error del sistema en actualización: {ex.Message}";
        
        if (lote.Intentos >= maxIntentos)
        {
          await MarcarComoNoSincronizadoAsync(lote, context);
        }
        else
        {
          lote.Estado = "Error";
        }
        
        await context.SaveChangesAsync();
        await fallbackTransaction.CommitAsync();
      }
      catch (Exception saveEx)
      {
        _logger.LogError(saveEx, "Error crítico al guardar estado de error para lote {LoteId}", lote.LoteId);
      }
    }
  }

  private async Task ActualizarCotizacionExitosaAsync(IntegracionPedidoErp lote, DbContextCotizaciones context)
  {
    var cotizacion = await context.Cotizaciones
        .FirstOrDefaultAsync(c => c.CotizacionId == lote.CotizacionId);

    if (cotizacion != null)
    {
      // Actualizar cotización
      cotizacion.FechaEnvioERP = lote.FechaProcesado;
      cotizacion.EstadoActual = 'X'; // Archivada

      // Obtener usuario del historial para el archivo
      var usuarioArchivador = await context.HistorialesCotizacion
          .Where(h => h.VersionId == lote.VersionId && h.TipoEvento == "EnviadaERP")
          .OrderByDescending(h => h.FechaEvento)
          .Select(h => h.UsuarioEvento)
          .FirstOrDefaultAsync();

      // Crear archivo de cotización como Concretada
      var archivo = new ArchivoCotizacion
      {
        CotizacionId = lote.CotizacionId,
        VersionArchivada = lote.VersionId,
        FechaArchivado = lote.FechaProcesado ?? DateTime.Now,
        UsuarioArchiva = usuarioArchivador,
        TipoArchivo = (char)TipoArchivo.Concretada, // 'T'
        MotivoArchivado = "Cotización aceptada por el cliente",
        Comentario = $"Cotización aceptada por el cliente. Pedido generado {lote.PedidoErp} en el ERP"
      };

      context.ArchivosCotizacion.Add(archivo);

      // Registrar en historial
      var historial = new HistorialCotizacion
      {
        VersionId = lote.VersionId,
        TipoEvento = "EnviadaERP",
        FechaEvento = DateTime.Now,
        UsuarioEvento = usuarioArchivador ?? 0,
        Comentario = $"Cotización procesada exitosamente en ERP. Pedido generado: {lote.PedidoErp}"
      };

      context.HistorialesCotizacion.Add(historial);
    }
  }

  private async Task MarcarComoNoSincronizadoAsync(IntegracionPedidoErp lote, DbContextCotizaciones context)
  {
    // Marcar integración como NoSincronizado
    lote.Estado = "NoSincronizado";

    // Actualizar cotización: revertir EnviadoERP
    var cotizacion = await context.Cotizaciones
        .FirstOrDefaultAsync(c => c.CotizacionId == lote.CotizacionId);

    if (cotizacion != null)
    {
      cotizacion.EnviadoERP = 'N';
      cotizacion.FechaEnvioERP = null;
    }

    // Obtener usuario del historial de envío
    var usuarioEnvio = await context.HistorialesCotizacion
        .Where(h => h.VersionId == lote.VersionId && h.TipoEvento == "EnviadaERP")
        .OrderByDescending(h => h.FechaEvento)
        .Select(h => h.UsuarioEvento)
        .FirstOrDefaultAsync();

    // Obtener email del usuario para notificación
    string? emailDestino = null;
    if (usuarioEnvio.HasValue)
    {
      emailDestino = await context.Usuarios
          .Where(u => u.UsuarioId == usuarioEnvio.Value)
          .Select(u => u.Email)
          .FirstOrDefaultAsync();
    }

    // Generar notificación de error
    if (!string.IsNullOrWhiteSpace(emailDestino))
    {
      var cuerpoMensaje = $@"Se ha producido un error en la integración de la cotización con el ERP.

Detalles del error:
- Cotización ID: {lote.CotizacionId}
- Versión ID: {lote.VersionId} (versión interna del sistema)
- Error: {lote.MensajeError}
- Intentos realizados: {lote.Intentos}

La cotización ha sido marcada como no enviada al ERP y requiere revisión manual.";

      // Crear notificación directamente en la base de datos
      var notificacion = new NotificacionCotizacion
      {
        CotizacionId = lote.CotizacionId,
        VersionId = lote.VersionId,
        TipoNotificacion = "ErrorIntegracionERP",
        EmailDestino = emailDestino,
        Asunto = "Error en Integración de la Cotización con el ERP",
        Cuerpo = cuerpoMensaje,
        FechaProgramada = DateTime.Now, // Enviar inmediatamente
        Estado = "Pendiente",
        Intentos = 0
      };

      context.NotificacionesCotizacion.Add(notificacion);
      
      _logger.LogInformation("?? Notificación de error de integración creada para {Email} (cotización {CotizacionId})", 
          emailDestino, lote.CotizacionId);
    }

    // Registrar en historial el fallo
    var historialError = new HistorialCotizacion
    {
      VersionId = lote.VersionId,
      TipoEvento = "NoEnviadaERP",
      FechaEvento = DateTime.Now,
      UsuarioEvento = usuarioEnvio ?? 0,
      Comentario = "Cotización no enviada al ERP por error de integración"
    };

    context.HistorialesCotizacion.Add(historialError);
  }
}