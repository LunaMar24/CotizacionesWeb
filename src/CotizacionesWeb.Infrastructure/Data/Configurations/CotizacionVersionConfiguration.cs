using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class CotizacionVersionConfiguration : IEntityTypeConfiguration<CotizacionVersion>
{
    public void Configure(EntityTypeBuilder<CotizacionVersion> builder)
    {
        builder.ToTable("CotizacionVersion");
        
        // VersionId como llave primaria
        builder.HasKey(cv => cv.VersionId);
        
        builder.Property(cv => cv.VersionId)
               .ValueGeneratedOnAdd();
        
        // Ignorar Id de BaseEntity ya que usamos VersionId
        builder.Ignore(cv => cv.Id);
        
        builder.Property(cv => cv.CotizacionId)
            .HasMaxLength(30)
            .IsRequired();
        
        builder.Property(cv => cv.NumeroVersion)
            .HasColumnType("decimal(3,1)")
            .IsRequired();
        
        // CORREGIDO: Fechas como datetime (no datetime2)
        builder.Property(cv => cv.FechaVersion)
            .HasColumnType("datetime")
            .IsRequired();
        
        builder.Property(cv => cv.CreatedAt)
            .HasColumnType("datetime")
            .IsRequired();
        
        builder.Property(cv => cv.ModifiedAt)
            .HasColumnType("datetime");
        
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
            .IsRequired();
        
        builder.Property(cv => cv.Notas)
            .HasMaxLength(2000);
        
        // AGREGADO: Foreign Keys para auditoría
        builder.Property(cv => cv.CreatedBy)
            .IsRequired();
        
        builder.Property(cv => cv.ModifiedBy);
        
        // Foreign Keys para auditoría
        builder.HasOne<Usuario>()
               .WithMany()
               .HasForeignKey(cv => cv.CreatedBy)
               .HasPrincipalKey(u => u.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_CotizacionVersion_Usuarios_CreatedBy");
               
        builder.HasOne<Usuario>()
               .WithMany()
               .HasForeignKey(cv => cv.ModifiedBy)
               .HasPrincipalKey(u => u.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_CotizacionVersion_Usuarios_ModifiedBy");
        
        builder.HasOne(cv => cv.Cotizacion)
            .WithMany(c => c.Versiones)
            .HasForeignKey(cv => cv.CotizacionId)
            .HasPrincipalKey(c => c.CotizacionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(cv => new { cv.CotizacionId, cv.NumeroVersion })
            .IsUnique();
    }
}