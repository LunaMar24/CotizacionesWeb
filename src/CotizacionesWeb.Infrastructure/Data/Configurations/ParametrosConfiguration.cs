using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class ParametrosConfiguration : IEntityTypeConfiguration<Parametros>
{
    public void Configure(EntityTypeBuilder<Parametros> builder)
    {
        builder.ToTable("Parametros");
        
        // FASE 3: Parametros usa ParametroId (sin BaseEntity según modelo)
        builder.HasKey(p => p.ParametroId);
        
        builder.Property(p => p.ParametroId)
               .ValueGeneratedOnAdd();  // Configurar como IDENTITY
        
        builder.Property(p => p.Descripcion)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(p => p.Valor)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(p => p.Tipo)
            .HasMaxLength(1)
            .IsRequired();
        
        // Sin campos de auditoría - no hereda de BaseEntity según modelo
    }
}
