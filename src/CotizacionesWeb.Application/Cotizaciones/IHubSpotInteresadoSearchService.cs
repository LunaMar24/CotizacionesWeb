namespace CotizacionesWeb.Application.Cotizaciones;

public interface IHubSpotInteresadoSearchService
{
  Task<List<HubSpotInteresadoSearchItem>> BuscarAsync(BuscarInteresadosHubSpotRequest request);
}
