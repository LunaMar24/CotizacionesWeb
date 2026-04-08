namespace CotizacionesWeb.Domain.Entities.ERP;

/// <summary>
/// Entidad para tabla staging COTWEB_PEDIDO_STG en ERP
/// </summary>
public class CotwebPedidoStg
{
    public Guid LoteId { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string CondicionPago { get; set; } = string.Empty;
    public string Bodega { get; set; } = string.Empty;
    public string Moneda { get; set; } = string.Empty;
    public string NivelPrecio { get; set; } = string.Empty; // AGREGADO
    public decimal TipoCambio { get; set; }
    public string UsuarioERP { get; set; } = string.Empty;
    public string ActividadComercial { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}