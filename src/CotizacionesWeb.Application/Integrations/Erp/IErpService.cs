using CotizacionesWeb.Domain.Entities.ERP;

namespace CotizacionesWeb.Application.Integrations.Erp;

public interface IErpService
{
  Task<List<ProductoErp>> ObtenerProductosAsync(
      string monedaCotizacion,
      string? textoBusqueda = null);
}