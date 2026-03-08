using CotizacionesWeb.Application.Roles;
using CotizacionesWeb.Domain.Entities;
using CotizacionesWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CotizacionesWeb.Infrastructure.Services;

public class RolService : IRolService
{
    private readonly DbContextCotizaciones _db;

    public RolService(DbContextCotizaciones db)
    {
        _db = db;
    }

    public async Task<List<RolDto>> GetAllActiveAsync()
    {
        var roles = await _db.Roles
            .Include(r => r.UsuarioRoles)
            .Include(r => r.PermisosRoles)
            .Where(r => r.Activo)
            .OrderBy(r => r.Nombre)
            .ToListAsync();

        return roles.Select(r => new RolDto(
            r.Id, 
            r.Nombre, 
            r.Descripcion, 
            r.Activo,
            r.UsuarioRoles.Count,
            r.PermisosRoles.Count,
            r.CreatedAt
        )).ToList();
    }

    public async Task<List<RolDto>> GetAllAsync()
    {
        var roles = await _db.Roles
            .Include(r => r.UsuarioRoles)
            .Include(r => r.PermisosRoles)
            .OrderBy(r => r.Nombre)
            .ToListAsync();

        return roles.Select(r => new RolDto(
            r.Id, 
            r.Nombre, 
            r.Descripcion, 
            r.Activo,
            r.UsuarioRoles.Count,
            r.PermisosRoles.Count,
            r.CreatedAt
        )).ToList();
    }

    public async Task<RolDto?> GetByIdAsync(int id)
    {
        var rol = await _db.Roles
            .Include(r => r.UsuarioRoles)
            .Include(r => r.PermisosRoles)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rol == null) return null;

        return new RolDto(
            rol.Id, 
            rol.Nombre, 
            rol.Descripcion, 
            rol.Activo,
            rol.UsuarioRoles.Count,
            rol.PermisosRoles.Count,
            rol.CreatedAt
        );
    }

    public async Task<RolDto> CreateAsync(CreateRolRequest request)
    {
        var rol = new Rol
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = request.Activo,
            CreatedAt = DateTime.UtcNow
        };

        _db.Roles.Add(rol);
        await _db.SaveChangesAsync();

        return new RolDto(rol.Id, rol.Nombre, rol.Descripcion, rol.Activo, 0, 0, rol.CreatedAt);
    }

    public async Task<RolDto> UpdateAsync(int id, UpdateRolRequest request)
    {
        var rol = await _db.Roles.FindAsync(id);
        if (rol == null)
            throw new InvalidOperationException($"Rol con ID {id} no encontrado");

        rol.Nombre = request.Nombre;
        rol.Descripcion = request.Descripcion;
        rol.Activo = request.Activo;
        rol.ModifiedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        var cantUsuarios = await _db.UsuarioRoles.CountAsync(ur => ur.RolId == id);
        var cantPermisos = await _db.PermisosRoles.CountAsync(pr => pr.RolId == id);

        return new RolDto(rol.Id, rol.Nombre, rol.Descripcion, rol.Activo, cantUsuarios, cantPermisos, rol.CreatedAt);
    }

    public async Task DeleteAsync(int id)
    {
        var rol = await _db.Roles
            .Include(r => r.UsuarioRoles)
            .Include(r => r.PermisosRoles)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rol == null)
            throw new InvalidOperationException($"Rol con ID {id} no encontrado");

        if (rol.UsuarioRoles.Any())
            throw new InvalidOperationException("No se puede eliminar un rol que tiene usuarios asignados");

        _db.Roles.Remove(rol);
        await _db.SaveChangesAsync();
    }

    public async Task<List<PermisoDto>> GetRolPermisosAsync(int rolId)
    {
        var permisos = await _db.PermisosRoles
            .Where(pr => pr.RolId == rolId)
            .Include(pr => pr.Permiso)
            .Select(pr => pr.Permiso)
            .ToListAsync();

        return permisos.Select(p => new PermisoDto(p.Id, p.Codigo, p.Categoria, p.Descripcion, true)).ToList();
    }

    public async Task<List<PermisoDto>> GetAllPermisosAsync()
    {
        var permisos = await _db.Permisos
            .OrderBy(p => p.Categoria)
            .ThenBy(p => p.Codigo)
            .ToListAsync();

        return permisos.Select(p => new PermisoDto(p.Id, p.Codigo, p.Categoria, p.Descripcion, true)).ToList();
    }

    public async Task UpdateRolPermisosAsync(int rolId, List<int> permisosIds)
    {
        var rol = await _db.Roles
            .Include(r => r.PermisosRoles)
            .FirstOrDefaultAsync(r => r.Id == rolId);

        if (rol == null)
            throw new InvalidOperationException($"Rol con ID {rolId} no encontrado");

        // Eliminar permisos existentes
        _db.PermisosRoles.RemoveRange(rol.PermisosRoles);

        // Agregar nuevos permisos
        foreach (var permisoId in permisosIds)
        {
            rol.PermisosRoles.Add(new PermisoRol
            {
                RolId = rolId,
                PermisoId = permisoId,
                CreatedAt = DateTime.UtcNow
            });
        }

        rol.ModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}

