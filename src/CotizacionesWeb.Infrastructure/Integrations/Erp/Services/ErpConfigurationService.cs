using CotizacionesWeb.Application.Configuracion;
using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Infrastructure.Integrations.Erp.Services;

/// <summary>
/// Servicio para obtener configuración ERP desde parámetros del sistema
/// </summary>
public interface IErpConfigurationService
{
    Task<ErpIntegrationConfig> GetErpConfigurationAsync();
    Task<bool> ValidateErpConfigurationAsync();
}

public class ErpConfigurationService : IErpConfigurationService
{
    private readonly IParametroSistemaService _parametroService;
    private readonly ILogger<ErpConfigurationService> _logger;

    // Códigos de parámetros ERP
    private const string ERP_CONDICION_PAGO = "ERP_CONDICION_PAGO";
    private const string ERP_BODEGA = "ERP_BODEGA";
    private const string ERP_USUARIO = "ERP_USUARIO";
    private const string ERP_ACTIVIDAD_COMERCIAL = "ERP_ACTIVIDAD_COMERCIAL";
    private const string ERP_CIA = "ERP_CIA";

    public ErpConfigurationService(
        IParametroSistemaService parametroService,
        ILogger<ErpConfigurationService> logger)
    {
        _parametroService = parametroService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la configuración ERP completa desde parámetros del sistema
    /// </summary>
    public async Task<ErpIntegrationConfig> GetErpConfigurationAsync()
    {
        try
        {
            _logger.LogDebug("Obteniendo configuración ERP desde parámetros del sistema");

            var config = new ErpIntegrationConfig
            {
                CondicionPago = await _parametroService.ObtenerValorParametroAsync(ERP_CONDICION_PAGO) ?? string.Empty,
                Bodega = await _parametroService.ObtenerValorParametroAsync(ERP_BODEGA) ?? string.Empty,
                UsuarioERP = await _parametroService.ObtenerValorParametroAsync(ERP_USUARIO) ?? string.Empty,
                ActividadComercial = await _parametroService.ObtenerValorParametroAsync(ERP_ACTIVIDAD_COMERCIAL) ?? string.Empty,
                Cia = await _parametroService.ObtenerValorParametroAsync(ERP_CIA) ?? string.Empty
            };

            if (!config.IsValid())
            {
                var missingParams = config.GetMissingParameters();
                _logger.LogWarning("Configuración ERP incompleta. Parámetros faltantes: {MissingParams}", 
                    string.Join(", ", missingParams));
            }
            else
            {
                _logger.LogDebug("Configuración ERP obtenida exitosamente");
            }

            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener configuración ERP");
            throw new InvalidOperationException("Error al obtener configuración ERP", ex);
        }
    }

    /// <summary>
    /// Valida que la configuración ERP esté completa
    /// </summary>
    public async Task<bool> ValidateErpConfigurationAsync()
    {
        try
        {
            var config = await GetErpConfigurationAsync();
            var isValid = config.IsValid();

            if (!isValid)
            {
                var missingParams = config.GetMissingParameters();
                _logger.LogError("Configuración ERP inválida. Parámetros faltantes: {MissingParams}", 
                    string.Join(", ", missingParams));
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar configuración ERP");
            return false;
        }
    }
}