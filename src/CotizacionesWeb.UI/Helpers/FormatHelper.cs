using System.Globalization;

namespace CotizacionesWeb.UI.Helpers;

/// <summary>
/// Helper para formateo de números y monedas con punto decimal consistente
/// </summary>
public static class FormatHelper
{
    /// <summary>
    /// Diccionario de símbolos de moneda con códigos Unicode correctos
    /// </summary>
    private static readonly Dictionary<string, string> CurrencySymbols = new()
    {
        { "CRC", "¢" },     // Colón costarricense (Alt+189)
        { "USD", "$" },     // Dólar estadounidense 
        { "DOL", "$" },     // Dólar (alias)
        { "EUR", "€" },     // Euro (Unicode: U+20AC)
        { "MXN", "$" },     // Peso mexicano
        { "CAD", "$" },     // Dólar canadiense
        { "GBP", "£" },     // Libra esterlina (Unicode: U+00A3)
        { "JPY", "¥" },     // Yen japonés (Unicode: U+00A5)
        { "CNY", "¥" },     // Yuan chino (Unicode: U+00A5)
    };

    /// <summary>
    /// Formatea una versión con "v" y punto decimal
    /// </summary>
    public static string FormatVersion(decimal version)
    {
        return $"v{version.ToString("0.0", CultureInfo.InvariantCulture)}";
    }

    /// <summary>
    /// Formatea moneda con punto decimal y símbolo según la moneda
    /// </summary>
    /// <param name="value">Valor monetario</param>
    /// <param name="currency">Código de moneda (CRC, USD, EUR, etc.)</param>
    /// <returns>Valor formateado con símbolo correcto</returns>
    public static string FormatCurrency(decimal value, string currency = "CRC")
    {
        var formattedNumber = value.ToString("N2", CultureInfo.InvariantCulture);
        var symbol = GetCurrencySymbol(currency);
        return $"{symbol}{formattedNumber}";
    }

    /// <summary>
    /// Formatea números con punto decimal
    /// </summary>
    public static string FormatNumber(decimal value, int decimals = 2)
    {
        var format = decimals == 0 ? "N0" : $"N{decimals}";
        return value.ToString(format, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Obtiene el símbolo de moneda según el código
    /// </summary>
    /// <param name="currency">Código de moneda</param>
    /// <returns>Símbolo de moneda</returns>
    public static string GetCurrencySymbol(string currency)
    {
        if (string.IsNullOrEmpty(currency))
            return "¢"; // Default a colón costarricense (Alt+189)

        var upperCurrency = currency.ToUpper().Trim();
        return CurrencySymbols.TryGetValue(upperCurrency, out string? symbol) ? symbol : upperCurrency;
    }

    /// <summary>
    /// Formatea solo el número con punto decimal (sin símbolo de moneda)
    /// </summary>
    public static string FormatAmount(decimal value)
    {
        return value.ToString("N2", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Obtiene el texto descriptivo del tipo de interesado
    /// </summary>
    /// <param name="tipoInteresado">Código del tipo de interesado (P, E, O)</param>
    /// <returns>Texto descriptivo del tipo</returns>
    public static string GetTipoInteresadoTexto(char tipoInteresado)
    {
        return tipoInteresado switch
        {
            'P' => "Persona",
            'E' => "Empresa", 
            'O' => "Otro",
            _ => "No especificado"
        };
    }

    /// <summary>
    /// Obtiene la lista de monedas disponibles con sus símbolos y nombres descriptivos
    /// </summary>
    /// <returns>Lista de tuplas con (código, símbolo, nombre)</returns>
    public static List<(string codigo, string simbolo, string nombre)> GetMonedasDisponibles()
    {
        return new List<(string codigo, string simbolo, string nombre)>
        {
            ("CRC", GetCurrencySymbol("CRC"), "Colón Costarricense"),
            ("USD", GetCurrencySymbol("USD"), "Dólar Estadounidense"),
            ("EUR", GetCurrencySymbol("EUR"), "Euro"),
            ("GBP", GetCurrencySymbol("GBP"), "Libra Esterlina"),
            ("JPY", GetCurrencySymbol("JPY"), "Yen Japonés"),
            ("MXN", GetCurrencySymbol("MXN"), "Peso Mexicano"),
            ("CAD", GetCurrencySymbol("CAD"), "Dólar Canadiense"),
            ("CNY", GetCurrencySymbol("CNY"), "Yuan Chino")
        };
    }

    /// <summary>
    /// Verifica si un código de moneda está soportado
    /// </summary>
    /// <param name="currency">Código de moneda a verificar</param>
    /// <returns>True si la moneda está soportada</returns>
    public static bool IsSupportedCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            return false;
            
        return CurrencySymbols.ContainsKey(currency.ToUpper().Trim());
    }
}