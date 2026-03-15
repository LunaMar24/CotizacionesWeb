namespace CotizacionesWeb.Domain.Entities;

public class UsuarioRol
{
    // Llave compuesta: UsuarioId + RolId (sin BaseEntity según modelo)
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public int RolId { get; set; }
    public Rol Rol { get; set; } = null!;
}
