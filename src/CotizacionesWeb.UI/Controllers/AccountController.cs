using System.Security.Claims;
using CotizacionesWeb.Application.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CotizacionesWeb.UI.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAuthService authService, ILogger<AccountController> logger)
    {
        _authService = authService;
        _logger = logger;
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
}
