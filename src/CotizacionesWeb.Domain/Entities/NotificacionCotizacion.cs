using CotizacionesWeb.Domain.Common;

namespace CotizacionesWeb.Domain.Entities;

/// <summary>
/// Entidad para gestión de notificaciones de cotizaciones por email
/// </summary>
public class NotificacionCotizacion : BaseEntity
{
    public int NotificacionId { get; set; }  // PK específica siguiendo el patrón del proyecto
    public string CotizacionId { get; set; } = string.Empty;
    public int VersionId { get; set; }
    public string TipoNotificacion { get; set; } = string.Empty;
    public string EmailDestino { get; set; } = string.Empty;
    public string? Asunto { get; set; }
    public string? Cuerpo { get; set; }
    public DateTime FechaProgramada { get; set; }
    public DateTime? FechaEnviada { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int Intentos { get; set; } = 0;
    public string? MensajeError { get; set; }

    // Relaciones
    public Cotizacion Cotizacion { get; set; } = null!;
    public CotizacionVersion Version { get; set; } = null!;
}