using CotizacionesWeb.Application.Authentication;
using CotizacionesWeb.Application.Cotizaciones;
using CotizacionesWeb.Application.Integrations.Erp;
using CotizacionesWeb.Application.Integrations.Erp.Services;
using CotizacionesWeb.Application.Integrations.HubSpot;
using CotizacionesWeb.Application.Roles;
using CotizacionesWeb.Application.Users;
using CotizacionesWeb.Infrastructure.Data;
using CotizacionesWeb.Infrastructure.Data.Interceptors;
using CotizacionesWeb.Infrastructure.Integrations.Erp.Services;
using CotizacionesWeb.Infrastructure.Integrations.HubSpot;
using CotizacionesWeb.Infrastructure.Integrations.HubSpot.Services;
using CotizacionesWeb.Infrastructure.Security;
using CotizacionesWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

try
{
  Log.Information("Starting CotizacionesWeb");

  var builder = WebApplication.CreateBuilder(args);

  builder.Host.UseSerilog();

  // HTTP Context Accessor (necesario para auditoría y permisos)
  builder.Services.AddHttpContextAccessor();

  // Database con interceptor de auditoría
  builder.Services.AddDbContext<DbContextCotizaciones>((serviceProvider, options) =>
  {
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

    var auditInterceptor = new AuditInterceptor(() =>
      {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
          var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
          if (int.TryParse(userIdClaim, out int userId))
          {
            return userId;
          }
        }
        return null; // null para usuarios no autenticados - se convertirá en 0 en el interceptor
      });

    options.UseSqlServer(builder.Configuration.GetConnectionString("CotizacionesDb"))
             .AddInterceptors(auditInterceptor);
  });

  builder.Services.AddDbContext<DbContextErp>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("ErpDb")));

  // Security
  builder.Services.AddScoped<PasswordHasher>();
  builder.Services.AddScoped<IAuthService, AuthService>();

  // Application services - Users & Roles
  builder.Services.AddScoped<IUsuarioService, CotizacionesWeb.Infrastructure.Services.UsuarioService>();
  builder.Services.AddScoped<IRolService, CotizacionesWeb.Infrastructure.Services.RolService>();
  builder.Services.AddScoped<CotizacionesWeb.Application.Permisos.IPermisoService, CotizacionesWeb.Infrastructure.Services.PermisoService>();

  // UI Services
  builder.Services.AddScoped<CotizacionesWeb.UI.Services.IPermisoChecker, CotizacionesWeb.UI.Services.PermisoChecker>();
  builder.Services.AddScoped<CotizacionesWeb.UI.Helpers.PermisoHelper>();

  // Application services - Cotizaciones
  builder.Services.AddScoped<ICotizacionService, CotizacionesWeb.Infrastructure.Services.CotizacionService>();
  
  // Application services - Documentos
  builder.Services.AddScoped<CotizacionesWeb.Application.Documents.IDocumentoCotizacionService, CotizacionesWeb.Infrastructure.Services.DocumentoCotizacionService>();

  // Application services - Notificaciones
  builder.Services.AddScoped<CotizacionesWeb.Application.Notificaciones.INotificacionCotizacionService, CotizacionesWeb.Infrastructure.Services.NotificacionCotizacionService>();
  builder.Services.AddScoped<CotizacionesWeb.Application.Common.Email.IEmailService, CotizacionesWeb.Infrastructure.Services.EmailService>();

  // Sistema de parámetros y consecutivos
  builder.Services.AddScoped<CotizacionesWeb.Infrastructure.Services.ConsecutivoGenerator>();
  builder.Services.AddScoped<CotizacionesWeb.Infrastructure.Services.IConfiguracionService, CotizacionesWeb.Infrastructure.Services.ConfiguracionService>();

  // Integrations ERP
  builder.Services.AddScoped<IErpConfigurationService, ErpConfigurationService>();
  builder.Services.AddScoped<IErpPedidoService, ErpPedidoService>();

  // Integrations
  builder.Services.AddScoped<IErpService, ErpService>();
  builder.Services.AddHttpClient<IHubSpotService, HubSpotClient>();
  builder.Services.AddScoped<IAssignInteresadoHubSpotService, AssignInteresadoHubSpotService>();

  // Background Services - Notificaciones
  builder.Services.AddHostedService<CotizacionesWeb.Infrastructure.Services.NotificacionCotizacionBackgroundService>();

  // Authentication
  builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
          options.LoginPath = "/Account/Login";
          options.AccessDeniedPath = "/Account/Denied";
          options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
          options.SlidingExpiration = true;
          options.Cookie.HttpOnly = true;
          options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
        });

  builder.Services.AddAuthorization();

  builder.Services.AddControllersWithViews();

  var app = builder.Build();

  if (!app.Environment.IsDevelopment())
  {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
  }

  app.UseHttpsRedirection();
  app.UseStaticFiles();

  app.UseRouting();

  app.UseAuthentication();
  app.UseAuthorization();

  app.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}");

  app.Run();
}
catch (Exception ex)
{
  Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}
