using CotizacionesWeb.Domain.Common;

namespace CotizacionesWeb.Domain.Entities;

public class HistorialCotizacion : BaseEntity
{
    public int VersionId { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
    public int? UsuarioEvento { get; set; }
    public string? Comentario { get; set; }
    
    public CotizacionVersion Version { get; set; } = null!;
}
