using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class PermisoRolConfiguration : IEntityTypeConfiguration<PermisoRol>
{
    public void Configure(EntityTypeBuilder<PermisoRol> builder)
    {
        // FASE 2: Llave compuesta PermisoId + RolId (sin BaseEntity según modelo)
        builder.HasKey(pr => new { pr.PermisoId, pr.RolId });
        
        // Relación: PermisoRol -> Permiso
        builder.HasOne(pr => pr.Permiso)
               .WithMany(p => p.PermisosRoles)
               .HasForeignKey(pr => pr.PermisoId)
               .OnDelete(DeleteBehavior.Cascade);
        
        // Relación: PermisoRol -> Rol
        builder.HasOne(pr => pr.Rol)
               .WithMany(r => r.PermisosRoles)
               .HasForeignKey(pr => pr.RolId)
               .OnDelete(DeleteBehavior.Cascade);
        
        // Sin campos de auditoría - no hereda de BaseEntity según modelo
    }
}
