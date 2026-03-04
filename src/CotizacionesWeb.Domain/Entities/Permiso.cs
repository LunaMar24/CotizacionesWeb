using CotizacionesWeb.Domain.Common;

namespace CotizacionesWeb.Domain.Entities;

public class Permiso : BaseEntity
{
    public string Descripcion { get; set; } = string.Empty;
    public ICollection<PermisoRol> PermisosRoles { get; set; } = new List<PermisoRol>();
}
