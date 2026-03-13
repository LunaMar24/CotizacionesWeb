using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class InteresadoConfiguration : IEntityTypeConfiguration<Interesado>
{
    public void Configure(EntityTypeBuilder<Interesado> builder)
    {
        builder.ToTable("Interesado");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.Id)
            .HasColumnName("InteresadoId");
        
        builder.Property(i => i.HubspotObjectId)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(i => i.HubspotObjectType)
            .HasMaxLength(30)
            .IsRequired();
        
        builder.Property(i => i.TipoInteresado)
            .HasMaxLength(1)
            .IsRequired();
        
        builder.Property(i => i.Activo)
            .IsRequired();
        
        builder.Property(i => i.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(i => i.ModifiedBy)
            .HasMaxLength(100);
        
        builder.HasIndex(i => i.HubspotObjectId);
    }
}
