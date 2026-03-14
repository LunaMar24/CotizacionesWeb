using CotizacionesWeb.Domain.Common;

namespace CotizacionesWeb.Domain.Entities;

public class CotizacionVersion : BaseEntity
{
    public string CotizacionId { get; set; } = string.Empty;
    public int NumeroVersion { get; set; }
    public DateTime FechaVersion { get; set; }
    public string NombreInteresado { get; set; } = string.Empty;
    public string EmailInteresado { get; set; } = string.Empty;
    public string EmpresaInteresado { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public decimal? TipoCambio { get; set; }
    public int VersionActual { get; set; } // Cambiado de char a int
    public string? Notas { get; set; }
    
    public Cotizacion Cotizacion { get; set; } = null!;
    public ICollection<DetalleCotizacionVersion> Detalles { get; set; } = new List<DetalleCotizacionVersion>();
    public ICollection<HistorialCotizacion> Historiales { get; set; } = new List<HistorialCotizacion>();
}
