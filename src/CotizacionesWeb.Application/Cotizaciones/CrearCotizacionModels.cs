using CotizacionesWeb.Domain.Entities;

namespace CotizacionesWeb.Application.Cotizaciones;

public class CrearCotizacionRequest
{
    public string NombreInteresado { get; set; } = string.Empty;
    public string EmailInteresado { get; set; } = string.Empty;
    public string EmpresaInteresado { get; set; } = string.Empty;
    public char TipoInteresado { get; set; } = 'P';
    public string Moneda { get; set; } = "CRC";
    public decimal? TipoCambio { get; set; }
    public string Notas { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal TotalDescuentos { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Total { get; set; }
    public List<CrearDetalleRequest> Detalles { get; set; } = new();
}

public class CrearDetalleRequest
{
    public string ProductoId { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public decimal PorcentajeImpuesto { get; set; }
    public decimal TotalLinea { get; set; }
}

public class CrearCotizacionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CotizacionId { get; set; } // Cambio de int a string para usar el formato COT-XXXX
}
