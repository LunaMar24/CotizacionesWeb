using CotizacionesWeb.Domain.Common;
using CotizacionesWeb.Domain.Enums;

namespace CotizacionesWeb.Domain.Entities;

public class Interesado : BaseEntity
{
    public int InteresadoId { get; set; }  // FASE 3: Llave primaria específica según modelo
    public string HubspotObjectId { get; set; } = string.Empty;
    public string HubspotObjectType { get; set; } = string.Empty;
    public char TipoInteresado { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime? FechaUltSync { get; set; }
    
    public ICollection<Cotizacion> Cotizaciones { get; set; } = new List<Cotizacion>();
}
