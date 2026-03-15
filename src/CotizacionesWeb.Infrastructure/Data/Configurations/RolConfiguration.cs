using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        // FASE 2: Rol tiene tanto Id (BaseEntity) como RolId (específico)
        // Configurar RolId como llave primaria principal
        builder.HasKey(r => r.RolId);
        
        builder.Property(r => r.RolId)
               .ValueGeneratedOnAdd();  // Configurar como IDENTITY
        
        // El Id de BaseEntity se mantiene para futuras migraciones
        builder.Ignore(r => r.Id);
        
        builder.Property(r => r.Nombre)
               .HasMaxLength(100)
               .IsRequired();
        
        builder.HasIndex(r => r.Nombre)
               .IsUnique();
        
        builder.Property(r => r.Descripcion)
               .HasMaxLength(500);
        
        builder.Property(r => r.Activo)
               .IsRequired()
               .HasDefaultValue(true);
        
        // CORREGIDO: Fechas como datetime (no datetime2)
        builder.Property(r => r.CreatedAt)
               .HasColumnType("datetime")
               .IsRequired();
        
        builder.Property(r => r.ModifiedAt)
               .HasColumnType("datetime");
        
        // Auditoría con IDs de usuario
        builder.Property(r => r.CreatedBy)
               .IsRequired();
        
        builder.Property(r => r.ModifiedBy);
    }
}
