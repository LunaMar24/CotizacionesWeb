using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CotizacionesWeb.UI.Models;
using CotizacionesWeb.Application.Cotizaciones;

namespace CotizacionesWeb.UI.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICotizacionService _cotizacionService;

    public HomeController(ILogger<HomeController> logger, ICotizacionService cotizacionService)
    {
        _logger = logger;
        _cotizacionService = cotizacionService;
    }

    public async Task<IActionResult> Index()
    {
        var conteosPorEstado = await _cotizacionService.GetCotizacionesCountByEstadoAsync();
        
        var estadosCotizaciones = new List<EstadoCotizacionCard>
        {
            new EstadoCotizacionCard
            {
                CodigoEstado = 'B',
                NombreEstado = "Borrador",
                Cantidad = conteosPorEstado.GetValueOrDefault('B', 0),
                ColorClase = "secondary",
                Icono = "fa-file-alt"
            },
            new EstadoCotizacionCard
            {
                CodigoEstado = 'P',
                NombreEstado = "Pendiente Aprobación",
                Cantidad = conteosPorEstado.GetValueOrDefault('P', 0),
                ColorClase = "warning",
                Icono = "fa-clock"
            },
            new EstadoCotizacionCard
            {
                CodigoEstado = 'A',
                NombreEstado = "Aprobada",
                Cantidad = conteosPorEstado.GetValueOrDefault('A', 0),
                ColorClase = "success",
                Icono = "fa-check-circle"
            },
            new EstadoCotizacionCard
            {
                CodigoEstado = 'E',
                NombreEstado = "Enviada",
                Cantidad = conteosPorEstado.GetValueOrDefault('E', 0),
                ColorClase = "info",
                Icono = "fa-paper-plane"
            },
            new EstadoCotizacionCard
            {
                CodigoEstado = 'T',
                NombreEstado = "Aceptada",
                Cantidad = conteosPorEstado.GetValueOrDefault('T', 0),
                ColorClase = "primary",
                Icono = "fa-thumbs-up"
            },
            new EstadoCotizacionCard
            {
                CodigoEstado = 'R',
                NombreEstado = "Rechazada",
                Cantidad = conteosPorEstado.GetValueOrDefault('R', 0),
                ColorClase = "danger",
                Icono = "fa-times-circle"
            }
        };
        
        var viewModel = new DashboardViewModel
        {
            UsuarioNombre = User.Identity?.Name ?? "Usuario",
            EstadosCotizaciones = estadosCotizaciones
        };
        
        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

