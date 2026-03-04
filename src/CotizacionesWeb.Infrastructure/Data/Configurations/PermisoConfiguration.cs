using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Descripcion)
               .HasMaxLength(200)
               .IsRequired();
        
        builder.HasIndex(p => p.Descripcion)
               .IsUnique();
        
        builder.Property(p => p.CreatedBy)
               .HasMaxLength(100);
        
        builder.Property(p => p.ModifiedBy)
               .HasMaxLength(100);
    }
}
