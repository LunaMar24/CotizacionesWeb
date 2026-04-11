using System.ComponentModel.DataAnnotations;

namespace CotizacionesWeb.UI.Models;

// Clase para serialización JSON de monedas
public class MonedaDisponible
{
    public string Codigo { get; set; } = string.Empty;
    public string Simbolo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

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
    
    // Información financiera
    public string Moneda { get; set; } = "CRC"; // Moneda por defecto
    
    // Nuevos campos según lineamientos funcionales
    public DateTime? FechaAceptacion { get; set; }
    public DateTime? FechaRechazo { get; set; }
    public char EnviadoERP { get; set; } = 'N';
    public DateTime? FechaEnvioERP { get; set; }
    
    // Campos específicos para cotizaciones archivadas
    public string? UsuarioQueArchivo { get; set; }
    public DateTime? FechaArchivado { get; set; }
    
    // ?? Nuevos campos para controlar reactivación
    public DateTime? FechaReactivacion { get; set; }
    public string? UsuarioQueReactivo { get; set; }
    
    // TipoArchivo para controlar acciones según reglas de negocio
    public char? TipoArchivo { get; set; }
}

public class CotizacionFiltrosViewModel
{
    public string? Busqueda { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }

    // Fechas de archivado (específico para cotizaciones archivadas)
    public DateTime? FechaArchivadoDesde { get; set; }
    public DateTime? FechaArchivadoHasta { get; set; }

    // Filtros específicos para monto
    public decimal? MontoDesde { get; set; }
    public decimal? MontoHasta { get; set; }

    // Filtro específico para versión
    public decimal? Version { get; set; }

    // Filtro específico para moneda
    public string? Moneda { get; set; }

    // Filtro para usuario que archivó (específico para cotizaciones archivadas)
    public string? UsuarioArchivo { get; set; }

    // Búsqueda en detalle de productos
    public string? BusquedaProducto { get; set; }
    public string? BusquedaDescripcion { get; set; }

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

public class CotizacionDetalleViewModel
{
    // Información básica de la cotización
    public string CotizacionId { get; set; } = string.Empty;
    public char EstadoActual { get; set; }
    public string EstadoActualTexto { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }
    
    // Información de la versión actual
    public int VersionId { get; set; }
    public decimal NumeroVersion { get; set; }
    public DateTime FechaVersion { get; set; }
    
    // Información del cliente
    public string NombreInteresado { get; set; } = string.Empty;
    public string EmailInteresado { get; set; } = string.Empty;
    public string EmpresaInteresado { get; set; } = string.Empty;
    public char TipoInteresado { get; set; } = 'P'; // Nuevo campo para el tipo de interesado
    
    // Información financiera
    public decimal SubTotal { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public decimal? TipoCambio { get; set; }
    
    // Fechas importantes según el estado
    public DateTime? FechaEnvio { get; set; }
    public DateTime? FechaAceptacion { get; set; }
    public DateTime? FechaRechazo { get; set; }
    public char EnviadoERP { get; set; } = 'N';
    public DateTime? FechaEnvioERP { get; set; }
    
    // Notas
    public string? Notas { get; set; }
    
    // Líneas de detalle
    public List<DetalleCotizacionViewModel> Detalles { get; set; } = new();
}

public class DetalleCotizacionViewModel
{
    public int DetalleVersionId { get; set; }
    public string ProductoId { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public decimal PorcentajeImpuesto { get; set; } // Porcentaje de impuesto aplicado
    public decimal TotalLinea { get; set; }
}

public class CotizacionEditarViewModel
{
    // Información básica de la cotización (solo lectura)
    public string CotizacionId { get; set; } = string.Empty;
    public char EstadoActual { get; set; }
    public string EstadoActualTexto { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }
    
    // Información de la versión actual (solo lectura)
    public int VersionId { get; set; }
    public decimal NumeroVersion { get; set; }
    public DateTime FechaVersion { get; set; }
    
    // Información del interesado (EDITABLE)
    public int? InteresadoId { get; set; }
    public string NombreInteresado { get; set; } = string.Empty;
    public string EmailInteresado { get; set; } = string.Empty;
    public string EmpresaInteresado { get; set; } = string.Empty;
    public char TipoInteresado { get; set; } = 'P';
    
    // Información financiera (calculada automáticamente)
    public decimal SubTotal { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public decimal? TipoCambio { get; set; }
    
    // Fechas importantes (solo lectura)
    public DateTime? FechaEnvio { get; set; }
    public DateTime? FechaAceptacion { get; set; }
    public DateTime? FechaRechazo { get; set; }
    public char EnviadoERP { get; set; } = 'N';
    public DateTime? FechaEnvioERP { get; set; }
    
    // Notas (EDITABLE)
    public string? Notas { get; set; }
    
    // Líneas de detalle (EDITABLE)
    public List<DetalleEditarViewModel> Detalles { get; set; } = new();
    
    // Configuración de impuestos (desde parámetros del sistema)
    /// <summary>
    /// Indica si se deben usar los porcentajes de impuesto provenientes del ERP.
    /// Valores: "S" = Usar impuestos del ERP, "N" = Usar tasa fija del sistema
    /// </summary>
    public string UsarImpuestosErp { get; set; } = "S";
    
    /// <summary>
    /// Tasa de impuesto por defecto del sistema (ej: 13.0 para 13%)
    /// Se usa cuando UsarImpuestosErp = "N" o cuando el ERP no provee porcentaje
    /// </summary>
    public decimal TasaImpuesto { get; set; } = 13.0m;
    
    // Propiedades calculadas para reglas de negocio
    /// <summary>
    /// Indica si se puede cambiar la moneda de la cotización.
    /// Para nuevas cotizaciones: siempre puede cambiar si está en estado Borrador
    /// Para ediciones existentes: solo si está en Borrador Y no tiene líneas persistentes en BD
    /// </summary>
    public bool PuedeCambiarMoneda => 
        EstadoActual == 'B' && (
            EsNuevaCotizacion || // Nueva cotización: siempre puede cambiar
            (Detalles == null || !Detalles.Any(d => d.DetalleVersionId > 0)) // Edición: sin líneas persistentes
        );

    /// <summary>
    /// Indica si es una nueva cotización (creación en curso).
    /// Se usa para ajustar comportamiento específico de la pantalla de creación.
    /// </summary>
    public bool EsNuevaCotizacion { get; set; } = false;
}

public class DetalleEditarViewModel
{
    public int DetalleVersionId { get; set; }
    public string ProductoId { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public decimal PorcentajeImpuesto { get; set; } // Porcentaje de impuesto aplicado
    public decimal TotalLinea { get; set; }
}

public class InteresadoViewModel
{
    public int InteresadoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public char TipoInteresado { get; set; }
    public string TipoInteresadoTexto { get; set; } = string.Empty;
}

public class ArchivoCotizacionDetalleViewModel
{
    // Información del archivo
    public int ArchivoId { get; set; }
    public string CotizacionId { get; set; } = string.Empty;
    public int? VersionArchivada { get; set; }
    public DateTime FechaArchivado { get; set; }
    public int? UsuarioArchiva { get; set; }
    public string? NombreUsuarioArchiva { get; set; }
    public DateTime? FechaReactivacion { get; set; }
    public int? UsuarioReactiva { get; set; }
    public string? NombreUsuarioReactiva { get; set; }
    public char TipoArchivo { get; set; }
    public string TipoArchivoTexto { get; set; } = string.Empty;
    public string? Comentario { get; set; }
    public string MotivoArchivado { get; set; } = string.Empty;

    // Información de la cotización (del detalle base)
    public char EstadoActual { get; set; }
    public string EstadoActualTexto { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }
    
    // Información de la versión archivada
    public int VersionId { get; set; }
    public decimal NumeroVersion { get; set; }
    public DateTime FechaVersion { get; set; }
    
    // Información del cliente
    public string NombreInteresado { get; set; } = string.Empty;
    public string EmailInteresado { get; set; } = string.Empty;
    public string EmpresaInteresado { get; set; } = string.Empty;
    public char TipoInteresado { get; set; } = 'P';
    
    // Información financiera
    public decimal SubTotal { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public decimal? TipoCambio { get; set; }
    
    // Fechas importantes según el estado
    public DateTime? FechaEnvio { get; set; }
    public DateTime? FechaAceptacion { get; set; }
    public DateTime? FechaRechazo { get; set; }
    public char EnviadoERP { get; set; } = 'N';
    public DateTime? FechaEnvioERP { get; set; }
    
    // Notas de la versión
    public string? Notas { get; set; }
    
    // Líneas de detalle
    public List<DetalleCotizacionViewModel> Detalles { get; set; } = new();
}
