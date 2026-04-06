namespace CotizacionesWeb.Application.Notificaciones;

/// <summary>
/// DTO con los datos necesarios para generar el contenido de una notificación
/// </summary>
public record NotificacionCotizacionDto(
    int NotificacionId,
    string CotizacionId,
    int VersionId,
    string TipoNotificacion,
    string EmailDestino,
    DateTime FechaProgramada,
    int Intentos,
    
    // Datos de la cotización
    string EstadoCotizacion,
    decimal MontoCotizacion,
    string Moneda,
    
    // Datos de la versión
    decimal NumeroVersion,
    string NombreInteresado,
    string EmailInteresado,
    string EmpresaInteresado
);

/// <summary>
/// Resultado del procesamiento de una notificación
/// </summary>
public record ResultadoProcesamiento(
    bool Exitoso,
    string? MensajeError = null,
    DateTime? FechaEnviada = null
);

/// <summary>
/// Resumen del procesamiento de notificaciones
/// </summary>
public record ResumenProcesamiento(
    int NotificacionesEncontradas,
    int NotificacionesExitosas,
    int NotificacionesConError
);