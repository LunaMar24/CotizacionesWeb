using CotizacionesWeb.Domain.Common;

namespace CotizacionesWeb.Domain.Entities;

public class DetalleCotizacionVersion : BaseEntity
{
    public int DetalleVersionId { get; set; }  // FASE 3: Llave primaria específica según modelo
    public int VersionId { get; set; }
    public string ProductoId { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public decimal PorcentajeImpuesto { get; set; } // Porcentaje de impuesto (ej: 13.00)
    public decimal TotalLinea { get; set; }
    
    public CotizacionVersion Version { get; set; } = null!;
}
