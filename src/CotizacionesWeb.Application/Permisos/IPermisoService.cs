namespace CotizacionesWeb.Application.Permisos;

public interface IPermisoService
{
    Task<List<PermisoDto>> GetAllAsync();
    Task<List<string>> GetUsuarioPermisosCodigosAsync(int usuarioId);
    Task<bool> UsuarioTienePermisoAsync(int usuarioId, string codigoPermiso);
}

public record PermisoDto(
    int Id,
    string Codigo,
    string Categoria,
    string Descripcion
);
