using CotizacionesWeb.Application.Documents;
using CotizacionesWeb.UI.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CotizacionesWeb.UI.Controllers;

[Authorize]
public class DocumentosController : Controller
{
    private readonly IDocumentoCotizacionService _documentoService;
    private readonly ILogger<DocumentosController> _logger;

    public DocumentosController(
        IDocumentoCotizacionService documentoService,
        ILogger<DocumentosController> logger)
    {
        _documentoService = documentoService;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequierePermiso("COT_EXPORT")]
    public async Task<IActionResult> GenerarDocumentoCotizacion(string cotizacionId, int? versionId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(cotizacionId))
            {
                return Json(new { success = false, message = "ID de cotización requerido" });
            }

            var request = new GenerarDocumentoCotizacionRequest(cotizacionId, versionId);
            var resultado = await _documentoService.GenerarDocumentoCotizacionAsync(request);

            if (!resultado.Success)
            {
                return Json(new { success = false, message = resultado.ErrorMessage });
            }

            // Devolver el archivo Word
            return File(
                resultado.DocumentContent!,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                resultado.FileName!
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar documento para cotización {CotizacionId}", cotizacionId);
            return Json(new { success = false, message = "Error interno al generar documento" });
        }
    }

    [HttpGet]
    [RequierePermiso("COT_EXPORT")]
    public async Task<IActionResult> ValidarPlantilla()
    {
        try
        {
            var esValida = await _documentoService.ValidarPlantillaAsync();
            var placeholders = await _documentoService.ObtenerPlaceholdersDisponiblesAsync();

            return Json(new
            {
                success = true,
                plantillaValida = esValida,
                placeholdersEncontrados = placeholders,
                message = esValida ? "Plantilla válida" : "Plantilla no encontrada o inválida"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar plantilla");
            return Json(new { success = false, message = "Error al validar plantilla" });
        }
    }
}