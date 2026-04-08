namespace CotizacionesWeb.Application.Integrations.Erp;

/// <summary>
/// Resultado de la generación de pedido en ERP
/// </summary>
public record GenerarPedidoResult(
    bool Success,
    string? PedidoErp = null,
    string? Message = null,
    int IntegracionId = 0,
    Guid? LoteId = null
);

/// <summary>
/// Servicio para integración con ERP para generación de pedidos
/// </summary>
public interface IErpPedidoService
{
    /// <summary>
    /// Genera un pedido en ERP a partir de una cotización
    /// </summary>
    /// <param name="cotizacionId">ID de la cotización</param>
    /// <param name="versionId">ID de la versión específica</param>
    /// <returns>Resultado de la operación</returns>
    Task<GenerarPedidoResult> GenerarPedidoAsync(string cotizacionId, int versionId);

    /// <summary>
    /// Valida que una cotización esté en estado válido para envío a ERP
    /// </summary>
    /// <param name="cotizacionId">ID de la cotización</param>
    /// <returns>True si está en estado válido</returns>
    Task<bool> ValidarEstadoCotizacionParaErpAsync(string cotizacionId);
}