using CotizacionesWeb.Domain.Entities.ERP;

namespace CotizacionesWeb.Application.Integrations;

public interface IErpService
{
  Task<List<ProductoErp>> ObtenerProductosAsync(
      string monedaCotizacion,
      string? textoBusqueda = null);
}