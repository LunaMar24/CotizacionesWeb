using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class NotificacionCotizacionConfiguration : IEntityTypeConfiguration<NotificacionCotizacion>
{
    public void Configure(EntityTypeBuilder<NotificacionCotizacion> builder)
    {
        builder.ToTable("NotificacionCotizacion");
        
        // NotificacionId como llave primaria específica
        builder.HasKey(n => n.NotificacionId);
        
        // Configuración de campos
        builder.Property(n => n.NotificacionId)
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        builder.Property(n => n.CotizacionId)
            .HasMaxLength(30)
            .IsRequired();
        
        builder.Property(n => n.VersionId)
            .IsRequired();
        
        builder.Property(n => n.TipoNotificacion)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(n => n.EmailDestino)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(n => n.Asunto)
            .HasMaxLength(300);
        
        builder.Property(n => n.Cuerpo)
            .HasColumnType("nvarchar(max)");
        
        builder.Property(n => n.FechaProgramada)
            .HasColumnType("datetime")
            .IsRequired();
        
        builder.Property(n => n.FechaEnviada)
            .HasColumnType("datetime");
        
        builder.Property(n => n.Estado)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(n => n.Intentos)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(n => n.MensajeError)
            .HasMaxLength(1000);
        
        // Relaciones con Cotizacion y CotizacionVersion
        builder.HasOne(n => n.Cotizacion)
            .WithMany()
            .HasForeignKey(n => n.CotizacionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_NotificacionCotizacion_Cotizacion");
        
        builder.HasOne(n => n.Version)
            .WithMany()
            .HasForeignKey(n => n.VersionId)
            .HasPrincipalKey(v => v.VersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_NotificacionCotizacion_CotizacionVersion");
        
        // Índices para optimización de consultas
        builder.HasIndex(n => new { n.Estado, n.FechaProgramada })
            .HasDatabaseName("IX_NotificacionCotizacion_Estado_FechaProgramada");
        
        builder.HasIndex(n => new { n.CotizacionId, n.VersionId, n.TipoNotificacion })
            .HasDatabaseName("IX_NotificacionCotizacion_CotizacionId_VersionId_TipoNotificacion");
    }
}