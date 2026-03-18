using System.Globalization;

namespace CotizacionesWeb.UI.Helpers;

/// <summary>
/// Helper para formateo de números y monedas con punto decimal consistente
/// </summary>
public static class FormatHelper
{
    /// <summary>
    /// Diccionario de símbolos de moneda
    /// </summary>
    private static readonly Dictionary<string, string> CurrencySymbols = new()
    {
        { "CRC", "¢" },     // Colón costarricense
        { "USD", "$" },     // Dólar estadounidense 
        { "DOL", "$" },     // Dólar (alias)
        { "EUR", "€" },     // Euro
        { "MXN", "$" },     // Peso mexicano
        { "CAD", "$" },     // Dólar canadiense
        { "GBP", "£" },     // Libra esterlina
        { "JPY", "¥" },     // Yen japonés
        { "CNY", "¥" },     // Yuan chino
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
            return "?"; // Default a colón costarricense

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
}