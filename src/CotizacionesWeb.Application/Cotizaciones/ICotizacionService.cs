namespace CotizacionesWeb.Application.Cotizaciones;

public interface ICotizacionService
{
    // Listar cotizaciones (versión actual vigente)
    Task<List<CotizacionListDto>> GetCotizacionesListAsync(GetCotizacionesListRequest request);
    
    // Obtener historial de la versión actual de una cotización
    Task<List<HistorialCotizacionDto>> GetCotizacionCurrentHistoryAsync(string cotizacionId);
    
    // Obtener todas las versiones de una cotización
    Task<List<CotizacionVersionDto>> GetCotizacionVersionsAsync(string cotizacionId);
    
    // Obtener detalle completo de una versión específica
    Task<CotizacionVersionDetalleDto?> GetCotizacionVersionDetailAsync(int versionId);
    
    // Obtener historial de una versión específica
    Task<List<HistorialCotizacionDto>> GetCotizacionVersionHistoryAsync(int versionId);
    
    // Copiar versión actual (desde listado principal)
    Task<CopiarVersionResult> CopiarVersionActualAsync(string cotizacionId);
    
    // Copiar versión específica (desde sub-listado de versiones)
    Task<CopiarVersionResult> CopiarVersionEspecificaAsync(CopiarVersionRequest request);
    
    // Duplicar cotización (crear nueva independiente)
    Task<DuplicarCotizacionResult> DuplicarCotizacionAsync(DuplicarCotizacionRequest request);
}
