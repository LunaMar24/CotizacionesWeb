using CotizacionesWeb.Domain.Entities.ERP;
using Microsoft.EntityFrameworkCore;

namespace CotizacionesWeb.Infrastructure.Data;

/// <summary>
/// Read-only context for ERP database. No migrations generated here.
/// </summary>
public class DbContextErp : DbContext
{
    public DbContextErp(DbContextOptions<DbContextErp> options)
        : base(options)
    {
    }

    // Tablas staging para integración
    public DbSet<CotwebPedidoStg> CotwebPedidoStg => Set<CotwebPedidoStg>();
    public DbSet<CotwebPedidoLineaStg> CotwebPedidoLineaStg => Set<CotwebPedidoLineaStg>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configuración para COTWEB_PEDIDO_STG
        modelBuilder.Entity<CotwebPedidoStg>(entity =>
        {
            entity.HasNoKey();  // Tabla staging sin PK
            entity.ToTable("COTWEB_PEDIDO_STG");
            
            entity.Property(e => e.LoteId).HasColumnName("LOTE_ID").IsRequired();
            entity.Property(e => e.TipoDocumento).HasColumnName("TIPO_DOCUMENTO").HasMaxLength(10).IsRequired();
            entity.Property(e => e.Cliente).HasColumnName("CLIENTE").HasMaxLength(20).IsRequired();
            entity.Property(e => e.CondicionPago).HasColumnName("CONDICION_PAGO").HasMaxLength(10).IsRequired();
            entity.Property(e => e.Bodega).HasColumnName("BODEGA").HasMaxLength(10).IsRequired();
            entity.Property(e => e.Moneda).HasColumnName("MONEDA").HasMaxLength(1).IsRequired();
            entity.Property(e => e.NivelPrecio).HasColumnName("NIVEL_PRECIO").HasMaxLength(12).IsRequired();
            entity.Property(e => e.TipoCambio).HasColumnName("TIPO_CAMBIO").HasColumnType("decimal(18,6)").IsRequired();
            entity.Property(e => e.UsuarioERP).HasColumnName("USUARIO_ERP").HasMaxLength(20).IsRequired();
            entity.Property(e => e.ActividadComercial).HasColumnName("ACTIVIDAD_COMERCIAL").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Observaciones).HasColumnName("OBSERVACIONES").HasMaxLength(500);
            entity.Property(e => e.FechaCreacion).HasColumnName("FECHA_CREACION").IsRequired();
        });

        // Configuración para COTWEB_PEDIDO_LINEA_STG
        modelBuilder.Entity<CotwebPedidoLineaStg>(entity =>
        {
            entity.HasNoKey();  // Tabla staging sin PK
            entity.ToTable("COTWEB_PEDIDO_LINEA_STG");
            
            entity.Property(e => e.LoteId).HasColumnName("LOTE_ID").IsRequired();
            entity.Property(e => e.Linea).HasColumnName("LINEA").IsRequired();
            entity.Property(e => e.Producto).HasColumnName("PRODUCTO").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnName("DESCRIPCION").HasMaxLength(200);
            entity.Property(e => e.Cantidad).HasColumnName("CANTIDAD").HasColumnType("decimal(18,4)").IsRequired();
            entity.Property(e => e.PrecioUnitario).HasColumnName("PRECIO_UNITARIO").HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.MontoDescuento).HasColumnName("MONTO_DESCUENTO").HasColumnType("decimal(18,2)");
            entity.Property(e => e.PorcentajeImpuesto).HasColumnName("PORCENTAJE_IMPUESTO").HasColumnType("decimal(5,2)");
            entity.Property(e => e.Subtotal).HasColumnName("SUBTOTAL").HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Bodega).HasColumnName("BODEGA").HasMaxLength(10).IsRequired();
        });
    }
}
