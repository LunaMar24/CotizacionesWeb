using CotizacionesWeb.Domain.Common;

namespace CotizacionesWeb.Domain.Entities;

public class ArchivoCotizacion : BaseEntity
{
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
