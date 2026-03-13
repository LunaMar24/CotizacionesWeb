using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class ArchivoCotizacionConfiguration : IEntityTypeConfiguration<ArchivoCotizacion>
{
    public void Configure(EntityTypeBuilder<ArchivoCotizacion> builder)
    {
        builder.ToTable("ArchivoCotizacion");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .HasColumnName("ArchivoId");
        
        builder.Property(a => a.CotizacionId)
            .HasMaxLength(30)
            .IsRequired();
        
        builder.Property(a => a.FechaArchivado)
            .IsRequired();
        
        builder.Property(a => a.TipoArchivo)
            .HasMaxLength(1)
            .IsRequired();
        
        builder.Property(a => a.MotivoArchivado)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(a => a.Comentario)
            .HasMaxLength(500);
        
        builder.Property(a => a.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(a => a.ModifiedBy)
            .HasMaxLength(100);
        
        builder.HasOne(a => a.Cotizacion)
            .WithMany(c => c.Archivos)
            .HasForeignKey(a => a.CotizacionId)
            .HasPrincipalKey(c => c.CotizacionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(a => a.FechaArchivado);
    }
}
