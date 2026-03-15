using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        // FASE 2: Permiso usa PermisoId como llave primaria (sin BaseEntity)
        builder.HasKey(p => p.PermisoId);
        
        builder.Property(p => p.PermisoId)
               .ValueGeneratedOnAdd();  // Configurar como IDENTITY
        
        builder.Property(p => p.Codigo)
               .HasMaxLength(30)
               .IsRequired();
        
        builder.HasIndex(p => p.Codigo)
               .IsUnique();
        
        builder.Property(p => p.Categoria)
               .HasMaxLength(50)
               .IsRequired();
        
        builder.Property(p => p.Descripcion)
               .HasMaxLength(200)
               .IsRequired();
        
        // Sin campos de auditoría - no hereda de BaseEntity según modelo
    }
}

