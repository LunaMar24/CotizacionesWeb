using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        // FASE 2: Usuario tiene tanto Id (BaseEntity) como UsuarioId (específico)
        // Configurar UsuarioId como llave primaria principal
        builder.HasKey(u => u.UsuarioId);
        
        builder.Property(u => u.UsuarioId)
               .ValueGeneratedOnAdd();  // Configurar como IDENTITY
        
        // El Id de BaseEntity se mantiene para futuras migraciones
        builder.Ignore(u => u.Id);
        
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
        
        builder.Property(u => u.UltimoAcceso)
               .HasColumnType("datetime");
        
        // CORREGIDO: Fechas como datetime (no datetime2)
        builder.Property(u => u.CreatedAt)
               .HasColumnType("datetime")
               .IsRequired();
        
        builder.Property(u => u.ModifiedAt)
               .HasColumnType("datetime");
        
        // Auditoría con IDs de usuario
        builder.Property(u => u.CreatedBy)
               .IsRequired();
        
        builder.Property(u => u.ModifiedBy);
    }
}
