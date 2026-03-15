using System.ComponentModel.DataAnnotations;

namespace CotizacionesWeb.UI.Models;

public class CotizacionViewModel
{
    public int Id { get; set; }
    public string CotizacionId { get; set; } = string.Empty;
    public int? InteresadoId { get; set; }
    public string NombreInteresado { get; set; } = string.Empty;
    public string EmpresaInteresado { get; set; } = string.Empty;
    public char EstadoActual { get; set; }
    public string EstadoActualTexto { get; set; } = string.Empty;
    public int VersionActual { get; set; }
    public decimal NumeroVersion { get; set; } // Número específico de la versión (ej: 1.0, 2.0)
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }
    public decimal MontoCotizacion { get; set; }
    public DateTime? FechaEnvio { get; set; }
    
    // Nuevos campos según lineamientos funcionales
    public DateTime? FechaAceptacion { get; set; }
    public DateTime? FechaRechazo { get; set; }
    public char EnviadoERP { get; set; } = 'N';
    public DateTime? FechaEnvioERP { get; set; }
}

public class CotizacionFiltrosViewModel
{
    public string? Busqueda { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }

    // Filtros específicos para monto
    public decimal? MontoDesde { get; set; }
    public decimal? MontoHasta { get; set; }

    // Filtro específico para versión
    public decimal? Version { get; set; }

    // Estados seleccionados para filtrar (según nuevos lineamientos)
    public bool FiltroBorrador { get; set; }
    public bool FiltroPendienteAprobacion { get; set; }
    public bool FiltroAprobada { get; set; }
    public bool FiltroEnviada { get; set; }
    public bool FiltroAceptada { get; set; }
    public bool FiltroRechazada { get; set; }
    // Nota: Archivada NO se incluye en filtros según lineamientos
    // Nota: Cancelada se mantiene por retrocompatibilidad temporal
    public bool FiltroCancelada { get; set; }
}

public class CotizacionIndexViewModel
{
    public List<CotizacionViewModel> Cotizaciones { get; set; } = new();
    public CotizacionFiltrosViewModel Filtros { get; set; } = new();
}

public class CotizacionVersionViewModel
{
    public int VersionId { get; set; }
    public string CotizacionId { get; set; } = string.Empty;
    public decimal NumeroVersion { get; set; }
    public DateTime FechaVersion { get; set; }
    public string NombreInteresado { get; set; } = string.Empty;
    public string EmailInteresado { get; set; } = string.Empty;
    public string EmpresaInteresado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public int VersionActual { get; set; } // Cambiado de char a int
    public string? NombreUsuarioCreacion { get; set; }
}

public class HistorialViewModel
{
    public int HistorialId { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
    public string? NombreUsuario { get; set; }
    public string? Comentario { get; set; }
}
