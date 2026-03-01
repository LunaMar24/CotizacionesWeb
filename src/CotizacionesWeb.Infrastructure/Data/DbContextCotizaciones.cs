using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CotizacionesWeb.Infrastructure.Data;

public class DbContextCotizaciones : DbContext
{
    public DbContextCotizaciones(DbContextOptions<DbContextCotizaciones> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
    public DbSet<Cotizacion> Cotizaciones => Set<Cotizacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContextCotizaciones).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        // TODO: Replace with current user from IHttpContextAccessor when UI is wired up
        const string system = "system";

        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = system;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedAt = now;
                entry.Entity.ModifiedBy = system;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
