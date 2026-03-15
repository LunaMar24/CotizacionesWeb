namespace CotizacionesWeb.Domain.Entities;

public class HistorialCotizacion
{
    public int HistorialId { get; set; }  // FASE 3: Llave primaria específica (sin BaseEntity según modelo)
    public int VersionId { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
    public int? UsuarioEvento { get; set; }
    public string? Comentario { get; set; }
    
    public CotizacionVersion Version { get; set; } = null!;
}
