using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CotizacionesWeb.Infrastructure.Data;

/// <summary>
/// Factory para crear DbContext en tiempo de diseño (migraciones)
/// CORREGIDO: Usar el mismo connection string que la aplicación
/// </summary>
public class DbContextCotizacionesFactory : IDesignTimeDbContextFactory<DbContextCotizaciones>
{
  public DbContextCotizaciones CreateDbContext(string[] args)
  {
    var optionsBuilder = new DbContextOptionsBuilder<DbContextCotizaciones>();

    // Usar el connection string de Azure directamente para migraciones
    //var connectionString = "Server=tcp:lunamarsql.database.windows.net,1433;Database=CotizacionesWeb;User Id=CloudSA916628a2;Password=bp_7fy+UP[GE;TrustServerCertificate=True;";

    //Conección local para desarrollo
    var connectionString = "Server=localhost;Database=CotizacionesWeb;User Id=sa;Password=123;TrustServerCertificate=True;";

    optionsBuilder.UseSqlServer(connectionString);

    return new DbContextCotizaciones(optionsBuilder.Options);
  }
}