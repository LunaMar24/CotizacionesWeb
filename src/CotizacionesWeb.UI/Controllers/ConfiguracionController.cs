using CotizacionesWeb.Application.Configuracion;
using CotizacionesWeb.Infrastructure.Services;
using CotizacionesWeb.UI.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CotizacionesWeb.UI.Controllers;

[RequierePermiso("CFG_PARAMS_VIEW")]
public class ConfiguracionController : Controller
{
    private readonly IConfiguracionService _configuracionService;

    public ConfiguracionController(IConfiguracionService configuracionService)
    {
        _configuracionService = configuracionService;
    }

    public async Task<IActionResult> Parametros(string? categoria = null)
    {
        // Verificar permisos
        var puedeEditar = User.IsInRole("Admin") || User.IsInRole("Administrador") || 
                         User.HasClaim("Permission", "CFG_PARAMS_EDIT");

        // Cargar todas las categorías para el menú lateral
        var categorias = await _configuracionService.ObtenerParametrosPorCategoriaAsync();

        // Validar consecutivos para mostrar alertas
        var consecutivosValidos = await _configuracionService.ValidarConsecutivosAsync();

        // Si se especifica una categoría, cargarla
        CategoriaParametrosDto? categoriaActual = null;
        if (!string.IsNullOrEmpty(categoria))
        {
            categoriaActual = await _configuracionService.ObtenerParametrosCategoriaAsync(categoria);
        }
        else if (categorias.Any())
        {
            // Por defecto, mostrar la primera categoría
            categoria = categorias.First().Categoria;
            categoriaActual = categorias.First();
        }

        ViewBag.CategoriaSeleccionada = categoria;
        ViewBag.Categorias = categorias;
        ViewBag.CategoriaActual = categoriaActual;
        ViewBag.PuedeEditar = puedeEditar;
        ViewBag.ConsecutivosValidos = consecutivosValidos;

        return View();
    }

    [HttpPost]
    [RequierePermiso("CFG_PARAMS_EDIT")]
    public async Task<IActionResult> ActualizarParametro(int parametroId, string codigo, string valor, string? categoriaSeleccionada)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Parametros), new { categoria = categoriaSeleccionada });
        }

        var request = new ActualizarParametroRequest(parametroId, codigo, valor);
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "sistema";

        var resultado = await _configuracionService.ActualizarParametroAsync(request, usuarioId);

        if (resultado)
        {
            TempData["SuccessMessage"] = $"Parámetro {codigo} actualizado correctamente";
        }
        else
        {
            TempData["ErrorMessage"] = $"Error al actualizar el parámetro {codigo}";
        }

        return RedirectToAction(nameof(Parametros), new { categoria = categoriaSeleccionada });
    }

    [HttpPost]
    [RequierePermiso("CFG_PARAMS_RESET")]
    public async Task<IActionResult> ResetearParametro(string codigo, string? categoriaSeleccionada)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "sistema";
        var resultado = await _configuracionService.ResetearParametroAsync(codigo, usuarioId);

        if (resultado)
        {
            TempData["SuccessMessage"] = $"Parámetro {codigo} reseteado al valor por defecto";
        }
        else
        {
            TempData["ErrorMessage"] = $"Error al resetear el parámetro {codigo}";
        }

        return RedirectToAction(nameof(Parametros), new { categoria = categoriaSeleccionada });
    }

    [HttpGet]
    public async Task<IActionResult> ValidarParametro(string codigo, string valor, string tipo)
    {
        if (!Enum.TryParse<Domain.Enums.TipoParametro>(tipo, out var tipoParametro))
        {
            return Json(new { esValido = false, mensaje = "Tipo de parámetro inválido" });
        }

        var resultado = await _configuracionService.ValidarParametroAsync(codigo, valor, tipoParametro);
        
        return Json(new { 
            esValido = resultado.EsValido, 
            mensaje = resultado.MensajeError 
        });
    }
}