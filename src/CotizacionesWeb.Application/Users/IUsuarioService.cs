namespace CotizacionesWeb.Application.Users;

public interface IUsuarioService
{
    Task<List<UsuarioDto>> GetAllAsync();
    Task<UsuarioDto?> GetByIdAsync(int id);
    Task<UsuarioDto> CreateAsync(CreateUsuarioRequest request);
    Task<UsuarioDto> UpdateAsync(int id, UpdateUsuarioRequest request);
    Task DeleteAsync(int id);
    Task ResetPasswordAsync(int id, string newPassword);
    Task<List<RolDto>> GetUsuarioRolesAsync(int usuarioId);
    Task UpdateUsuarioRolesAsync(int usuarioId, List<int> rolesIds);
}
