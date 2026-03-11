using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CotizacionesWeb.Infrastructure.Data;

public class DbContextCotizaciones : DbContext
{
    public DbContextCotizaciones(DbContextOptions<DbContextCotizaciones> options)
        : base(options)
    {
    }

    // Tablas de Seguridad
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<PermisoRol> PermisosRoles => Set<PermisoRol>();
    
    // NOTA: Cotizaciones se agregará en una fase posterior
    // public DbSet<Cotizacion> Cotizaciones => Set<Cotizacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContextCotizaciones).Assembly);
    }
}
