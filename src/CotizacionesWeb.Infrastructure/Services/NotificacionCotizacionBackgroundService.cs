using CotizacionesWeb.Application.Notificaciones;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CotizacionesWeb.Application.Configuracion;

namespace CotizacionesWeb.Infrastructure.Services;

/// <summary>
/// BackgroundService para procesar notificaciones de cotizaciones de forma periódica
/// </summary>
public class NotificacionCotizacionBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificacionCotizacionBackgroundService> _logger;
    
    // Constantes para parámetros del sistema
    private const string NOTIFICACIONES_ENABLED = "NOTIFICACIONES_COTIZACIONES_ENABLED";
    private const string FRECUENCIA_MINUTOS = "NOTIFICACIONES_FRECUENCIA_MINUTOS";
    
    // Valores por defecto
    private const int FRECUENCIA_DEFECTO_MINUTOS = 5;
    
    public NotificacionCotizacionBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<NotificacionCotizacionBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NotificacionCotizacionBackgroundService iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcesarCicloNotificacionesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Cancelación normal, no es error
                _logger.LogInformation("NotificacionCotizacionBackgroundService detenido por cancelación");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en el ciclo de procesamiento de notificaciones");
            }
        }
        
        _logger.LogInformation("NotificacionCotizacionBackgroundService finalizado");
    }

    /// <summary>
    /// Procesa un ciclo completo de notificaciones
    /// </summary>
    private async Task ProcesarCicloNotificacionesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var parametroService = scope.ServiceProvider.GetRequiredService<IParametroSistemaService>();
        
        try
        {
            // Leer configuración del sistema
            var (notificacionesHabilitadas, frecuenciaMinutos) = await ObtenerConfiguracionAsync(parametroService);
            
            if (!notificacionesHabilitadas)
            {
                _logger.LogDebug("Notificaciones de cotizaciones deshabilitadas. Esperando {Minutos} minutos para próxima verificación", frecuenciaMinutos);
            }
            else
            {
                _logger.LogInformation("Iniciando procesamiento de notificaciones de cotizaciones");
                
                // Aquí se procesarán las notificaciones pendientes
                await ProcesarNotificacionesPendientesAsync(scope.ServiceProvider, cancellationToken);
                
                _logger.LogInformation("Procesamiento de notificaciones completado. Esperando {Minutos} minutos para próximo ciclo", frecuenciaMinutos);
            }
            
            // Esperar hasta el próximo ciclo
            await EsperarProximoCicloAsync(frecuenciaMinutos, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw; // Re-lanzar para manejo en nivel superior
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el procesamiento del ciclo de notificaciones");
            
            // En caso de error, esperar tiempo mínimo antes de reintentar
            var tiempoEspera = Math.Min(FRECUENCIA_DEFECTO_MINUTOS, 2);
            _logger.LogInformation("Esperando {Minutos} minutos antes de reintentar debido a error", tiempoEspera);
            await EsperarProximoCicloAsync(tiempoEspera, cancellationToken);
        }
    }

    /// <summary>
    /// Obtiene la configuración actual del sistema para notificaciones
    /// </summary>
    private async Task<(bool habilitadas, int frecuenciaMinutos)> ObtenerConfiguracionAsync(IParametroSistemaService parametroService)
    {
        try
        {
            // Leer si las notificaciones están habilitadas
            var habilitadasTexto = await parametroService.ObtenerValorParametroAsync(NOTIFICACIONES_ENABLED);
            var notificacionesHabilitadas = string.Equals(habilitadasTexto, "S", StringComparison.OrdinalIgnoreCase);

            // Leer frecuencia de procesamiento
            var frecuencia = await parametroService.ObtenerValorParametroAsync<int?>(FRECUENCIA_MINUTOS);
            var frecuenciaMinutos = frecuencia ?? FRECUENCIA_DEFECTO_MINUTOS;

            // Validar rango razonable para frecuencia
            if (frecuenciaMinutos < 1)
            {
                _logger.LogWarning("Frecuencia de {Minutos} minutos es demasiado baja, usando frecuencia mínima de 1 minuto", frecuenciaMinutos);
                frecuenciaMinutos = 1;
            }
            else if (frecuenciaMinutos > 1440) // Más de 24 horas
            {
                _logger.LogWarning("Frecuencia de {Minutos} minutos es demasiado alta, usando frecuencia máxima de 1440 minutos (24 horas)", frecuenciaMinutos);
                frecuenciaMinutos = 1440;
            }

            _logger.LogDebug("Configuración de notificaciones: Habilitadas={Habilitadas}, Frecuencia={Frecuencia} minutos", 
                notificacionesHabilitadas, frecuenciaMinutos);

            return (notificacionesHabilitadas, frecuenciaMinutos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener configuración de notificaciones, usando valores por defecto");
            return (false, FRECUENCIA_DEFECTO_MINUTOS);
        }
    }

    /// <summary>
    /// Procesa las notificaciones pendientes de envío
    /// </summary>
    private async Task ProcesarNotificacionesPendientesAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        try
        {
            var notificacionService = serviceProvider.GetRequiredService<INotificacionCotizacionService>();
            
            _logger.LogDebug("Verificando notificaciones pendientes...");
            
            // Procesar notificaciones usando el servicio especializado
            var notificacionesProcesadas = await notificacionService.ProcesarNotificacionesPendientesAsync(cancellationToken);
            
            if (notificacionesProcesadas > 0)
            {
                _logger.LogInformation("Se procesaron {Cantidad} notificaciones exitosamente", notificacionesProcesadas);
            }
            else
            {
                _logger.LogDebug("No se encontraron notificaciones pendientes o todas fallaron");
            }
        }
        catch (OperationCanceledException)
        {
            throw; // Re-lanzar para manejo en nivel superior
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar notificaciones pendientes");
            throw; // Re-lanzar para manejo en ProcesarCicloNotificacionesAsync
        }
    }

    /// <summary>
    /// Espera hasta el próximo ciclo de procesamiento
    /// </summary>
    private async Task EsperarProximoCicloAsync(int minutos, CancellationToken cancellationToken)
    {
        var tiempoEspera = TimeSpan.FromMinutes(minutos);
        var proximaEjecucion = DateTime.Now.Add(tiempoEspera);
        
        _logger.LogDebug("Próxima verificación de notificaciones programada para: {ProximaEjecucion:yyyy-MM-dd HH:mm:ss}", proximaEjecucion);
        
        try
        {
            await Task.Delay(tiempoEspera, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Espera cancelada, deteniendo servicio de notificaciones");
            throw; // Re-lanzar para detener el servicio
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deteniendo NotificacionCotizacionBackgroundService...");
        await base.StopAsync(cancellationToken);
        _logger.LogInformation("NotificacionCotizacionBackgroundService detenido");
    }
}