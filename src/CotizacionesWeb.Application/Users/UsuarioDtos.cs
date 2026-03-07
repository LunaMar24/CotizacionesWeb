namespace CotizacionesWeb.Application.Users;

public record UsuarioDto(
    int Id,
    string Nombre,
    string Email,
    bool Activo,
    int IntentosFallidos,
    DateTime? UltimoAcceso,
    DateTime CreatedAt,
    List<RolDto> Roles
);

public record RolDto(
    int Id,
    string Nombre,
    string Descripcion
);

public record CreateUsuarioRequest(
    string Nombre,
    string Email,
    string Password,
    bool Activo,
    List<int> RolesIds
);

public record UpdateUsuarioRequest(
    string Nombre,
    string Email,
    bool Activo
);
