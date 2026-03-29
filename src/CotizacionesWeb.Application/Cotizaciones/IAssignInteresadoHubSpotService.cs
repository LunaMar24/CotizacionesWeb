namespace CotizacionesWeb.Application.Cotizaciones;

public interface IAssignInteresadoHubSpotService
{
  Task<AssignInteresadoHubSpotResult> AssignAsync(AssignInteresadoHubSpotRequest request, int? userId = null);
}
