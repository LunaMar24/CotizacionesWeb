namespace CotizacionesWeb.Application.Configuracion;

public interface IParametroSistemaService
{
  Task<string?> ObtenerValorParametroAsync(string codigo);
  Task<T?> ObtenerValorParametroAsync<T>(string codigo);
  Task<bool> ActualizarParametroAsync(string codigo, string nuevoValor);
  Task<string> ObtenerSiguienteConsecutivoCotizacionAsync();
  Task<bool> ValidarConfiguracionConsecutivosAsync();
}
