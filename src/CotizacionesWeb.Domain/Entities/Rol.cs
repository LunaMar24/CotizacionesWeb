using CotizacionesWeb.Domain.Common;

namespace CotizacionesWeb.Domain.Entities;

public class Rol : BaseEntity
{
    public int RolId { get; set; }  // Llave primaria específica según modelo
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    
    public ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
    public ICollection<PermisoRol> PermisosRoles { get; set; } = new List<PermisoRol>();
}
