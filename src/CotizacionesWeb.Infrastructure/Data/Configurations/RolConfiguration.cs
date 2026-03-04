using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.HasKey(r => r.Id);
        
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
        
        builder.Property(r => r.CreatedBy)
               .HasMaxLength(100);
        
        builder.Property(r => r.ModifiedBy)
               .HasMaxLength(100);
    }
}
