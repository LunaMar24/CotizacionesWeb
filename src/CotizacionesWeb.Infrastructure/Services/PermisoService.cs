using CotizacionesWeb.Application.Permisos;
using CotizacionesWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CotizacionesWeb.Infrastructure.Services;

public class PermisoService : IPermisoService
{
    private readonly DbContextCotizaciones _db;

    public PermisoService(DbContextCotizaciones db)
    {
        _db = db;
    }

    public async Task<List<PermisoDto>> GetAllAsync()
    {
        var permisos = await _db.Permisos
            .OrderBy(p => p.Categoria)
            .ThenBy(p => p.Codigo)
            .ToListAsync();

        return permisos.Select(p => new PermisoDto(
            p.Id,
            p.Codigo,
            p.Categoria,
            p.Descripcion
        )).ToList();
    }

    public async Task<List<string>> GetUsuarioPermisosCodigosAsync(int usuarioId)
    {
        var permisos = await _db.UsuarioRoles
            .Where(ur => ur.UsuarioId == usuarioId)
            .Include(ur => ur.Rol)
                .ThenInclude(r => r.PermisosRoles)
                    .ThenInclude(pr => pr.Permiso)
            .SelectMany(ur => ur.Rol.PermisosRoles.Select(pr => pr.Permiso.Codigo))
            .Distinct()
            .ToListAsync();

        return permisos;
    }

    public async Task<bool> UsuarioTienePermisoAsync(int usuarioId, string codigoPermiso)
    {
        var tienePermiso = await _db.UsuarioRoles
            .Where(ur => ur.UsuarioId == usuarioId)
            .Include(ur => ur.Rol)
                .ThenInclude(r => r.PermisosRoles)
                    .ThenInclude(pr => pr.Permiso)
            .AnyAsync(ur => ur.Rol.PermisosRoles.Any(pr => pr.Permiso.Codigo == codigoPermiso));

        return tienePermiso;
    }
}
