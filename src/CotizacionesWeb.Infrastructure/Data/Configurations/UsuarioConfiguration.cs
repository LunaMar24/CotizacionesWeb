using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Nombre)
               .HasMaxLength(100)
               .IsRequired();
        
        builder.Property(u => u.Email)
               .HasMaxLength(150)
               .IsRequired();
        
        builder.HasIndex(u => u.Email)
               .IsUnique();
        
        builder.Property(u => u.PasswordHash)
               .HasMaxLength(512)
               .IsRequired();
        
        builder.Property(u => u.Activo)
               .IsRequired()
               .HasDefaultValue(true);
        
        builder.Property(u => u.IntentosFallidos)
               .IsRequired()
               .HasDefaultValue(0);
        
        builder.Property(u => u.UltimoAcceso);
        
        builder.Property(u => u.CreatedBy)
               .HasMaxLength(100);
        
        builder.Property(u => u.ModifiedBy)
               .HasMaxLength(100);
    }
}
