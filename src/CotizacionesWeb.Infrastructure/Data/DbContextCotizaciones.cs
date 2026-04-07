using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CotizacionesWeb.Infrastructure.Data;

public class DbContextCotizaciones : DbContext
{
  public DbContextCotizaciones(DbContextOptions<DbContextCotizaciones> options)
      : base(options)
  {
  }

  // Tablas de Seguridad
  public DbSet<Usuario> Usuarios => Set<Usuario>();
  public DbSet<Rol> Roles => Set<Rol>();
  public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
  public DbSet<Permiso> Permisos => Set<Permiso>();
  public DbSet<PermisoRol> PermisosRoles => Set<PermisoRol>();

  // Tablas de Cotizaciones
  public DbSet<Interesado> Interesados => Set<Interesado>();
  public DbSet<Cotizacion> Cotizaciones => Set<Cotizacion>();
  public DbSet<CotizacionVersion> CotizacionesVersiones => Set<CotizacionVersion>();
  public DbSet<DetalleCotizacionVersion> DetallesCotizacionVersion => Set<DetalleCotizacionVersion>();
  public DbSet<HistorialCotizacion> HistorialesCotizacion => Set<HistorialCotizacion>();
  public DbSet<ArchivoCotizacion> ArchivosCotizacion => Set<ArchivoCotizacion>();

  // Tablas de Configuración
  public DbSet<Parametros> Parametros => Set<Parametros>();

  // Tablas Control Notificaciones
  public DbSet<NotificacionCotizacion> NotificacionesCotizacion => Set<NotificacionCotizacion>();

  // Tablas Control de Integracion ERP
  public DbSet<IntegracionPedidoErp> IntegracionesPedidoErp => Set<IntegracionPedidoErp>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContextCotizaciones).Assembly);
  }
}
