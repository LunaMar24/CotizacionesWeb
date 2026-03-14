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
    DateTime FechaCreacion,
    DateTime? FechaUltimaActualizacion,
    decimal MontoCotizacion,
    DateTime? FechaEnvio
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
    char VersionActual,
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
    DateTime? FechaHasta
);

public record CopiarVersionRequest(
    string CotizacionId,
    int VersionIdBase,
    bool EsVersionAntigua
);

public record DuplicarCotizacionRequest(
    string CotizacionIdBase
);

// Results
public record CopiarVersionResult(
    bool Success,
    string? ErrorMessage,
    int? NuevaVersionId,
    int? NumeroVersion
);

public record DuplicarCotizacionResult(
    bool Success,
    string? ErrorMessage,
    string? NuevaCotizacionId
);
