using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CotizacionesWeb.Application.Permisos;
using CotizacionesWeb.UI.Helpers;

namespace CotizacionesWeb.UI.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequierePermisoAttribute : TypeFilterAttribute
{
    public RequierePermisoAttribute(string codigoPermiso) : base(typeof(PermisoFilter))
    {
        Arguments = new object[] { codigoPermiso };
    }
}

public class PermisoFilter : IAsyncAuthorizationFilter
{
    private readonly string _codigoPermiso;
    private readonly IPermisoService _permisoService;

    public PermisoFilter(string codigoPermiso, IPermisoService permisoService)
    {
        _codigoPermiso = codigoPermiso;
        _permisoService = permisoService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        
        // Utilizar PermisoHelper para centralizar la lógica de verificación
        var tienePermiso = await PermisoHelper.VerificarPermisoAsync(_permisoService, user, _codigoPermiso);
        
        if (!tienePermiso)
        {
            if (!user.Identity?.IsAuthenticated == true)
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
