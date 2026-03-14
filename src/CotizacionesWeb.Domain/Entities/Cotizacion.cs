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
    
    // Nuevos campos según lineamientos funcionales
    public DateTime? FechaAceptacion { get; set; }
    public DateTime? FechaRechazo { get; set; }
    public char EnviadoERP { get; set; } = 'N'; // S/N, default N
    public DateTime? FechaEnvioERP { get; set; }
    
    public Interesado? Interesado { get; set; }
    public ICollection<CotizacionVersion> Versiones { get; set; } = new List<CotizacionVersion>();
    public ICollection<ArchivoCotizacion> Archivos { get; set; } = new List<ArchivoCotizacion>();
}
