namespace CotizacionesWeb.Domain.Entities.ERP;

public class ProductoErp
{
  public string Producto { get; set; } = string.Empty;
  public string Descripcion { get; set; } = string.Empty;
  public string CodigoImpuesto { get; set; } = string.Empty;
  public string Tarifa { get; set; } = string.Empty;
  public string Tipo { get; set; } = string.Empty;
  public decimal Porcentaje { get; set; }
  public decimal? Precio { get; set; }
  public string? NivelPrecio { get; set; }
  public string? Moneda { get; set; }
}
