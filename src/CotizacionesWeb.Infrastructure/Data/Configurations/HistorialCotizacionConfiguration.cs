using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class HistorialCotizacionConfiguration : IEntityTypeConfiguration<HistorialCotizacion>
{
    public void Configure(EntityTypeBuilder<HistorialCotizacion> builder)
    {
        builder.ToTable("HistorialCotizacion");
        
        builder.HasKey(h => h.Id);
        
        builder.Property(h => h.Id)
            .HasColumnName("HistorialId");
        
        builder.Property(h => h.VersionId)
            .IsRequired();
        
        builder.Property(h => h.TipoEvento)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(h => h.FechaEvento)
            .IsRequired();
        
        builder.Property(h => h.Comentario)
            .HasMaxLength(500);
        
        builder.Property(h => h.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(h => h.ModifiedBy)
            .HasMaxLength(100);
        
        builder.HasOne(h => h.Version)
            .WithMany(v => v.Historiales)
            .HasForeignKey(h => h.VersionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(h => h.FechaEvento);
    }
}
