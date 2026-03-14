using CotizacionesWeb.Domain.Common;

namespace CotizacionesWeb.Domain.Entities;

public class Cotizacion : BaseEntity
{
    public string CotizacionId { get; set; } = string.Empty;
    public int? InteresadoId { get; set; }
    public char EstadoActual { get; set; }
    public int VersionActual { get; set; }
    public decimal MontoCotizacion { get; set; }
    public DateTime? FechaEnvio { get; set; }
    
    public Interesado? Interesado { get; set; }
    public ICollection<CotizacionVersion> Versiones { get; set; } = new List<CotizacionVersion>();
    public ICollection<ArchivoCotizacion> Archivos { get; set; } = new List<ArchivoCotizacion>();
}
