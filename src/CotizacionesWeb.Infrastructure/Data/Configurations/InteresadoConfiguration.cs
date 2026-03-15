using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class InteresadoConfiguration : IEntityTypeConfiguration<Interesado>
{
    public void Configure(EntityTypeBuilder<Interesado> builder)
    {
        builder.ToTable("Interesado");
        
        // CORREGIDO: InteresadoId como llave primaria única (eliminar duplicados)
        builder.HasKey(i => i.InteresadoId);
        
        builder.Property(i => i.InteresadoId)
               .ValueGeneratedOnAdd();
        
        // Ignorar Id de BaseEntity ya que usamos InteresadoId
        builder.Ignore(i => i.Id);
        
        builder.Property(i => i.HubspotObjectId)
               .HasMaxLength(50)
               .IsRequired();
        
        builder.HasIndex(i => i.HubspotObjectId);
        
        builder.Property(i => i.HubspotObjectType)
               .HasMaxLength(30)
               .IsRequired();
        
        builder.Property(i => i.TipoInteresado)
               .HasMaxLength(1)
               .IsRequired();
        
        builder.Property(i => i.Activo)
               .IsRequired()
               .HasDefaultValue(true);
        
        // CORREGIDO: Fechas como datetime (no datetime2)
        builder.Property(i => i.FechaUltSync)
               .HasColumnType("datetime");
        
        builder.Property(i => i.CreatedAt)
               .HasColumnType("datetime")
               .IsRequired();
        
        builder.Property(i => i.ModifiedAt)
               .HasColumnType("datetime");
        
        // AGREGADO: Foreign Keys para auditoría
        builder.Property(i => i.CreatedBy)
               .IsRequired();
        
        builder.Property(i => i.ModifiedBy);
        
        // Foreign Keys para auditoría
        builder.HasOne<Usuario>()
               .WithMany()
               .HasForeignKey(i => i.CreatedBy)
               .HasPrincipalKey(u => u.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_Interesado_Usuarios_CreatedBy");
               
        builder.HasOne<Usuario>()
               .WithMany()
               .HasForeignKey(i => i.ModifiedBy)
               .HasPrincipalKey(u => u.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_Interesado_Usuarios_ModifiedBy");
    }
}