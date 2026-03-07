namespace CotizacionesWeb.Application.Roles;

public interface IRolService
{
    Task<List<RolDto>> GetAllActiveAsync();
    Task<List<RolDto>> GetAllAsync();
}

public record RolDto(
    int Id,
    string Nombre,
    string Descripcion,
    bool Activo
);
