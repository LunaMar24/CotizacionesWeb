namespace CotizacionesWeb.UI.Models;

public class DashboardViewModel
{
    public string UsuarioNombre { get; set; } = string.Empty;
    public EstadisticasGenerales Estadisticas { get; set; } = new();
    public List<ActividadReciente> ActividadesRecientes { get; set; } = new();
}

public class EstadisticasGenerales
{
    public int TotalUsuarios { get; set; }
    public int UsuariosActivos { get; set; }
    public int UsuariosInactivos { get; set; }
    public int TotalCotizaciones { get; set; }
    public int CotizacionesPendientes { get; set; }
    public int CotizacionesAprobadas { get; set; }
    public decimal MontoTotalCotizaciones { get; set; }
}

public class ActividadReciente
{
    public string Icono { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string TipoClase { get; set; } = "info";
}
