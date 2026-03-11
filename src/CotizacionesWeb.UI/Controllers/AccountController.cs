using System.Security.Claims;
using CotizacionesWeb.Application.Authentication;
using CotizacionesWeb.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CotizacionesWeb.UI.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly PasswordHasher _passwordHasher;
    private readonly ILogger<AccountController> _logger;
    private readonly IWebHostEnvironment _environment;

    public AccountController(
        IAuthService authService, 
        PasswordHasher passwordHasher,
        ILogger<AccountController> logger,
        IWebHostEnvironment environment)
    {
        _authService = authService;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(new LoginRequest
        {
            Email = model.Email,
            Password = model.Password
        });

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Error al iniciar sesión.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.UsuarioId.ToString()),
            new Claim(ClaimTypes.Name, result.NombreUsuario ?? string.Empty),
            new Claim(ClaimTypes.Email, model.Email)
        };

        foreach (var rol in result.Roles)
            claims.Add(new Claim(ClaimTypes.Role, rol));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        _logger.LogInformation("Usuario {Email} Logueado", model.Email);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Denied()
    {
        return View();
    }

    // ============================================
    // ENDPOINT TEMPORAL PARA GENERAR HASHES
    // ⚠️ SOLO DISPONIBLE EN DESARROLLO
    // ============================================
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GenerarHash(string? password)
    {
        // Verificar que estamos en entorno de desarrollo
        if (!_environment.IsDevelopment())
        {
            _logger.LogWarning("Intento de acceso a GenerarHash en entorno {Environment} desde IP: {IP}", 
                _environment.EnvironmentName,
                HttpContext.Connection.RemoteIpAddress);
            return NotFound();
        }

        if (!string.IsNullOrEmpty(password))
        {
            var hash = _passwordHasher.Hash(password);
            _logger.LogInformation("Hash generado en entorno de desarrollo");
            return Content($"Password: {password}\nHash: {hash}", "text/plain");
        }
        
        return Content("Uso: /Account/GenerarHash?password=TuContraseña\n\n⚠️ Este endpoint solo está disponible en desarrollo.", "text/plain");
    }
}


