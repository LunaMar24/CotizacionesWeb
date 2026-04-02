using System.Security.Claims;
using CotizacionesWeb.Application.Permisos;
using CotizacionesWeb.UI.Helpers;

namespace CotizacionesWeb.UI.Helpers;

/// <summary>
/// Helper centralizado para verificación de permisos de usuario
/// </summary>
public class PermisoHelper
{
    private readonly IPermisoService _permisoService;

    public PermisoHelper(IPermisoService permisoService)
    {
        _permisoService = permisoService;
    }

    /// <summary>
    /// Verifica si el usuario actual tiene un permiso específico
    /// </summary>
    /// <param name="user">Usuario actual del contexto HTTP</param>
    /// <param name="codigoPermiso">Código del permiso a verificar</param>
    /// <returns>True si tiene el permiso, False en caso contrario</returns>
    public async Task<bool> UsuarioTienePermisoAsync(ClaimsPrincipal user, string codigoPermiso)
    {
        if (!user.Identity?.IsAuthenticated == true)
        {
            return false;
        }

        var usuarioId = user.GetUsuarioId();
        if (usuarioId == 0)
        {
            return false;
        }

        return await _permisoService.UsuarioTienePermisoAsync(usuarioId, codigoPermiso);
    }

    /// <summary>
    /// Verifica si el usuario actual tiene al menos uno de los permisos especificados
    /// </summary>
    /// <param name="user">Usuario actual del contexto HTTP</param>
    /// <param name="codigosPermiso">Lista de códigos de permisos a verificar</param>
    /// <returns>True si tiene al menos uno de los permisos, False en caso contrario</returns>
    public async Task<bool> UsuarioTieneAlgunPermisoAsync(ClaimsPrincipal user, params string[] codigosPermiso)
    {
        if (!user.Identity?.IsAuthenticated == true)
        {
            return false;
        }

        var usuarioId = user.GetUsuarioId();
        if (usuarioId == 0)
        {
            return false;
        }

        foreach (var codigo in codigosPermiso)
        {
            var tienePermiso = await _permisoService.UsuarioTienePermisoAsync(usuarioId, codigo);
            if (tienePermiso)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Método estático para usar desde filtros y atributos
    /// </summary>
    /// <param name="permisoService">Servicio de permisos</param>
    /// <param name="user">Usuario actual</param>
    /// <param name="codigoPermiso">Código del permiso</param>
    /// <returns>True si tiene el permiso</returns>
    public static async Task<bool> VerificarPermisoAsync(IPermisoService permisoService, ClaimsPrincipal user, string codigoPermiso)
    {
        if (!user.Identity?.IsAuthenticated == true)
        {
            return false;
        }

        var usuarioId = user.GetUsuarioId();
        if (usuarioId == 0)
        {
            return false;
        }

        return await permisoService.UsuarioTienePermisoAsync(usuarioId, codigoPermiso);
    }

    /// <summary>
    /// Método estático para verificar múltiples permisos desde filtros y atributos
    /// </summary>
    /// <param name="permisoService">Servicio de permisos</param>
    /// <param name="user">Usuario actual</param>
    /// <param name="codigosPermiso">Códigos de permisos</param>
    /// <returns>True si tiene al menos uno de los permisos</returns>
    public static async Task<bool> VerificarAlgunPermisoAsync(IPermisoService permisoService, ClaimsPrincipal user, params string[] codigosPermiso)
    {
        if (!user.Identity?.IsAuthenticated == true)
        {
            return false;
        }

        var usuarioId = user.GetUsuarioId();
        if (usuarioId == 0)
        {
            return false;
        }

        foreach (var codigo in codigosPermiso)
        {
            var tienePermiso = await permisoService.UsuarioTienePermisoAsync(usuarioId, codigo);
            if (tienePermiso)
            {
                return true;
            }
        }

        return false;
    }
}