namespace CotizacionesWeb.Application.Cotizaciones;

public interface ICotizacionService
{
    // Listar cotizaciones (versión actual vigente)
    Task<List<CotizacionListDto>> GetCotizacionesListAsync(GetCotizacionesListRequest request);
    
    // Listar cotizaciones archivadas con filtros específicos
    Task<List<CotizacionListDto>> GetCotizacionesArchivadasAsync(GetCotizacionesArchivadasRequest request);
    
    // Método legacy para mantener compatibilidad (deprecated)
    [Obsolete("Use GetCotizacionesArchivadasAsync(GetCotizacionesArchivadasRequest) instead")]
    Task<List<CotizacionListDto>> GetCotizacionesArchivadasAsync(GetCotizacionesListRequest request);
    
    // Obtener historial de la versión actual de una cotización
    Task<List<HistorialCotizacionDto>> GetCotizacionCurrentHistoryAsync(string cotizacionId);
    
    // Obtener todas las versiones de una cotización
    Task<List<CotizacionVersionDto>> GetCotizacionVersionsAsync(string cotizacionId);
    
    // Obtener detalle completo de una versión específica
    Task<CotizacionVersionDetalleDto?> GetCotizacionVersionDetailAsync(int versionId);
    
    // Obtener historial de una versión específica
    Task<List<HistorialCotizacionDto>> GetCotizacionVersionHistoryAsync(int versionId);
    
    // Crear nueva cotización
    Task<CrearCotizacionResult> CrearCotizacionAsync(CrearCotizacionRequest request, int? userId = null);
    
    // Copiar versión actual (desde listado principal)
    Task<CopiarVersionResult> CopiarVersionActualAsync(string cotizacionId, string? comentario = null, int? userId = null);
    
    // Copiar versión específica (desde sub-listado de versiones)
    Task<CopiarVersionResult> CopiarVersionEspecificaAsync(CopiarVersionRequest request, int? userId = null);
    
    // Duplicar cotización (crear nueva independiente)
    Task<DuplicarCotizacionResult> DuplicarCotizacionAsync(DuplicarCotizacionRequest request, int? userId = null);
    
    // Actualizar cotización existente (solo en estado Borrador)
    Task<ActualizarCotizacionResult> ActualizarCotizacionAsync(ActualizarCotizacionRequest request, int? userId = null);
    
    // Debugging: Obtener información detallada de estado para diagnóstico
    Task<CotizacionDebugInfoDto> GetCotizacionDebugInfoAsync(string cotizacionId);
    
    // Obtener detalle de la versión actual de una cotización específica
    Task<CotizacionVersionDetalleDto?> GetCotizacionCurrentVersionDetailAsync(string cotizacionId);
    
    // Obtener conteo de cotizaciones agrupadas por estado
    Task<Dictionary<char, int>> GetCotizacionesCountByEstadoAsync();
    
    // Cambios de estado con validación y registro de historial
    Task<CambiarEstadoResult> CambiarEstadoCotizacionAsync(string cotizacionId, char estadoEsperado, char nuevoEstado, string comentario, int? userId = null);
    Task<CambiarEstadoResult> CambiarEstadoCotizacionBasicoAsync(string cotizacionId, char nuevoEstado, string comentario, string? comentarioAdicional = null, int? userId = null);
    Task<CambiarEstadoResult> MarcarEnvioERPAsync(string cotizacionId, int? userId = null);
    
    // Obtener detalle completo de archivo de cotización
    Task<ArchivoCotizacionDetalleDto?> GetArchivoCotizacionDetalleAsync(string cotizacionId);
}
