namespace CotizacionesWeb.Application.Notificaciones;

/// <summary>
/// Servicio para procesar notificaciones de cotizaciones
/// </summary>
public interface INotificacionCotizacionService
{
    /// <summary>
    /// Procesa las notificaciones pendientes de envío
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de notificaciones procesadas</returns>
    Task<int> ProcesarNotificacionesPendientesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Crea una notificación para envío cuando una cotización cambia a Pendiente Aprobación
    /// </summary>
    /// <param name="cotizacionId">ID de la cotización</param>
    /// <param name="versionId">ID de la versión</param>
    /// <param name="emailDestino">Email del destinatario</param>
    /// <param name="fechaProgramada">Fecha programada para el envío (opcional, por defecto inmediato)</param>
    /// <returns>True si se creó correctamente</returns>
    Task<bool> CrearNotificacionPendienteAprobacionAsync(
        string cotizacionId, 
        int versionId, 
        string emailDestino, 
        DateTime? fechaProgramada = null);
    
    /// <summary>
    /// Crea una notificación de seguimiento cuando una cotización es enviada al cliente
    /// </summary>
    /// <param name="cotizacionId">ID de la cotización</param>
    /// <param name="versionId">ID de la versión</param>
    /// <param name="emailDestino">Email del destinatario</param>
    /// <param name="fechaProgramada">Fecha programada para el envío</param>
    /// <returns>True si se creó correctamente</returns>
    Task<bool> CrearNotificacionSeguimientoEnviadaAsync(
        string cotizacionId, 
        int versionId, 
        string emailDestino, 
        DateTime? fechaProgramada = null);
}