using System.ComponentModel.DataAnnotations;

namespace CotizacionesWeb.UI.Models;

public class RolCreateViewModel
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}

public class RolEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    public bool Activo { get; set; }
}

public class RolPermisosViewModel
{
    public int RolId { get; set; }
    public string RolNombre { get; set; } = string.Empty;
    public List<PermisoItemViewModel> PermisosDisponibles { get; set; } = new();
    public List<int> PermisosAsignados { get; set; } = new();
}

public class PermisoItemViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

