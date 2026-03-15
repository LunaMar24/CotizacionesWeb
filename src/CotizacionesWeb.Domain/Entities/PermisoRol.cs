namespace CotizacionesWeb.Domain.Entities;

public class PermisoRol
{
    // Llave compuesta: PermisoId + RolId (sin BaseEntity según modelo)
    public int PermisoId { get; set; }
    public Permiso Permiso { get; set; } = null!;
    
    public int RolId { get; set; }
    public Rol Rol { get; set; } = null!;
}
