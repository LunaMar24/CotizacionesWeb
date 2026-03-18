namespace CotizacionesWeb.Application.Configuracion;

/// <summary>
/// DTO para mostrar parámetros del sistema en la UI
/// </summary>
public record ParametroDto(
    int ParametroId,
    string Codigo,
    string Descripcion,
    string Valor,
    string TipoValor, // Para display: "Texto", "Decimal", etc.
    string Categoria,
    bool EsModificable,
    string? ValorPorDefecto,
    string? Notas
);

/// <summary>
/// DTO para actualizar un parámetro
/// </summary>
public record ActualizarParametroRequest(
    int ParametroId,
    string Codigo,
    string Valor
);

/// <summary>
/// DTO para categorías con sus parámetros
/// </summary>
public record CategoriaParametrosDto(
    string Categoria,
    string Descripcion,
    string IconoClass,
    List<ParametroDto> Parametros
);

/// <summary>
/// DTO para el resultado de validación de un parámetro
/// </summary>
public record ValidacionParametroResult(
    bool EsValido,
    string? MensajeError,
    object? ValorConvertido
);