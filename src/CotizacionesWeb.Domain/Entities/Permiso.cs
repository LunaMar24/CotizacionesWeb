namespace CotizacionesWeb.Domain.Entities;

public class Permiso
{
    public int PermisoId { get; set; }  // Llave primaria específica según modelo
    public string Codigo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public ICollection<PermisoRol> PermisosRoles { get; set; } = new List<PermisoRol>();
}

