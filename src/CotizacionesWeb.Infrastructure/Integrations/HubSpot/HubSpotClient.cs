using CotizacionesWeb.Application.Cotizaciones;
using CotizacionesWeb.Application.Integrations;
using CotizacionesWeb.Domain.Enums;
using CotizacionesWeb.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CotizacionesWeb.Infrastructure.Integrations.HubSpot;

public class HubSpotClient : IHubSpotService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<HubSpotClient> _logger;
  private readonly IParametroSistemaService _parametroSistemaService;

  private const string HUBSPOT_ENABLED = "HUBSPOT_ENABLED";
  private const string HUBSPOT_API_BASE_URL = "HUBSPOT_API_BASE_URL";
  private const string HUBSPOT_ACCESS_TOKEN = "HUBSPOT_ACCESS_TOKEN";
  private const string HUBSPOT_OBJECT_CONTACT = "HUBSPOT_OBJECT_CONTACT";
  private const string HUBSPOT_OBJECT_COMPANY = "HUBSPOT_OBJECT_COMPANY";
  private const string HUBSPOT_CONTACT_SEARCH_FIELDS = "HUBSPOT_CONTACT_SEARCH_FIELDS";
  private const string HUBSPOT_COMPANY_SEARCH_FIELDS = "HUBSPOT_COMPANY_SEARCH_FIELDS";
  private const string HUBSPOT_PAGE_SIZE = "HUBSPOT_PAGE_SIZE";
  private const string HUBSPOT_TIMEOUT_SECONDS = "HUBSPOT_TIMEOUT_SECONDS";
  private const string HUBSPOT_RETRY_COUNT = "HUBSPOT_RETRY_COUNT";

  //Si se ocupan más parámetros de búsqueda, se deben configurar en el sistema y
  //mapear en el código, no enviar una lista larga desde el parámetro (Max 6)
  private const int MAX_HUBSOPT_SEARCH_FIELDS = 4;

  public HubSpotClient(HttpClient httpClient, ILogger<HubSpotClient> logger, IParametroSistemaService parametroSistemaService)
  {
    _httpClient = httpClient;
    _logger = logger;
    _parametroSistemaService = parametroSistemaService;
  }

  public async Task EnviarCotizacionAsync(int cotizacionId)
  {
    _logger.LogInformation("HubSpot send for cotizacion {Id} - not implemented yet", cotizacionId);
    await Task.CompletedTask;
  }

  public async Task<List<HubSpotInteresadoSearchItem>> BuscarInteresadosAsync(
      BuscarInteresadosHubSpotRequest request,
      CancellationToken cancellationToken = default)
  {
    if (request is null)
      throw new ArgumentNullException(nameof(request));

    if (string.IsNullOrWhiteSpace(request.TextoBusqueda))
      return new List<HubSpotInteresadoSearchItem>();

    var config = await ObtenerConfiguracionBusquedaAsync();

    if (!config.Enabled)
    {
      _logger.LogWarning("La integración con HubSpot está deshabilitada.");
      return new List<HubSpotInteresadoSearchItem>();
    }

    if (string.IsNullOrWhiteSpace(config.BaseUrl))
      throw new InvalidOperationException("No se encontró configurada la URL base de HubSpot.");

    if (string.IsNullOrWhiteSpace(config.AccessToken))
      throw new InvalidOperationException("No se encontró configurado el token de acceso de HubSpot.");

    var hubSpotObject = ResolverHubSpotObject(request.TipoInteresado, config);
    var searchFields = ResolverSearchFields(request.TipoInteresado, config);

    if (string.IsNullOrWhiteSpace(hubSpotObject))
      throw new InvalidOperationException("No se encontró configurado el objeto HubSpot para el tipo de interesado.");

    if (searchFields is null || searchFields.Length == 0)
      throw new InvalidOperationException("No se encontraron campos de búsqueda HubSpot configurados para el tipo de interesado.");

    _logger.LogInformation(
        "Buscando interesados en HubSpot. Tipo: {TipoInteresado}, Objeto: {Objeto}, Campo: {CampoBusqueda}",
        request.TipoInteresado,
        hubSpotObject,
        string.Join(",", searchFields));

    var endpoint = ConstruirSearchEndpoint(config.BaseUrl, hubSpotObject);
    var requestBody = ConstruirSearchRequestBody(searchFields, request.TextoBusqueda, config.PageSize);

    using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
    httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.AccessToken);
    httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    httpRequest.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

    _httpClient.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);

    using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

    if (!response.IsSuccessStatusCode)
    {
      _logger.LogError(
          "Error consultando HubSpot. StatusCode: {StatusCode}. Response: {Response}",
          response.StatusCode,
          responseContent);

      throw new InvalidOperationException("HubSpot devolvió un error al buscar interesados.");
    }

    var deserializationOptions = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    };

    var hubSpotResponse = JsonSerializer.Deserialize<HubSpotSearchResponse>(
        responseContent,
        deserializationOptions);

    var results = MapearResultadosBusqueda(
        request.TipoInteresado,
        hubSpotObject,
        hubSpotResponse);

    _logger.LogInformation(
        "Búsqueda en HubSpot completada correctamente. Objeto: {Objeto}. Texto: {TextoBusqueda}. Resultados: {Cantidad}",
        hubSpotObject,
        request.TextoBusqueda,
        results.Count);

    return results;
  }


  private static string ResolverHubSpotObject(
    TipoInteresado tipoInteresado,
    HubSpotSearchConfiguration config)
  {
    return tipoInteresado switch
    {
      TipoInteresado.Persona => config.ContactObject,
      TipoInteresado.Empresa => config.CompanyObject,
      _ => throw new InvalidOperationException("Tipo de interesado no soportado para búsqueda en HubSpot.")
    };
  }

  private static string[] ResolverSearchFields(
      TipoInteresado tipoInteresado,
      HubSpotSearchConfiguration config)
  {
    return tipoInteresado switch
    {
      TipoInteresado.Persona => config.ContactSearchFields,
      TipoInteresado.Empresa => config.CompanySearchFields,
      _ => throw new InvalidOperationException("Tipo de interesado no soportado para búsqueda en HubSpot.")
    };
  }

  private async Task<HubSpotSearchConfiguration> ObtenerConfiguracionBusquedaAsync()
  {
    var enabledValue = await _parametroSistemaService.ObtenerValorParametroAsync(HUBSPOT_ENABLED);
    var baseUrl = await _parametroSistemaService.ObtenerValorParametroAsync(HUBSPOT_API_BASE_URL);
    var accessToken = await _parametroSistemaService.ObtenerValorParametroAsync(HUBSPOT_ACCESS_TOKEN);
    var contactObject = await _parametroSistemaService.ObtenerValorParametroAsync(HUBSPOT_OBJECT_CONTACT);
    var companyObject = await _parametroSistemaService.ObtenerValorParametroAsync(HUBSPOT_OBJECT_COMPANY);
    var contactSearchFieldsRaw = await _parametroSistemaService.ObtenerValorParametroAsync(HUBSPOT_CONTACT_SEARCH_FIELDS);
    var companySearchFieldsRaw = await _parametroSistemaService.ObtenerValorParametroAsync(HUBSPOT_COMPANY_SEARCH_FIELDS);
    var pageSize = await _parametroSistemaService.ObtenerValorParametroAsync<int>(HUBSPOT_PAGE_SIZE);
    var timeoutSeconds = await _parametroSistemaService.ObtenerValorParametroAsync<int>(HUBSPOT_TIMEOUT_SECONDS);
    var retryCount = await _parametroSistemaService.ObtenerValorParametroAsync<int>(HUBSPOT_RETRY_COUNT);

    return new HubSpotSearchConfiguration(
        Enabled: string.Equals(enabledValue, "S", StringComparison.OrdinalIgnoreCase),
        BaseUrl: baseUrl ?? string.Empty,
        AccessToken: accessToken ?? string.Empty,
        ContactObject: contactObject ?? "contacts",
        CompanyObject: companyObject ?? "companies",
        ContactSearchFields: ParseSearchFields(contactSearchFieldsRaw),
        CompanySearchFields: ParseSearchFields(companySearchFieldsRaw),
        PageSize: pageSize > 0 ? pageSize : 100,
        TimeoutSeconds: timeoutSeconds > 0 ? timeoutSeconds : 30,
        RetryCount: retryCount >= 0 ? retryCount : 3
    );
  }

  #region Helpers

  private static string ConstruirSearchEndpoint(string baseUrl, string hubSpotObject)
  {
    var trimmedBaseUrl = baseUrl.TrimEnd('/');
    return $"{trimmedBaseUrl}/crm/v3/objects/{hubSpotObject}/search";
  }

  private static string ConstruirSearchRequestBody(
    string[] searchFields,
    string textoBusqueda,
    int pageSize)
  {
    var validFields = searchFields
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Take(MAX_HUBSOPT_SEARCH_FIELDS)
        .ToArray();

    var payload = new
    {
      filterGroups = validFields
            .Select(field => new
            {
              filters = new[]
                {
                    new
                    {
                        propertyName = field,
                        @operator = "CONTAINS_TOKEN",
                        value = textoBusqueda
                    }
                }
            })
            .ToArray(),
      limit = pageSize,
      properties = new[]
        {
            "firstname",
            "lastname",
            "email",
            "phone",
            "jobtitle",
            "company",
            "mobilephone",
            "name",
            "domain"
        }
    };

    return JsonSerializer.Serialize(payload);
  }

  private static string ConstruirNombreInteresado(
    TipoInteresado tipoInteresado,
    HubSpotSearchProperties? properties)
  {
    if (properties is null)
      return string.Empty;

    if (tipoInteresado == TipoInteresado.Empresa)
      return properties.name ?? string.Empty;

    var nombreCompleto = string.Join(" ",
        new[] { properties.firstname, properties.lastname }
            .Where(x => !string.IsNullOrWhiteSpace(x)));

    return !string.IsNullOrWhiteSpace(nombreCompleto)
        ? nombreCompleto
        : properties.email ?? string.Empty;
  }

  private static List<HubSpotInteresadoSearchItem> MapearResultadosBusqueda(
    TipoInteresado tipoInteresado,
    string hubSpotObject,
    HubSpotSearchResponse? response)
  {
    if (response?.Results is null || response.Results.Count == 0)
      return new List<HubSpotInteresadoSearchItem>();

    return response.Results
        .Where(r => !string.IsNullOrWhiteSpace(r.Id))
        .Select(r =>
        {
          var nombre = ConstruirNombreInteresado(tipoInteresado, r.Properties);
          var email = tipoInteresado == TipoInteresado.Persona
              ? r.Properties?.email
              : "N/A";
          var empresa = tipoInteresado == TipoInteresado.Empresa
              ? r.Properties?.domain
              : r.Properties?.company;

          return new HubSpotInteresadoSearchItem(
              HubSpotObjectId: r.Id,
              HubSpotObjectType: hubSpotObject,
              TipoInteresado: tipoInteresado == TipoInteresado.Persona ? 'P' : 'E',
              NombreInteresado: nombre,
              EmailInteresado: email,
              EmpresaInteresado: empresa
          );
        })
        .ToList();
  }

  private static string[] ParseSearchFields(string? rawValue)
  {
    if (string.IsNullOrWhiteSpace(rawValue))
      return Array.Empty<string>();

    return rawValue
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Take(MAX_HUBSOPT_SEARCH_FIELDS)
        .ToArray();
  }

  #endregion Helpers

  #region Records Internos Integracion

  private sealed record HubSpotSearchConfiguration(
    bool Enabled,
    string BaseUrl,
    string AccessToken,
    string ContactObject,
    string CompanyObject,
    string[] ContactSearchFields,
    string[] CompanySearchFields,
    int PageSize,
    int TimeoutSeconds,
    int RetryCount
);

  private sealed record HubSpotSearchResponse(
    List<HubSpotSearchResultItem>? Results
);

  private sealed record HubSpotSearchResultItem(
      string Id,
      HubSpotSearchProperties? Properties
  );

  private sealed record HubSpotSearchProperties(
      string? firstname,
      string? lastname,
      string? email,
      string? phone,
      string? jobtitle,
      string? company,
      string? mobilephone,
      string? name,
      string? domain
  );

  #endregion Records Internos Integracion

}
