namespace CotizacionesWeb.Domain.Entities;

/// <summary>
/// Entidad para control de integración de pedidos con ERP
/// Registra el resultado del envío de cotizaciones hacia el ERP y soporta reintentos controlados
/// </summary>
public class IntegracionPedidoErp
{
    public int IntegracionId { get; set; }  // PK
    public string CotizacionId { get; set; } = string.Empty;
    public int VersionId { get; set; }
    public Guid LoteId { get; set; }  // Identifica el lote para staging ERP, reutilizable en reintentos
    public string Estado { get; set; } = string.Empty;  // Pendiente, Procesado, Error
    public string? PedidoErp { get; set; }  // ID del pedido generado en ERP
    public int Intentos { get; set; } = 0;  // Contador de intentos de procesamiento
    public string? MensajeError { get; set; }  // Mensaje de error del último intento
    public DateTime FechaCreacion { get; set; }  // Fecha de creación de la integración
    public DateTime? FechaProcesado { get; set; }  // Fecha cuando se procesó exitosamente

    // Relaciones
    public Cotizacion Cotizacion { get; set; } = null!;
    public CotizacionVersion Version { get; set; } = null!;
}