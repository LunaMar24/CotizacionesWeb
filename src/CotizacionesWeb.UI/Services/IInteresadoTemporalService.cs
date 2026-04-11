namespace CotizacionesWeb.UI.Services;

/// <summary>
/// Servicio para manejar interesados temporales durante la creación de cotizaciones
/// </summary>
public interface IInteresadoTemporalService
{
    /// <summary>
    /// Guardar InteresadoId temporalmente para una sesión de creación de cotización
    /// </summary>
    void GuardarInteresadoTemporal(int interesadoId, string nombreInteresado, string? emailInteresado, string? empresaInteresado, char tipoInteresado);
    
    /// <summary>
    /// Obtener InteresadoId temporal guardado
    /// </summary>
    InteresadoTemporal? ObtenerInteresadoTemporal();
    
    /// <summary>
    /// Limpiar datos temporales
    /// </summary>
    void LimpiarInteresadoTemporal();
}

/// <summary>
/// Datos del interesado guardados temporalmente
/// </summary>
public record InteresadoTemporal(
    int InteresadoId,
    string NombreInteresado,
    string? EmailInteresado,
    string? EmpresaInteresado,
    char TipoInteresado
);