using System.Security.Claims;
using CotizacionesWeb.Application.Permisos;
using CotizacionesWeb.UI.Helpers;

namespace CotizacionesWeb.UI.Services;

public interface IPermisoChecker
{
    Task<bool> TienePermisoAsync(string codigoPermiso);
    Task<List<string>> GetPermisosUsuarioActualAsync();
}

public class PermisoChecker : IPermisoChecker
{
    private readonly IPermisoService _permisoService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermisoChecker(IPermisoService permisoService, IHttpContextAccessor httpContextAccessor)
    {
        _permisoService = permisoService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> TienePermisoAsync(string codigoPermiso)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true)
            return false;

        var usuarioId = user.GetUsuarioId();
        if (usuarioId == 0)
            return false;

        return await _permisoService.UsuarioTienePermisoAsync(usuarioId, codigoPermiso);
    }

    public async Task<List<string>> GetPermisosUsuarioActualAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true)
            return new List<string>();

        var usuarioId = user.GetUsuarioId();
        if (usuarioId == 0)
            return new List<string>();

        return await _permisoService.GetUsuarioPermisosCodigosAsync(usuarioId);
    }
}
