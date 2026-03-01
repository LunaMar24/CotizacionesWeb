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

    // TODO: Add DbSet properties for ERP entities when integration is defined
}
