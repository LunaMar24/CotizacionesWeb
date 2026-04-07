using CotizacionesWeb.Application.Cotizaciones;

namespace CotizacionesWeb.Application.Integrations.HubSpot;

public interface IHubSpotService
{
  Task EnviarCotizacionAsync(int cotizacionId);

  Task<List<HubSpotInteresadoSearchItem>> BuscarInteresadosAsync(
      BuscarInteresadosHubSpotRequest request,
      CancellationToken cancellationToken = default);
}
