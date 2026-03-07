using CotizacionesWeb.Application.Authentication;
using CotizacionesWeb.Application.Cotizaciones;
using CotizacionesWeb.Application.Integrations;
using CotizacionesWeb.Application.Users;
using CotizacionesWeb.Application.Roles;
using CotizacionesWeb.Infrastructure.Data;
using CotizacionesWeb.Infrastructure.Integrations.Erp;
using CotizacionesWeb.Infrastructure.Integrations.HubSpot;
using CotizacionesWeb.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;

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

    // Database
    builder.Services.AddDbContext<DbContextCotizaciones>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("CotizacionesDb")));

    builder.Services.AddDbContext<DbContextErp>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ErpDb")));

    // Security
    builder.Services.AddScoped<PasswordHasher>();
    builder.Services.AddScoped<IAuthService, AuthService>();

    // Application services - Users & Roles
    builder.Services.AddScoped<IUsuarioService, CotizacionesWeb.Infrastructure.Services.UsuarioService>();
    builder.Services.AddScoped<IRolService, CotizacionesWeb.Infrastructure.Services.RolService>();

    // Application services - Cotizaciones
    builder.Services.AddScoped<ICrearCotizacionService, CrearCotizacionService>();

    // Integrations
    builder.Services.AddScoped<IErpService, ErpService>();
    builder.Services.AddHttpClient<IHubSpotService, HubSpotClient>(client =>
    {
        var baseUrl = builder.Configuration["HubSpot:BaseUrl"] ?? string.Empty;
        if (!string.IsNullOrEmpty(baseUrl))
            client.BaseAddress = new Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });

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
