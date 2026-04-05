namespace CotizacionesWeb.Application.Documents;

/// <summary>
/// Servicio para generación de documentos Word de cotizaciones
/// </summary>
public interface IDocumentoCotizacionService
{
    /// <summary>
    /// Genera un documento Word de cotización basado en plantilla
    /// </summary>
    /// <param name="request">Request con datos de la cotización</param>
    /// <returns>Resultado con el documento generado</returns>
    Task<GenerarDocumentoResult> GenerarDocumentoCotizacionAsync(GenerarDocumentoCotizacionRequest request);
    
    /// <summary>
    /// Verifica si existe la plantilla base y está configurada correctamente
    /// </summary>
    /// <returns>True si la plantilla está disponible</returns>
    Task<bool> ValidarPlantillaAsync();
    
    /// <summary>
    /// Obtiene información sobre los placeholders disponibles en la plantilla
    /// </summary>
    /// <returns>Lista de placeholders encontrados</returns>
    Task<List<string>> ObtenerPlaceholdersDisponiblesAsync();
}