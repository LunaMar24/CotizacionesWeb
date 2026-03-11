using Microsoft.AspNetCore.Mvc;
using CotizacionesWeb.Application.Permisos;
using CotizacionesWeb.UI.Helpers;

namespace CotizacionesWeb.UI.ViewComponents;

public class PermisoCheckViewComponent : ViewComponent
{
    private readonly IPermisoService _permisoService;

    public PermisoCheckViewComponent(IPermisoService permisoService)
    {
        _permisoService = permisoService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string codigoPermiso)
    {
        var usuarioId = UserClaimsPrincipal.GetUsuarioId();
        if (usuarioId == 0)
        {
            return Content("false");
        }

        var tienePermiso = await _permisoService.UsuarioTienePermisoAsync(usuarioId, codigoPermiso);
        return Content(tienePermiso.ToString().ToLower());
    }
}
