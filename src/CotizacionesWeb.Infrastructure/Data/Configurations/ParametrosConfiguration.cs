using CotizacionesWeb.Domain.Entities;
using CotizacionesWeb.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CotizacionesWeb.Infrastructure.Data.Configurations;

public class ParametrosConfiguration : IEntityTypeConfiguration<Parametros>
{
    public void Configure(EntityTypeBuilder<Parametros> builder)
    {
        builder.ToTable("Parametros");
        
        // Clave primaria
        builder.HasKey(p => p.ParametroId);
        
        builder.Property(p => p.ParametroId)
               .ValueGeneratedOnAdd();
        
        // Campos expandidos para sistema de consecutivos
        
        // Código único para identificación programática
        builder.Property(p => p.Codigo)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.HasIndex(p => p.Codigo)
            .IsUnique()
            .HasDatabaseName("IX_Parametros_Codigo");
        
        // Descripción expandida
        builder.Property(p => p.Descripcion)
            .HasMaxLength(500)  // Expandido de 100 a 500
            .IsRequired();
        
        // Valor expandido para datos más complejos
        builder.Property(p => p.Valor)
            .HasMaxLength(1000)  // Expandido de 200 a 1000
            .IsRequired();
        
        // TipoValor usando enum existente (se guarda como char en BD)
        builder.Property(p => p.TipoValor)
            .HasConversion(
                v => (char)v,  // Convertir enum a char para BD
                v => (TipoParametro)v  // Convertir char a enum desde BD
            )
            .HasMaxLength(1)
            .IsRequired()
            .HasDefaultValue(TipoParametro.Texto);  // Usar valor del enum
        
        // Nueva categoría para agrupación
        builder.Property(p => p.Categoria)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.HasIndex(p => p.Categoria)
            .HasDatabaseName("IX_Parametros_Categoria");
        
        // EsModificable para proteger parámetros críticos
        builder.Property(p => p.EsModificable)
            .IsRequired()
            .HasDefaultValue(true);
        
        builder.HasIndex(p => p.EsModificable)
            .HasDatabaseName("IX_Parametros_EsModificable");
        
        // ValorPorDefecto opcional
        builder.Property(p => p.ValorPorDefecto)
            .HasMaxLength(1000)
            .IsRequired(false);
        
        // Notas opcionales
        builder.Property(p => p.Notas)
            .HasMaxLength(2000)
            .IsRequired(false);
        
        // EsSensitivo para parámetros con información confidencial
        builder.Property(p => p.EsSensitivo)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.HasIndex(p => p.EsSensitivo)
            .HasDatabaseName("IX_Parametros_EsSensitivo");
        
        // Índices para performance
        builder.HasIndex(p => new { p.Categoria, p.Codigo })
            .HasDatabaseName("IX_Parametros_Categoria_Codigo");
        
        // Restricciones usando valores del enum
        builder.HasCheckConstraint("CK_Parametros_TipoValor", 
            $"[TipoValor] IN ('{(char)TipoParametro.Texto}', '{(char)TipoParametro.Decimal}', '{(char)TipoParametro.Booleano}', '{(char)TipoParametro.Fecha}', '{(char)TipoParametro.Entero}')");
    }
}
