namespace CotizacionesWeb.Application.Documents;

/// <summary>
/// DTO para datos de cotización necesarios para generar documento Word
/// </summary>
public record CotizacionDocumentDto(
    // Información básica de la cotización
    string CotizacionId,
    string Version,
    DateTime FechaCotizacion,
    int ano,

    // Información del cliente
    string Cliente,
    string Empresa,
    string Email,
    
    // Información financiera
    string Moneda,
    string TipoCambio,
    decimal SubTotal,
    decimal ImpuestoTotal,
    decimal Total,
    
    // Información de parámetros del sistema
    string Vigencia,
    string CondicionesPago,
    string NotasComerciales,    
    string TituloDetalle,
    string TituloResumen,

    // Líneas de detalle
    List<DetalleDocumentDto> Detalles
);

/// <summary>
/// DTO para línea de detalle de cotización en documento
/// </summary>
public record DetalleDocumentDto(
    string Producto,
    string Descripcion,
    decimal Cantidad,
    decimal PrecioUnitario,
    decimal SubtotalLinea,
    decimal PorcentajeImpuesto,
    decimal TotalLinea
);

/// <summary>
/// Request para generar documento de cotización
/// </summary>
public record GenerarDocumentoCotizacionRequest(
    string CotizacionId,
    int? VersionId = null // Si es null, usa la versión actual
);

/// <summary>
/// Resultado de generación de documento
/// </summary>
public record GenerarDocumentoResult(
    bool Success,
    string? ErrorMessage,
    byte[]? DocumentContent,
    string? FileName
);

/// <summary>
/// Configuración de placeholders para el documento
/// </summary>
public record PlaceholderConfig(
    Dictionary<string, string> PlaceholdersSimples,
    string PlaceholderDetalle = "{{DETALLE_COTIZACION}}"
);