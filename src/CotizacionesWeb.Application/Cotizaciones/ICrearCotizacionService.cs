namespace CotizacionesWeb.Application.Cotizaciones;

public interface ICrearCotizacionService
{
    Task<CrearCotizacionResult> CrearAsync(CrearCotizacionRequest request);
}
