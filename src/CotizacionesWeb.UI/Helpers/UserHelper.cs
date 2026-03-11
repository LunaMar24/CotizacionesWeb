using System.Security.Claims;

namespace CotizacionesWeb.UI.Helpers;

public static class UserHelper
{
    public static int GetUsuarioId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return 0;
    }

    public static string GetUsuarioNombre(this ClaimsPrincipal user)
    {
        return user.Identity?.Name ?? "Usuario";
    }

    public static string GetUsuarioEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value ?? "";
    }
}
