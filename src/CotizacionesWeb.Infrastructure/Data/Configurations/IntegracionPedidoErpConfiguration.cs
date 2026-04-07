using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class IntegracionPedidoErpConfiguration : IEntityTypeConfiguration<IntegracionPedidoErp>
{
    public void Configure(EntityTypeBuilder<IntegracionPedidoErp> builder)
    {
        builder.ToTable("IntegracionPedidoErp");
        
        // IntegracionId como llave primaria
        builder.HasKey(i => i.IntegracionId);
        
        // Configuración de campos
        builder.Property(i => i.IntegracionId)
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        builder.Property(i => i.CotizacionId)
            .HasMaxLength(30)
            .IsRequired();
        
        builder.Property(i => i.VersionId)
            .IsRequired();
        
        builder.Property(i => i.LoteId)
            .IsRequired();
        
        builder.Property(i => i.Estado)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(i => i.PedidoErp)
            .HasMaxLength(50);
        
        builder.Property(i => i.Intentos)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(i => i.MensajeError)
            .HasMaxLength(1000);
        
        builder.Property(i => i.FechaCreacion)
            .HasColumnType("datetime")
            .IsRequired();
        
        builder.Property(i => i.FechaProcesado)
            .HasColumnType("datetime");
        
        // Relaciones con Cotizacion y CotizacionVersion
        builder.HasOne(i => i.Cotizacion)
            .WithMany()
            .HasForeignKey(i => i.CotizacionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_IntegracionPedidoErp_Cotizacion");
        
        builder.HasOne(i => i.Version)
            .WithMany()
            .HasForeignKey(i => i.VersionId)
            .HasPrincipalKey(v => v.VersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_IntegracionPedidoErp_CotizacionVersion");
        
        // Índices para optimización de consultas
        builder.HasIndex(i => i.Estado)
            .HasDatabaseName("IX_IntegracionPedidoErp_Estado");
        
        builder.HasIndex(i => new { i.CotizacionId, i.VersionId })
            .HasDatabaseName("IX_IntegracionPedidoErp_CotizacionId_VersionId");
        
        builder.HasIndex(i => i.LoteId)
            .HasDatabaseName("IX_IntegracionPedidoErp_LoteId");
    }
}