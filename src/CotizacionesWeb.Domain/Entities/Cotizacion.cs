using CotizacionesWeb.Domain.Common;
using CotizacionesWeb.Domain.Enums;

namespace CotizacionesWeb.Domain.Entities;

public class Cotizacion : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime Fecha { get; set; }
    public EstadoIntegracion EstadoErp { get; set; } = EstadoIntegracion.Pendiente;
    public EstadoIntegracion EstadoHubSpot { get; set; } = EstadoIntegracion.Pendiente;
}
