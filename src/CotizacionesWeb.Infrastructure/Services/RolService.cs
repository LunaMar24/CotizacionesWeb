using CotizacionesWeb.Application.Roles;
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
            .Where(r => r.Activo)
            .OrderBy(r => r.Nombre)
            .ToListAsync();

        return roles.Select(r => new RolDto(r.Id, r.Nombre, r.Descripcion, r.Activo)).ToList();
    }

    public async Task<List<RolDto>> GetAllAsync()
    {
        var roles = await _db.Roles
            .OrderBy(r => r.Nombre)
            .ToListAsync();

        return roles.Select(r => new RolDto(r.Id, r.Nombre, r.Descripcion, r.Activo)).ToList();
    }
}
