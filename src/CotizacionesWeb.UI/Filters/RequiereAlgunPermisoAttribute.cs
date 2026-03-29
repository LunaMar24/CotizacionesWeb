using CotizacionesWeb.Application.Permisos;
using CotizacionesWeb.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CotizacionesWeb.UI.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequiereAlgunPermisoAttribute : TypeFilterAttribute
{
  public RequiereAlgunPermisoAttribute(params string[] codigosPermiso)
      : base(typeof(RequiereAlgunPermisoFilter))
  {
    Arguments = new object[] { codigosPermiso };
  }
}

public class RequiereAlgunPermisoFilter : IAsyncAuthorizationFilter
{
  private readonly string[] _codigosPermiso;
  private readonly IPermisoService _permisoService;

  public RequiereAlgunPermisoFilter(string[] codigosPermiso, IPermisoService permisoService)
  {
    _codigosPermiso = codigosPermiso;
    _permisoService = permisoService;
  }

  public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
  {
    var user = context.HttpContext.User;

    if (!user.Identity?.IsAuthenticated == true)
    {
      context.Result = new UnauthorizedResult();
      return;
    }

    var usuarioId = user.GetUsuarioId();
    if (usuarioId == 0)
    {
      context.Result = new UnauthorizedResult();
      return;
    }

    foreach (var permiso in _codigosPermiso)
    {
      if (await _permisoService.UsuarioTienePermisoAsync(usuarioId, permiso))
      {
        return;
      }
    }

    context.Result = new ForbidResult();
  }
}