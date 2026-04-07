using CotizacionesWeb.Application.Configuracion;
using CotizacionesWeb.Application.Integrations.HubSpot.Services;
using CotizacionesWeb.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CotizacionesWeb.Infrastructure.Integrations.HubSpot.Services;

/// <summary>
/// Servicio para obtener códigos de cliente ERP desde HubSpot
/// </summary>
public class HubSpotClienteErpService : IHubSpotClienteErpService
{
    private readonly HttpClient _httpClient;
    private readonly IParametroSistemaService _parametroSistemaService;
    private readonly ILogger<HubSpotClienteErpService> _logger;

    // Parámetros de configuración
    private const string HUBSPOT_ENABLED = "HUBSPOT_ENABLED";
    private const string HUBSPOT_API_BASE_URL = "HUBSPOT_API_BASE_URL";
    private const string HUBSPOT_ACCESS_TOKEN = "HUBSPOT_ACCESS_TOKEN";
    private const string HUBSPOT_CONTACTO_CAMPO_CLIENTE_ERP = "HUBSPOT_CONTACTO_CAMPO_CLIENTE_ERP";
    private const string HUBSPOT_EMPRESA_CAMPO_CLIENTE_ERP = "HUBSPOT_EMPRESA_CAMPO_CLIENTE_ERP";

    public HubSpotClienteErpService(
        HttpClient httpClient,
        IParametroSistemaService parametroSistemaService,
        ILogger<HubSpotClienteErpService> logger)
    {
        _httpClient = httpClient;
        _parametroSistemaService = parametroSistemaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene el código de cliente ERP desde HubSpot para un interesado específico
    /// </summary>
    public async Task<string?> ObtenerClienteErpAsync(Interesado interesado)
    {
        if (interesado == null)
        {
            _logger.LogWarning("Interesado es null, no se puede obtener cliente ERP");
            return null;
        }

        try
        {
            // Validar que exista HubspotObjectId antes de construir la URL
            if (string.IsNullOrWhiteSpace(interesado.HubspotObjectId))
            {
                _logger.LogWarning("HubspotObjectId está vacío o nulo para interesado {InteresadoId}, no se puede obtener cliente ERP", 
                    interesado.InteresadoId);
                return null;
            }

            // Verificar si HubSpot está habilitado (manteniendo consistencia con HubSpotClient)
            var hubspotEnabled = await _parametroSistemaService.ObtenerValorParametroAsync(HUBSPOT_ENABLED);
            if (!string.Equals(hubspotEnabled, "S", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Integración HubSpot deshabilitada, no se puede obtener cliente ERP");
                return null;
            }

            // Obtener configuración
            var config = await ObtenerConfiguracionAsync();
            if (!ValidarConfiguracion(config))
            {
                _logger.LogError("Configuración HubSpot incompleta para obtener cliente ERP");
                return null;
            }

            // Determinar el campo a consultar según el tipo de interesado
            var campoClienteErp = DeterminarCampoClienteErp(interesado.TipoInteresado, config);
            if (string.IsNullOrEmpty(campoClienteErp))
            {
                _logger.LogWarning("No se encontró configurado el campo de cliente ERP para tipo {TipoInteresado}", 
                    interesado.TipoInteresado);
                return null;
            }

            // Consultar HubSpot
            var clienteErp = await ConsultarHubSpotAsync(interesado, campoClienteErp, config);
            if (!string.IsNullOrEmpty(clienteErp))
            {
                _logger.LogDebug("Cliente ERP obtenido desde HubSpot: {ClienteErp} para interesado {InteresadoId}", 
                    clienteErp, interesado.InteresadoId);
            }
            else
            {
                _logger.LogInformation("No se encontró cliente ERP en HubSpot para interesado {InteresadoId} (Tipo: {Tipo})", 
                    interesado.InteresadoId, interesado.TipoInteresado);
            }

            return clienteErp;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cliente ERP desde HubSpot para interesado {InteresadoId}", 
                interesado.InteresadoId);
            return null;
        }
    }

    /// <summary>
    /// Obtiene la configuración necesaria para la consulta a HubSpot
    /// </summary>
    private async Task<HubSpotClienteErpConfig> ObtenerConfiguracionAsync()
    {
        var baseUrl = await _parametroSistemaService.ObtenerValorParametroAsync(HUBSPOT_API_BASE_URL);
        var accessToken = await _parametroSistemaService.ObtenerValorParametroAsync(HUBSPOT_ACCESS_TOKEN);
        var campoContacto = await _parametroSistemaService.ObtenerValorParametroAsync(HUBSPOT_CONTACTO_CAMPO_CLIENTE_ERP);
        var campoEmpresa = await _parametroSistemaService.ObtenerValorParametroAsync(HUBSPOT_EMPRESA_CAMPO_CLIENTE_ERP);

        return new HubSpotClienteErpConfig(
            BaseUrl: baseUrl?.TrimEnd('/') ?? string.Empty,
            AccessToken: accessToken ?? string.Empty,
            CampoContactoClienteErp: campoContacto ?? string.Empty,
            CampoEmpresaClienteErp: campoEmpresa ?? string.Empty
        );
    }

    /// <summary>
    /// Valida que la configuración esté completa
    /// </summary>
    private bool ValidarConfiguracion(HubSpotClienteErpConfig config)
    {
        return !string.IsNullOrEmpty(config.BaseUrl) && 
               !string.IsNullOrEmpty(config.AccessToken);
    }

    /// <summary>
    /// Determina qué campo consultar según el tipo de interesado
    /// </summary>
    private string? DeterminarCampoClienteErp(char tipoInteresado, HubSpotClienteErpConfig config)
    {
        return tipoInteresado switch
        {
            'P' => config.CampoContactoClienteErp, // Persona
            'E' => config.CampoEmpresaClienteErp,  // Empresa
            _ => null
        };
    }

    /// <summary>
    /// Realiza la consulta a HubSpot para obtener el cliente ERP
    /// </summary>
    private async Task<string?> ConsultarHubSpotAsync(Interesado interesado, string campoClienteErp, HubSpotClienteErpConfig config)
    {
        var objectType = DeterminarTipoObjecto(interesado.TipoInteresado);
        if (string.IsNullOrEmpty(objectType))
        {
            _logger.LogWarning("Tipo de objeto HubSpot no válido para tipo interesado {Tipo}", interesado.TipoInteresado);
            return null;
        }

        var endpoint = $"{config.BaseUrl}/crm/v3/objects/{objectType}/{interesado.HubspotObjectId}";
        
        // Agregar el campo específico como parámetro para optimizar la consulta
        endpoint += $"?properties={campoClienteErp}";

        _logger.LogDebug("Consultando HubSpot: {Endpoint} para campo {Campo}", endpoint, campoClienteErp);

        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.AccessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Usar timeout configurado en DI en lugar de modificar HttpClient.Timeout
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // Timeout fijo por llamada
        
        using var response = await _httpClient.SendAsync(request, cts.Token);
        var responseContent = await response.Content.ReadAsStringAsync(cts.Token);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Error al consultar HubSpot. Status: {Status}, Response: {Response}", 
                response.StatusCode, responseContent);
            return null;
        }

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var hubspotObject = JsonSerializer.Deserialize<HubSpotObjectResponse>(responseContent, options);

            var clienteErp = ExtraerValorCampo(hubspotObject?.Properties, campoClienteErp);
            return !string.IsNullOrWhiteSpace(clienteErp) ? clienteErp.Trim() : null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error al deserializar respuesta de HubSpot");
            return null;
        }
    }

    /// <summary>
    /// Determina el tipo de objeto HubSpot según el tipo de interesado
    /// </summary>
    private static string? DeterminarTipoObjecto(char tipoInteresado)
    {
        return tipoInteresado switch
        {
            'P' => "contacts", // Persona
            'E' => "companies", // Empresa
            _ => null
        };
    }

    /// <summary>
    /// Extrae el valor del campo específico desde las propiedades del objeto HubSpot
    /// </summary>
    private string? ExtraerValorCampo(Dictionary<string, object>? properties, string nombreCampo)
    {
        if (properties == null || !properties.ContainsKey(nombreCampo))
            return null;

        var valor = properties[nombreCampo];
        if (valor is JsonElement jsonElement)
        {
            return jsonElement.ValueKind == JsonValueKind.String ? jsonElement.GetString() : null;
        }

        return valor?.ToString();
    }

    #region Records y DTOs

    /// <summary>
    /// Configuración para obtener cliente ERP desde HubSpot
    /// </summary>
    private sealed record HubSpotClienteErpConfig(
        string BaseUrl,
        string AccessToken,
        string CampoContactoClienteErp,
        string CampoEmpresaClienteErp
    );

    /// <summary>
    /// Respuesta de un objeto HubSpot
    /// </summary>
    private sealed record HubSpotObjectResponse(
        string? Id,
        Dictionary<string, object>? Properties
    );

    #endregion
}