using CotizacionesWeb.Domain.Enums;

namespace CotizacionesWeb.Domain.Entities;

/// <summary>
/// Entidad para manejar parámetros de configuración del sistema
/// ACTUALIZADA: Expandida para soportar consecutivos inteligentes y categorización
/// LIMPIA: Removida propiedad obsoleta Tipo
/// </summary>
public class Parametros
{
    public int ParametroId { get; set; }
    
    /// <summary>
    /// Código único del parámetro (ej: MASCARA_CONSECUTIVO_COTIZACION)
    /// Para identificación única y programática
    /// </summary>
    public string Codigo { get; set; } = string.Empty;
    
    /// <summary>
    /// Descripción legible del parámetro
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;
    
    /// <summary>
    /// Valor actual del parámetro
    /// </summary>
    public string Valor { get; set; } = string.Empty;
    
    /// <summary>
    /// Tipo de valor: Decimal, Texto, Booleano, Fecha, Entero
    /// </summary>
    public TipoParametro TipoValor { get; set; } = TipoParametro.Texto;
    
    /// <summary>
    /// Categoría del parámetro para agrupación
    /// </summary>
    public string Categoria { get; set; } = string.Empty;
    
    /// <summary>
    /// Indica si el parámetro puede ser modificado por usuarios
    /// </summary>
    public bool EsModificable { get; set; } = true;
    
    /// <summary>
    /// Valor por defecto del parámetro
    /// </summary>
    public string? ValorPorDefecto { get; set; }
    
    /// <summary>
    /// Notas adicionales sobre el parámetro
    /// </summary>
    public string? Notas { get; set; }
    
    /// <summary>
    /// Indica si el parámetro contiene información sensitiva (contraseñas, tokens, etc.)
    /// Los valores sensitivos se enmascaran en la UI y requieren permiso especial para visualizarse
    /// </summary>
    public bool EsSensitivo { get; set; } = false;
}
