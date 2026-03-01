using CotizacionesWeb.Domain.Common;

namespace CotizacionesWeb.Domain.Entities;

public class UsuarioRol : BaseEntity
{
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public int RolId { get; set; }
    public Rol Rol { get; set; } = null!;
}
