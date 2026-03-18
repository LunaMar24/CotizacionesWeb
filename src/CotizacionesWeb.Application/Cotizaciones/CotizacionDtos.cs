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
    decimal SubTotal,
    decimal Impuesto,
    decimal Descuento,
    decimal Total,
    string Moneda,
    decimal? TipoCambio,
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
    decimal? Version
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
