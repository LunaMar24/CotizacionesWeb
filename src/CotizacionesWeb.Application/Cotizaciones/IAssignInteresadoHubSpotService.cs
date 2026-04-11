namespace CotizacionesWeb.Application.Cotizaciones;

public interface IAssignInteresadoHubSpotService
{
  Task<AssignInteresadoHubSpotResult> AssignAsync(AssignInteresadoHubSpotRequest request, int? userId = null);
  
  /// <summary>
  /// Resuelve (busca o crea) un interesado desde HubSpot sin asociarlo a una cotización específica
  /// Usado para el flujo de creación de cotizaciones nuevas
  /// </summary>
  Task<AssignInteresadoHubSpotResult> ResolverInteresadoAsync(ResolverInteresadoHubSpotRequest request, int? userId = null);
}
