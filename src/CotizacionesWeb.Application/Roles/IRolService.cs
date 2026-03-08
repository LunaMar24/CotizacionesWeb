namespace CotizacionesWeb.Application.Roles;

public interface IRolService
{
    Task<List<RolDto>> GetAllActiveAsync();
    Task<List<RolDto>> GetAllAsync();
    Task<RolDto?> GetByIdAsync(int id);
    Task<RolDto> CreateAsync(CreateRolRequest request);
    Task<RolDto> UpdateAsync(int id, UpdateRolRequest request);
    Task DeleteAsync(int id);
    Task<List<PermisoDto>> GetRolPermisosAsync(int rolId);
    Task<List<PermisoDto>> GetAllPermisosAsync();
    Task UpdateRolPermisosAsync(int rolId, List<int> permisosIds);
}

public record RolDto(
    int Id,
    string Nombre,
    string Descripcion,
    bool Activo,
    int CantidadUsuarios,
    int CantidadPermisos,
    DateTime CreatedAt
);

public record RolDetalleDto(
    int Id,
    string Nombre,
    string Descripcion,
    bool Activo,
    List<PermisoDto> Permisos
);

public record PermisoDto(
    int Id,
    string Codigo,
    string Categoria,
    string Descripcion,
    bool Activo
);

public record CreateRolRequest(
    string Nombre,
    string Descripcion,
    bool Activo
);

public record UpdateRolRequest(
    string Nombre,
    string Descripcion,
    bool Activo
);

