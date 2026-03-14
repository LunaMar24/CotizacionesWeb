using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class CotizacionConfiguration : IEntityTypeConfiguration<Cotizacion>
{
    public void Configure(EntityTypeBuilder<Cotizacion> builder)
    {
        builder.ToTable("Cotizacion");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.CotizacionId)
            .HasMaxLength(30)
            .IsRequired();
        
        builder.Property(c => c.EstadoActual)
            .HasMaxLength(1)
            .IsRequired();
        
        builder.Property(c => c.VersionActual)
            .IsRequired();
        
        builder.Property(c => c.MontoCotizacion)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(c => c.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(c => c.ModifiedBy)
            .HasMaxLength(100);
        
        builder.HasIndex(c => c.CotizacionId)
            .IsUnique();
        
        builder.HasOne(c => c.Interesado)
            .WithMany(i => i.Cotizaciones)
            .HasForeignKey(c => c.InteresadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
