namespace CotizacionesWeb.UI.Models;

public class DashboardViewModel
{
    public string UsuarioNombre { get; set; } = string.Empty;
    public List<EstadoCotizacionCard> EstadosCotizaciones { get; set; } = new();
}

public class EstadoCotizacionCard
{
    public char CodigoEstado { get; set; }
    public string NombreEstado { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public string ColorClase { get; set; } = "info";
    public string Icono { get; set; } = "fa-file-invoice";
}
