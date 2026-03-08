using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Codigo)
               .HasMaxLength(30)
               .IsRequired();
        
        builder.HasIndex(p => p.Codigo)
               .IsUnique();
        
        builder.HasIndex(p => p.Codigo);
        
        builder.Property(p => p.Categoria)
               .HasMaxLength(50)
               .IsRequired();
        
        builder.Property(p => p.Descripcion)
               .HasMaxLength(200)
               .IsRequired();
        
        builder.Property(p => p.CreatedBy)
               .HasMaxLength(100);
        
        builder.Property(p => p.ModifiedBy)
               .HasMaxLength(100);
    }
}

