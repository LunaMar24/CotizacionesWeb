using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CotizacionesWeb.UI.Models;
using CotizacionesWeb.Application.Users;

namespace CotizacionesWeb.UI.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUsuarioService _usuarioService;

    public HomeController(ILogger<HomeController> logger, IUsuarioService usuarioService)
    {
        _logger = logger;
        _usuarioService = usuarioService;
    }

    public async Task<IActionResult> Index()
    {
        var usuarios = await _usuarioService.GetAllAsync();
        
        var viewModel = new DashboardViewModel
        {
            UsuarioNombre = User.Identity?.Name ?? "Usuario",
            Estadisticas = new EstadisticasGenerales
            {
                TotalUsuarios = usuarios.Count,
                UsuariosActivos = usuarios.Count(u => u.Activo),
                UsuariosInactivos = usuarios.Count(u => !u.Activo),
                TotalCotizaciones = 0,
                CotizacionesPendientes = 0,
                CotizacionesAprobadas = 0,
                MontoTotalCotizaciones = 0
            },
            ActividadesRecientes = new List<ActividadReciente>
            {
                new ActividadReciente
                {
                    Icono = "fa-user-plus",
                    Titulo = "Usuarios Registrados",
                    Descripcion = $"Hay {usuarios.Count} usuarios en el sistema",
                    Fecha = DateTime.Now,
                    TipoClase = "success"
                }
            }
        };
        
        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

