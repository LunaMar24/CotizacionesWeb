using CotizacionesWeb.Application.Integrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Infrastructure.Integrations.HubSpot;

public class HubSpotClient : IHubSpotService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HubSpotClient> _logger;

    public HubSpotClient(HttpClient httpClient, ILogger<HubSpotClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task EnviarCotizacionAsync(int cotizacionId)
    {
        // TODO: Implement HubSpot integration when integration is defined
        _logger.LogInformation("HubSpot send for cotizacion {Id} - not implemented", cotizacionId);
        await Task.CompletedTask;
    }
}
