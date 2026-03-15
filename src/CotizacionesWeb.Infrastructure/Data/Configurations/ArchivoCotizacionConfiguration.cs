using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class ArchivoCotizacionConfiguration : IEntityTypeConfiguration<ArchivoCotizacion>
{
    public void Configure(EntityTypeBuilder<ArchivoCotizacion> builder)
    {
        builder.ToTable("ArchivoCotizacion");
        
        // ArchivoId como llave primaria (sin BaseEntity según modelo)
        builder.HasKey(a => a.ArchivoId);
        
        builder.Property(a => a.ArchivoId)
               .ValueGeneratedOnAdd();
        
        builder.Property(a => a.CotizacionId)
            .HasMaxLength(30)
            .IsRequired();
        
        // CORREGIDO: Fechas como datetime (no datetime2)
        builder.Property(a => a.FechaArchivado)
            .HasColumnType("datetime")
            .IsRequired();
        
        builder.Property(a => a.FechaReactivacion)
            .HasColumnType("datetime");
        
        builder.Property(a => a.TipoArchivo)
            .HasMaxLength(1)
            .IsRequired();
        
        builder.Property(a => a.MotivoArchivado)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(a => a.Comentario)
            .HasMaxLength(500);
        
        // AGREGADO: Foreign Keys para usuarios
        builder.Property(a => a.UsuarioArchiva);
        builder.Property(a => a.UsuarioReactiva);
        
        // Foreign Keys para UsuarioArchiva y UsuarioReactiva
        builder.HasOne<Usuario>()
               .WithMany()
               .HasForeignKey(a => a.UsuarioArchiva)
               .HasPrincipalKey(u => u.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_ArchivoCotizacion_Usuarios_UsuarioArchiva");
               
        builder.HasOne<Usuario>()
               .WithMany()
               .HasForeignKey(a => a.UsuarioReactiva)
               .HasPrincipalKey(u => u.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_ArchivoCotizacion_Usuarios_UsuarioReactiva");
        
        builder.HasOne(a => a.Cotizacion)
            .WithMany(c => c.Archivos)
            .HasForeignKey(a => a.CotizacionId)
            .HasPrincipalKey(c => c.CotizacionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(a => a.FechaArchivado);
    }
}