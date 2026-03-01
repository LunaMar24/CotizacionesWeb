using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Application.Cotizaciones;

public class CrearCotizacionService : ICrearCotizacionService
{
    private readonly ILogger<CrearCotizacionService> _logger;

    // TODO: Inject DbContext when data access is wired up
    public CrearCotizacionService(ILogger<CrearCotizacionService> logger)
    {
        _logger = logger;
    }

    public async Task<CrearCotizacionResult> CrearAsync(CrearCotizacionRequest request)
    {
        // TODO: Implement cotizacion creation logic
        _logger.LogInformation("Creating cotizacion for client {Cliente}", request.Cliente);
        await Task.CompletedTask;
        return new CrearCotizacionResult { Success = false, ErrorMessage = "Not implemented" };
    }
}
