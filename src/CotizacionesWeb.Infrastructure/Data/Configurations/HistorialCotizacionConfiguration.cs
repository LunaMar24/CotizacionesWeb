using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class HistorialCotizacionConfiguration : IEntityTypeConfiguration<HistorialCotizacion>
{
    public void Configure(EntityTypeBuilder<HistorialCotizacion> builder)
    {
        builder.ToTable("HistorialCotizacion");
        
        // HistorialId como llave primaria (sin BaseEntity según modelo)
        builder.HasKey(h => h.HistorialId);
        
        builder.Property(h => h.HistorialId)
               .ValueGeneratedOnAdd();
        
        builder.Property(h => h.VersionId)
            .IsRequired();
        
        builder.Property(h => h.TipoEvento)
            .HasMaxLength(50)
            .IsRequired();
        
        // CORREGIDO: Fechas como datetime (no datetime2)
        builder.Property(h => h.FechaEvento)
            .HasColumnType("datetime")
            .IsRequired();
        
        builder.Property(h => h.Comentario)
            .HasMaxLength(500);
        
        // AGREGADO: Foreign Key para UsuarioEvento
        builder.Property(h => h.UsuarioEvento);
        
        // Foreign Key para UsuarioEvento
        builder.HasOne<Usuario>()
               .WithMany()
               .HasForeignKey(h => h.UsuarioEvento)
               .HasPrincipalKey(u => u.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_HistorialCotizacion_Usuarios_UsuarioEvento");
        
        builder.HasOne(h => h.Version)
            .WithMany(v => v.Historiales)
            .HasForeignKey(h => h.VersionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(h => h.FechaEvento);
    }
}