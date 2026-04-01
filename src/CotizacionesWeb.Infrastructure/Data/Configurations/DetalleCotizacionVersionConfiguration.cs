using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class DetalleCotizacionVersionConfiguration : IEntityTypeConfiguration<DetalleCotizacionVersion>
{
    public void Configure(EntityTypeBuilder<DetalleCotizacionVersion> builder)
    {
        builder.ToTable("DetalleCotizacionVersion");
        
        // CORREGIDO: DetalleVersionId como llave primaria única (eliminar duplicados)
        builder.HasKey(d => d.DetalleVersionId);
        
        builder.Property(d => d.DetalleVersionId)
               .ValueGeneratedOnAdd();
        
        // Ignorar Id de BaseEntity ya que usamos DetalleVersionId
        builder.Ignore(d => d.Id);
        
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
        
        builder.Property(d => d.PorcentajeImpuesto)
               .HasColumnType("decimal(18,2)")
               .IsRequired()
               .HasDefaultValue(0);
        
        builder.Property(d => d.TotalLinea)
               .HasColumnType("decimal(18,2)")
               .IsRequired();
        
        // CORREGIDO: Fechas como datetime (no datetime2)
        builder.Property(d => d.CreatedAt)
               .HasColumnType("datetime")
               .IsRequired();
        
        builder.Property(d => d.ModifiedAt)
               .HasColumnType("datetime");
        
        // AGREGADO: Foreign Keys para auditoría
        builder.Property(d => d.CreatedBy)
               .IsRequired();
        
        builder.Property(d => d.ModifiedBy);
        
        // Foreign Keys para auditoría
        builder.HasOne<Usuario>()
               .WithMany()
               .HasForeignKey(d => d.CreatedBy)
               .HasPrincipalKey(u => u.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_DetalleCotizacionVersion_Usuarios_CreatedBy");
               
        builder.HasOne<Usuario>()
               .WithMany()
               .HasForeignKey(d => d.ModifiedBy)
               .HasPrincipalKey(u => u.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_DetalleCotizacionVersion_Usuarios_ModifiedBy");
        
        // Relación con CotizacionVersion
        builder.HasOne(d => d.Version)
               .WithMany(v => v.Detalles)
               .HasForeignKey(d => d.VersionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}