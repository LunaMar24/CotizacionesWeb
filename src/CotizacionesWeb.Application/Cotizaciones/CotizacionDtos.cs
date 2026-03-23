namespace CotizacionesWeb.Application.Cotizaciones;

// DTOs para lectura
public record CotizacionListDto(
    int Id,
    string CotizacionId,
    int? InteresadoId,
    string NombreInteresado,
    string EmpresaInteresado,
    char EstadoActual,
    int VersionActual,
    decimal NumeroVersion, // Número específico de la versión actual (ej: 1.0, 2.0, etc.)
    DateTime FechaCreacion,
    DateTime? FechaUltimaActualizacion,
    decimal MontoCotizacion,
    DateTime? FechaEnvio,
    // Información financiera
    string Moneda, // Moneda de la cotización
    // Nuevos campos según lineamientos funcionales
    DateTime? FechaAceptacion,
    DateTime? FechaRechazo,
    char EnviadoERP,
    DateTime? FechaEnvioERP
);

public record CotizacionVersionDto(
    int VersionId,
    string CotizacionId,
    decimal NumeroVersion,
    DateTime FechaVersion,
    string NombreInteresado,
    string EmailInteresado,
    string EmpresaInteresado,
    char TipoInteresado, // Nuevo campo para el tipo de interesado
    decimal SubTotal,
    decimal Impuesto,
    decimal Descuento,
    decimal Total,
    string Moneda, // OBSOLETO: Ahora está en Cotización, se mantiene por compatibilidad
    decimal? TipoCambio, // OBSOLETO: Ahora está en Cotización, se mantiene por compatibilidad
    int VersionActual, // Cambiado de char a int
    string? Notas
);

public record DetalleCotizacionDto(
    int DetalleVersionId,
    int VersionId,
    string ProductoId,
    string ProductoNombre,
    decimal Cantidad,
    decimal PrecioUnitario,
    decimal Descuento,
    decimal TotalLinea
);

public record HistorialCotizacionDto(
    int HistorialId,
    int VersionId,
    string TipoEvento,
    DateTime FechaEvento,
    int? UsuarioEvento,
    string? NombreUsuario,
    string? Comentario
);

public record CotizacionVersionDetalleDto(
    CotizacionVersionDto Version,
    List<DetalleCotizacionDto> Detalles
);

// Requests
public record GetCotizacionesListRequest(
    List<char>? Estados,
    string? Busqueda,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    decimal? MontoDesde,
    decimal? MontoHasta,
    decimal? Version,
    string? Moneda
);

public record CopiarVersionRequest(
    string CotizacionId,
    int VersionIdBase,
    bool EsVersionAntigua,
    string? Comentario = null
);

public record DuplicarCotizacionRequest(
    string CotizacionIdBase
);

public record ActualizarCotizacionRequest
{
    public string CotizacionId { get; set; } = string.Empty;
    public int VersionId { get; set; }
    public string NombreInteresado { get; set; } = string.Empty;
    public string EmailInteresado { get; set; } = string.Empty;
    public string EmpresaInteresado { get; set; } = string.Empty;
    public char TipoInteresado { get; set; } = 'P';
    public string? Moneda { get; set; } // Nueva propiedad para cambio de moneda
    public decimal? TipoCambio { get; set; } // Nueva propiedad para tipo de cambio
    public string Notas { get; set; } = string.Empty;
    public List<ActualizarDetalleRequest> Detalles { get; set; } = new();
}

public record ActualizarDetalleRequest
{
    public int DetalleVersionId { get; set; } // 0 = nuevo detalle, >0 = actualizar existente
    public string ProductoId { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public decimal TotalLinea { get; set; }
}

// Results
public record CopiarVersionResult(
    bool Success,
    string? ErrorMessage,
    int? NuevaVersionId,
    decimal? NumeroVersion  // CORREGIDO: Cambiar de int? a decimal?
);

public record DuplicarCotizacionResult(
    bool Success,
    string? ErrorMessage,
    string? NuevaCotizacionId
);

public record ActualizarCotizacionResult(
    bool Success,
    string? ErrorMessage
);

// DTO para debugging y diagnóstico
public record CotizacionDebugInfoDto(
    string CotizacionId,
    char EstadoActual,
    string EstadoTexto,
    int VersionActual,
    DateTime? FechaCreacion,
    DateTime? FechaModificacion,
    string? VersionInfo,
    bool ExisteEnBase
);
