using CotizacionesWeb.Domain.Entities;

namespace CotizacionesWeb.Application.Cotizaciones;

public class CrearCotizacionRequest
{
    public string Cliente { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime Fecha { get; set; }
}

public class CrearCotizacionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int? CotizacionId { get; set; }
}
