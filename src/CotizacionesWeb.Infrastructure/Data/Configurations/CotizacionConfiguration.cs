using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class CotizacionConfiguration : IEntityTypeConfiguration<Cotizacion>
{
    public void Configure(EntityTypeBuilder<Cotizacion> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Numero).HasMaxLength(50).IsRequired();
        builder.HasIndex(c => c.Numero).IsUnique();
        builder.Property(c => c.Cliente).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Total).HasColumnType("decimal(18,2)");
        builder.Property(c => c.CreatedBy).HasMaxLength(100);
        builder.Property(c => c.ModifiedBy).HasMaxLength(100);
    }
}
