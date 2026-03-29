using CotizacionesWeb.Domain.Enums;

namespace CotizacionesWeb.Application.Cotizaciones;

public record BuscarInteresadosHubSpotRequest(
    TipoInteresado TipoInteresado,
    string TextoBusqueda
);

public record HubSpotInteresadoSearchItem(
    string HubSpotObjectId,
    string HubSpotObjectType,
    char TipoInteresado,
    string NombreInteresado,
    string? EmailInteresado,
    string? EmpresaInteresado
);

public record AssignInteresadoHubSpotRequest(
    string CotizacionId,
    string HubSpotObjectId,
    string HubSpotObjectType,
    TipoInteresado TipoInteresado,
    string NombreInteresado,
    string? EmailInteresado,
    string? EmpresaInteresado
);

public record AssignInteresadoHubSpotResult(
    bool Success,
    string? ErrorMessage,
    int? InteresadoId,
    char? TipoInteresado,
    string? NombreInteresado,
    string? EmailInteresado,
    string? EmpresaInteresado
);
