namespace CotizacionesWeb.Domain.Entities.ERP;

/// <summary>
/// Entidad para tabla staging COTWEB_PEDIDO_LINEA_STG en ERP
/// </summary>
public class CotwebPedidoLineaStg
{
    public Guid LoteId { get; set; }
    public int Linea { get; set; }
    public string Producto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal MontoDescuento { get; set; }
    public decimal PorcentajeImpuesto { get; set; } // CAMBIADO: era PorcentajeDescuento
    public decimal Subtotal { get; set; }
    public string Bodega { get; set; } = string.Empty;
}