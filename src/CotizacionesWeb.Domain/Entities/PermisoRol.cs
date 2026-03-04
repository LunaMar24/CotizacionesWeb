using CotizacionesWeb.Domain.Common;

namespace CotizacionesWeb.Domain.Entities;

public class PermisoRol : BaseEntity
{
    public int PermisoId { get; set; }
    public Permiso Permiso { get; set; } = null!;
    
    public int RolId { get; set; }
    public Rol Rol { get; set; } = null!;
}
