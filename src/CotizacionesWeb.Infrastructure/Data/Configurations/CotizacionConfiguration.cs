using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class CotizacionConfiguration : IEntityTypeConfiguration<Cotizacion>
{
    public void Configure(EntityTypeBuilder<Cotizacion> builder)
    {
        builder.ToTable("Cotizacion");
        
        // CotizacionId VARCHAR como llave primaria
        builder.HasKey(c => c.CotizacionId);
        
        // Ignorar Id de BaseEntity ya que usamos CotizacionId
        builder.Ignore(c => c.Id);
        
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
        
        // CORREGIDO: Fechas como datetime (no datetime2)
        builder.Property(c => c.FechaEnvio)
            .HasColumnType("datetime");
        
        builder.Property(c => c.FechaAceptacion)
            .HasColumnType("datetime");
        
        builder.Property(c => c.FechaRechazo)
            .HasColumnType("datetime");
        
        builder.Property(c => c.FechaEnvioERP)
            .HasColumnType("datetime");
        
        builder.Property(c => c.CreatedAt)
            .HasColumnType("datetime")
            .IsRequired();
        
        builder.Property(c => c.ModifiedAt)
            .HasColumnType("datetime");
        
        builder.Property(c => c.EnviadoERP)
            .HasMaxLength(1)
            .IsRequired()
            .HasDefaultValue('N');
        
        // AGREGADO: Foreign Keys para auditoría
        builder.Property(c => c.CreatedBy)
            .IsRequired();
        
        builder.Property(c => c.ModifiedBy);
        
        // Foreign Keys para auditoría
        builder.HasOne<Usuario>()
               .WithMany()
               .HasForeignKey(c => c.CreatedBy)
               .HasPrincipalKey(u => u.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_Cotizacion_Usuarios_CreatedBy");
               
        builder.HasOne<Usuario>()
               .WithMany()
               .HasForeignKey(c => c.ModifiedBy)
               .HasPrincipalKey(u => u.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_Cotizacion_Usuarios_ModifiedBy");
        
        // Check constraint para EnviadoERP
        builder.HasCheckConstraint("CK_Cotizacion_EnviadoERP", "[EnviadoERP] IN ('S', 'N')");
        
        builder.HasOne(c => c.Interesado)
            .WithMany(i => i.Cotizaciones)
            .HasForeignKey(c => c.InteresadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}