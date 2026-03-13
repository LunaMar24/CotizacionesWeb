using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class ParametrosConfiguration : IEntityTypeConfiguration<Parametros>
{
    public void Configure(EntityTypeBuilder<Parametros> builder)
    {
        builder.ToTable("Parametros");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .HasColumnName("ParametroId");
        
        builder.Property(p => p.Descripcion)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(p => p.Valor)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(p => p.Tipo)
            .HasMaxLength(1)
            .IsRequired();
        
        builder.Property(p => p.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(p => p.ModifiedBy)
            .HasMaxLength(100);
    }
}
