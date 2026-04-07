namespace CotizacionesWeb.Infrastructure.Integrations.Erp;

/// <summary>
/// Clase de configuración para integración con ERP
/// </summary>
public class ErpIntegrationConfig
{
    public string CondicionPago { get; set; } = string.Empty;
    public string Bodega { get; set; } = string.Empty;
    public string UsuarioERP { get; set; } = string.Empty;
    public string ActividadComercial { get; set; } = string.Empty;
    public string Cia { get; set; } = string.Empty;

    /// <summary>
    /// Valida que todos los parámetros requeridos estén configurados
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(CondicionPago) &&
               !string.IsNullOrWhiteSpace(Bodega) &&
               !string.IsNullOrWhiteSpace(UsuarioERP) &&
               !string.IsNullOrWhiteSpace(ActividadComercial) &&
               !string.IsNullOrWhiteSpace(Cia);
    }

    /// <summary>
    /// Obtiene una lista de los parámetros faltantes
    /// </summary>
    public List<string> GetMissingParameters()
    {
        var missing = new List<string>();
        
        if (string.IsNullOrWhiteSpace(CondicionPago))
            missing.Add("CondicionPago");
        if (string.IsNullOrWhiteSpace(Bodega))
            missing.Add("Bodega");
        if (string.IsNullOrWhiteSpace(UsuarioERP))
            missing.Add("UsuarioERP");
        if (string.IsNullOrWhiteSpace(ActividadComercial))
            missing.Add("ActividadComercial");
        if (string.IsNullOrWhiteSpace(Cia))
            missing.Add("Cia");
            
        return missing;
    }
}