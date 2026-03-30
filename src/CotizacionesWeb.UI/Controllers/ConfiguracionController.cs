using CotizacionesWeb.Application.Configuracion;
using CotizacionesWeb.Application.Permisos;
using CotizacionesWeb.Infrastructure.Services;
using CotizacionesWeb.UI.Filters;
using CotizacionesWeb.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CotizacionesWeb.UI.Controllers;

[RequierePermiso("CFG_PARAMS_VIEW")]
public class ConfiguracionController : Controller
{
    private readonly IConfiguracionService _configuracionService;
    private readonly IPermisoService _permisoService;
    private readonly ILogger<ConfiguracionController> _logger;

    public ConfiguracionController(
        IConfiguracionService configuracionService,
        IPermisoService permisoService,
        ILogger<ConfiguracionController> logger)
    {
        _configuracionService = configuracionService;
        _permisoService = permisoService;
        _logger = logger;
    }

    public async Task<IActionResult> Parametros(string? categoria = null)
    {
        var usuarioId = User.GetUsuarioId();
        
        // Verificar permisos usando el servicio
        var puedeEditar = await _permisoService.UsuarioTienePermisoAsync(usuarioId, "CFG_PARAMS_EDIT");
        var puedeVerSecretos = await _permisoService.UsuarioTienePermisoAsync(usuarioId, "CFG_PARAMS_VIEW_SECRET");

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
        ViewBag.PuedeVerSecretos = puedeVerSecretos;
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
    
    /// <summary>
    /// Revela el valor real de un parámetro sensitivo
    /// SEGURIDAD: Requiere permiso especial CFG_PARAMS_VIEW_SECRET
    /// </summary>
    [HttpGet]
    [RequierePermiso("CFG_PARAMS_VIEW_SECRET")]
    public async Task<IActionResult> RevelarValorSensitivo(int parametroId)
    {
        try
        {
            // Obtener parámetro desde la base de datos (con valor real)
            var parametro = await _configuracionService.ObtenerParametroRealAsync(parametroId);
            
            if (parametro == null)
            {
                return Json(new { 
                    success = false, 
                    message = "Parámetro no encontrado" 
                });
            }
            
            // Verificar que el parámetro sea realmente sensitivo
            if (!parametro.EsSensitivo)
            {
                return Json(new { 
                    success = false, 
                    message = "Este parámetro no es sensitivo. Use el valor visible en pantalla." 
                });
            }
            
            // Usuario tiene permiso y parámetro es sensitivo: devolver valor real
            var usuarioId = User.FindFirstValue(ClaimTypes.Email) ?? "sistema";
            _logger.LogWarning("Usuario {Usuario} reveló valor sensitivo del parámetro {Codigo}", 
                usuarioId, parametro.Codigo);
            
            return Json(new { 
                success = true, 
                valor = parametro.Valor,
                codigo = parametro.Codigo
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al revelar valor sensitivo del parámetro {ParametroId}", parametroId);
            return Json(new { 
                success = false, 
                message = "Error al obtener el valor del parámetro" 
            });
        }
    }
}