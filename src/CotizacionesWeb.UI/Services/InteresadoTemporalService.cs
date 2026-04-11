using Microsoft.AspNetCore.Http;

namespace CotizacionesWeb.UI.Services;

/// <summary>
/// Implementación del servicio de interesados temporales usando sesión
/// </summary>
public class InteresadoTemporalService : IInteresadoTemporalService
{
    private const string SESSION_KEY_INTERESADO_TEMPORAL = "InteresadoTemporal_CreacionCotizacion";
    
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<InteresadoTemporalService> _logger;

    public InteresadoTemporalService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<InteresadoTemporalService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public void GuardarInteresadoTemporal(int interesadoId, string nombreInteresado, string? emailInteresado, string? empresaInteresado, char tipoInteresado)
    {
        try
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                _logger.LogWarning("No hay sesión disponible para guardar interesado temporal");
                return;
            }

            var interesadoTemporal = new InteresadoTemporal(
                interesadoId,
                nombreInteresado,
                emailInteresado,
                empresaInteresado,
                tipoInteresado
            );

            // Serializar a JSON y guardar en sesión
            var json = System.Text.Json.JsonSerializer.Serialize(interesadoTemporal);
            session.SetString(SESSION_KEY_INTERESADO_TEMPORAL, json);
            
            _logger.LogInformation("Interesado temporal guardado: ID={InteresadoId}, Nombre={Nombre}", 
                interesadoId, nombreInteresado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar interesado temporal");
        }
    }

    public InteresadoTemporal? ObtenerInteresadoTemporal()
    {
        try
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                return null;
            }

            var json = session.GetString(SESSION_KEY_INTERESADO_TEMPORAL);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var interesadoTemporal = System.Text.Json.JsonSerializer.Deserialize<InteresadoTemporal>(json);
            
            _logger.LogDebug("Interesado temporal recuperado: ID={InteresadoId}", 
                interesadoTemporal?.InteresadoId);
                
            return interesadoTemporal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener interesado temporal");
            return null;
        }
    }

    public void LimpiarInteresadoTemporal()
    {
        try
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                return;
            }

            session.Remove(SESSION_KEY_INTERESADO_TEMPORAL);
            
            _logger.LogDebug("Interesado temporal limpiado de la sesión");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al limpiar interesado temporal");
        }
    }
}