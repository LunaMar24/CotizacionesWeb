using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRol>
{
    public void Configure(EntityTypeBuilder<UsuarioRol> builder)
    {
        // FASE 2: Llave compuesta UsuarioId + RolId (sin BaseEntity según modelo)
        builder.HasKey(ur => new { ur.UsuarioId, ur.RolId });
        
        // Relación: UsuarioRol -> Usuario
        builder.HasOne(ur => ur.Usuario)
               .WithMany(u => u.UsuarioRoles)
               .HasForeignKey(ur => ur.UsuarioId)
               .OnDelete(DeleteBehavior.Cascade);
        
        // Relación: UsuarioRol -> Rol
        builder.HasOne(ur => ur.Rol)
               .WithMany(r => r.UsuarioRoles)
               .HasForeignKey(ur => ur.RolId)
               .OnDelete(DeleteBehavior.Cascade);
        
        // Sin campos de auditoría - no hereda de BaseEntity según modelo
    }
}