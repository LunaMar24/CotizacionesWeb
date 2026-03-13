using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class DetalleCotizacionVersionConfiguration : IEntityTypeConfiguration<DetalleCotizacionVersion>
{
    public void Configure(EntityTypeBuilder<DetalleCotizacionVersion> builder)
    {
        builder.ToTable("DetalleCotizacionVersion");
        
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Id)
            .HasColumnName("DetalleVersionId");
        
        builder.Property(d => d.VersionId)
            .IsRequired();
        
        builder.Property(d => d.ProductoId)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(d => d.Cantidad)
            .HasColumnType("decimal(18,4)")
            .IsRequired();
        
        builder.Property(d => d.PrecioUnitario)
            .HasColumnType("decimal(18,4)")
            .IsRequired();
        
        builder.Property(d => d.Descuento)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(d => d.TotalLinea)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(d => d.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(d => d.ModifiedBy)
            .HasMaxLength(100);
        
        builder.HasOne(d => d.Version)
            .WithMany(v => v.Detalles)
            .HasForeignKey(d => d.VersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
