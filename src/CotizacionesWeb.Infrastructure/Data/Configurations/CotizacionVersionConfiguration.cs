using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class CotizacionVersionConfiguration : IEntityTypeConfiguration<CotizacionVersion>
{
    public void Configure(EntityTypeBuilder<CotizacionVersion> builder)
    {
        builder.ToTable("CotizacionVersion");
        
        builder.HasKey(cv => cv.Id);
        
        builder.Property(cv => cv.Id)
            .HasColumnName("VersionId");
        
        builder.Property(cv => cv.CotizacionId)
            .HasMaxLength(30)
            .IsRequired();
        
        builder.Property(cv => cv.NumeroVersion)
            .HasColumnType("decimal(3,1)")
            .IsRequired();
        
        builder.Property(cv => cv.FechaVersion)
            .IsRequired();
        
        builder.Property(cv => cv.NombreInteresado)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(cv => cv.EmailInteresado)
            .HasMaxLength(150)
            .IsRequired();
        
        builder.Property(cv => cv.EmpresaInteresado)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(cv => cv.SubTotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(cv => cv.Impuesto)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(cv => cv.Descuento)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(cv => cv.Total)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(cv => cv.Moneda)
            .HasMaxLength(3)
            .IsRequired();
        
        builder.Property(cv => cv.TipoCambio)
            .HasColumnType("decimal(18,2)");
        
        builder.Property(cv => cv.VersionActual)
            .IsRequired(); // Cambiado de char(1) a int
        
        builder.Property(cv => cv.Notas)
            .HasMaxLength(2000);
        
        builder.Property(cv => cv.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(cv => cv.ModifiedBy)
            .HasMaxLength(100);
        
        builder.HasOne(cv => cv.Cotizacion)
            .WithMany(c => c.Versiones)
            .HasForeignKey(cv => cv.CotizacionId)
            .HasPrincipalKey(c => c.CotizacionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(cv => new { cv.CotizacionId, cv.NumeroVersion })
            .IsUnique();
    }
}
