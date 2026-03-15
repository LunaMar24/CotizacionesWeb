namespace CotizacionesWeb.Domain.Entities;

public class ArchivoCotizacion
{
    public int ArchivoId { get; set; }  // FASE 3: Llave primaria específica (sin BaseEntity según modelo)
    public string CotizacionId { get; set; } = string.Empty;
    public int? VersionArchivada { get; set; }
    public DateTime FechaArchivado { get; set; }
    public int? UsuarioArchiva { get; set; }
    public DateTime? FechaReactivacion { get; set; }
    public int? UsuarioReactiva { get; set; }
    public char TipoArchivo { get; set; }
    public string? Comentario { get; set; }
    public string MotivoArchivado { get; set; } = string.Empty;
    
    public Cotizacion Cotizacion { get; set; } = null!;
}
