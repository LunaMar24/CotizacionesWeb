using CotizacionesWeb.Application.Users;
using CotizacionesWeb.Domain.Entities;
using CotizacionesWeb.Infrastructure.Data;
using CotizacionesWeb.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Infrastructure.Services;

public class UsuarioService : IUsuarioService
{
    private readonly DbContextCotizaciones _db;
    private readonly PasswordHasher _passwordHasher;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(
        DbContextCotizaciones db,
        PasswordHasher passwordHasher,
        ILogger<UsuarioService> logger)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<List<UsuarioDto>> GetAllAsync()
    {
        var usuarios = await _db.Usuarios
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .OrderBy(u => u.Nombre)
            .ToListAsync();

        return usuarios.Select(MapToDto).ToList();
    }

    public async Task<UsuarioDto?> GetByIdAsync(int id)
    {
        // CORREGIDO: Usar UsuarioId en lugar de Id
        var usuario = await _db.Usuarios
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.UsuarioId == id);

        return usuario == null ? null : MapToDto(usuario);
    }

    public async Task<UsuarioDto> CreateAsync(CreateUsuarioRequest request)
    {
        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Activo = request.Activo,
            // CORREGIDO: Agregar auditoría
            CreatedAt = DateTime.Now,
            CreatedBy = 1  // Usuario sistema para crear usuarios
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        if (request.RolesIds.Any())
        {
            foreach (var rolId in request.RolesIds)
            {
                _db.UsuarioRoles.Add(new UsuarioRol
                {
                    UsuarioId = usuario.UsuarioId,  // CORREGIDO: Usar UsuarioId
                    RolId = rolId
                });
            }
            await _db.SaveChangesAsync();
        }

        _logger.LogInformation("Usuario {Email} creado exitosamente por el servicio", usuario.Email);

        var usuarioCreado = await GetByIdAsync(usuario.UsuarioId);  // CORREGIDO: Usar UsuarioId
        return usuarioCreado!;
    }

    public async Task<UsuarioDto> UpdateAsync(int id, UpdateUsuarioRequest request)
    {
        // CORREGIDO: Buscar por UsuarioId
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == id);
        if (usuario == null)
            throw new InvalidOperationException($"Usuario con ID {id} no encontrado.");

        usuario.Nombre = request.Nombre;
        usuario.Email = request.Email;
        usuario.Activo = request.Activo;
        // CORREGIDO: Agregar auditoría
        usuario.ModifiedAt = DateTime.Now;
        usuario.ModifiedBy = 1;  // Usuario sistema

        await _db.SaveChangesAsync();

        _logger.LogInformation("Usuario {Email} modificado exitosamente", usuario.Email);

        var usuarioActualizado = await GetByIdAsync(id);
        return usuarioActualizado!;
    }

    public async Task DeleteAsync(int id)
    {
        // CORREGIDO: Buscar por UsuarioId
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == id);
        if (usuario == null)
            throw new InvalidOperationException($"Usuario con ID {id} no encontrado.");

        _db.Usuarios.Remove(usuario);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Usuario {Email} eliminado", usuario.Email);
    }

    public async Task ResetPasswordAsync(int id, string newPassword)
    {
        // CORREGIDO: Buscar por UsuarioId
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == id);
        if (usuario == null)
            throw new InvalidOperationException($"Usuario con ID {id} no encontrado.");

        usuario.PasswordHash = _passwordHasher.Hash(newPassword);
        usuario.IntentosFallidos = 0;
        // CORREGIDO: Agregar auditoría
        usuario.ModifiedAt = DateTime.Now;
        usuario.ModifiedBy = 1;  // Usuario sistema
        
        await _db.SaveChangesAsync();

        _logger.LogInformation("Contraseña reseteada para usuario {Email}", usuario.Email);
    }

    public async Task<List<RolDto>> GetUsuarioRolesAsync(int usuarioId)
    {
        // CORREGIDO: Buscar por UsuarioId
        var usuario = await _db.Usuarios
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);

        if (usuario == null)
            throw new InvalidOperationException($"Usuario con ID {usuarioId} no encontrado.");

        return usuario.UsuarioRoles
            .Select(ur => new RolDto(ur.Rol.RolId, ur.Rol.Nombre, ur.Rol.Descripcion))  // CORREGIDO: Usar RolId
            .ToList();
    }

    public async Task UpdateUsuarioRolesAsync(int usuarioId, List<int> rolesIds)
    {
        // CORREGIDO: Buscar por UsuarioId
        var usuario = await _db.Usuarios
            .Include(u => u.UsuarioRoles)
            .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);

        if (usuario == null)
            throw new InvalidOperationException($"Usuario con ID {usuarioId} no encontrado.");

        _db.UsuarioRoles.RemoveRange(usuario.UsuarioRoles);

        if (rolesIds.Any())
        {
            foreach (var rolId in rolesIds)
            {
                _db.UsuarioRoles.Add(new UsuarioRol
                {
                    UsuarioId = usuarioId,
                    RolId = rolId
                });
            }
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation("Roles actualizados para usuario {UsuarioId}", usuarioId);
    }

    private static UsuarioDto MapToDto(Usuario usuario)
    {
        return new UsuarioDto(
            usuario.UsuarioId,  // CORREGIDO: Usar UsuarioId
            usuario.Nombre,
            usuario.Email,
            usuario.Activo,
            usuario.IntentosFallidos,
            usuario.UltimoAcceso,
            usuario.CreatedAt,
            usuario.UsuarioRoles.Select(ur => new RolDto(
                ur.Rol.RolId,  // CORREGIDO: Usar RolId
                ur.Rol.Nombre,
                ur.Rol.Descripcion
            )).ToList()
        );
    }
}
