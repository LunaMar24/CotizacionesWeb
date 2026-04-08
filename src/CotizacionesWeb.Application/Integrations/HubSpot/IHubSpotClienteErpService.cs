using CotizacionesWeb.Domain.Entities;
using CotizacionesWeb.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Application.Integrations.HubSpot;

/// <summary>
/// Servicio para obtener códigos de cliente ERP desde HubSpot
/// </summary>
public interface IHubSpotClienteErpService
{
    /// <summary>
    /// Obtiene el código de cliente ERP desde HubSpot para un interesado específico
    /// </summary>
    /// <param name="interesado">Interesado con información de HubSpot</param>
    /// <returns>Código de cliente ERP si existe, null si no se encuentra</returns>
    Task<string?> ObtenerClienteErpAsync(Interesado interesado);
}