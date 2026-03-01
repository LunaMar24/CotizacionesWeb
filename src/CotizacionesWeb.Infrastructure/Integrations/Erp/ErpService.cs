using CotizacionesWeb.Application.Integrations;
using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Infrastructure.Integrations.Erp;

public class ErpService : IErpService
{
    private readonly ILogger<ErpService> _logger;

    public ErpService(ILogger<ErpService> logger)
    {
        _logger = logger;
    }

    public async Task SincronizarCotizacionAsync(int cotizacionId)
    {
        // TODO: Implement ERP synchronization when integration is defined
        _logger.LogInformation("ERP sync for cotizacion {Id} - not implemented", cotizacionId);
        await Task.CompletedTask;
    }
}
