using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CotizacionesWeb.Application.Cotizaciones;
using CotizacionesWeb.UI.Models;
using CotizacionesWeb.UI.Filters;

namespace CotizacionesWeb.UI.Controllers;

[Authorize(Roles = "Admin,Administrador")]
public class CotizacionesController : Controller
{
    private readonly ICotizacionService _cotizacionService;
    private readonly ILogger<CotizacionesController> _logger;

    public CotizacionesController(
        ICotizacionService cotizacionService,
        ILogger<CotizacionesController> logger)
    {
        _cotizacionService = cotizacionService;
        _logger = logger;
    }

    [RequierePermiso("COT_VIEW")]
    public async Task<IActionResult> Index(CotizacionFiltrosViewModel filtros)
    {
        try
        {
            var estadosSeleccionados = new List<char>();
            
            if (filtros.FiltroBorrador) estadosSeleccionados.Add('B');
            if (filtros.FiltroPendienteAprobacion) estadosSeleccionados.Add('P');
            if (filtros.FiltroAprobada) estadosSeleccionados.Add('A');
            if (filtros.FiltroEnviada) estadosSeleccionados.Add('E');
            if (filtros.FiltroAceptada) estadosSeleccionados.Add('T');
            if (filtros.FiltroRechazada) estadosSeleccionados.Add('R');
            // Mantener Cancelada por retrocompatibilidad temporal
            if (filtros.FiltroCancelada) estadosSeleccionados.Add('C');
            // Nota: Archivada (X) NO se incluye en filtros según lineamientos

            var request = new GetCotizacionesListRequest(
                estadosSeleccionados.Any() ? estadosSeleccionados : null,
                filtros.Busqueda,
                filtros.FechaDesde,
                filtros.FechaHasta
            );

            var cotizaciones = await _cotizacionService.GetCotizacionesListAsync(request);

            var viewModel = new CotizacionIndexViewModel
            {
                Cotizaciones = cotizaciones.Select(c => new CotizacionViewModel
                {
                    Id = c.Id,
                    CotizacionId = c.CotizacionId,
                    InteresadoId = c.InteresadoId,
                    NombreInteresado = c.NombreInteresado,
                    EmpresaInteresado = c.EmpresaInteresado,
                    EstadoActual = c.EstadoActual,
                    EstadoActualTexto = ObtenerTextoEstado(c.EstadoActual),
                    VersionActual = c.VersionActual,
                    NumeroVersion = c.NumeroVersion, // Incluir el número específico de versión
                    FechaCreacion = c.FechaCreacion,
                    FechaUltimaActualizacion = c.FechaUltimaActualizacion,
                    MontoCotizacion = c.MontoCotizacion,
                    FechaEnvio = c.FechaEnvio,
                    // Nuevos campos según lineamientos funcionales
                    FechaAceptacion = c.FechaAceptacion,
                    FechaRechazo = c.FechaRechazo,
                    EnviadoERP = c.EnviadoERP,
                    FechaEnvioERP = c.FechaEnvioERP
                }).ToList(),
                Filtros = filtros
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener listado de cotizaciones");
            TempData["Error"] = "Error al cargar las cotizaciones";
            return View(new CotizacionIndexViewModel());
        }
    }

    [HttpGet]
    [RequierePermiso("COT_VIEW")]
    public async Task<IActionResult> Historial(string cotizacionId)
    {
        try
        {
            var historiales = await _cotizacionService.GetCotizacionCurrentHistoryAsync(cotizacionId);

            var viewModel = historiales.Select(h => new HistorialViewModel
            {
                HistorialId = h.HistorialId,
                TipoEvento = h.TipoEvento,
                FechaEvento = h.FechaEvento,
                NombreUsuario = h.NombreUsuario,
                Comentario = h.Comentario
            }).ToList();

            ViewBag.CotizacionId = cotizacionId;
            return PartialView("_HistorialModal", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de cotización {CotizacionId}", cotizacionId);
            return PartialView("_HistorialModal", new List<HistorialViewModel>());
        }
    }

    [HttpGet]
    [RequierePermiso("COT_VIEW")]
    public async Task<IActionResult> Versiones(string cotizacionId)
    {
        try
        {
            var versiones = await _cotizacionService.GetCotizacionVersionsAsync(cotizacionId);

            var viewModel = versiones.Select(v => new CotizacionVersionViewModel
            {
                VersionId = v.VersionId,
                CotizacionId = v.CotizacionId,
                NumeroVersion = v.NumeroVersion,
                FechaVersion = v.FechaVersion,
                NombreInteresado = v.NombreInteresado,
                EmailInteresado = v.EmailInteresado,
                EmpresaInteresado = v.EmpresaInteresado,
                Total = v.Total,
                VersionActual = v.VersionActual
            }).ToList();

            ViewBag.CotizacionId = cotizacionId;
            return PartialView("_VersionesModal", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener versiones de cotización {CotizacionId}", cotizacionId);
            return PartialView("_VersionesModal", new List<CotizacionVersionViewModel>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequierePermiso("COT_EDIT")]
    public async Task<IActionResult> CopiarVersion(string cotizacionId)
    {
        try
        {
            var result = await _cotizacionService.CopiarVersionActualAsync(cotizacionId);

            if (result.Success)
            {
                return Json(new
                {
                    success = true,
                    message = $"Nueva versión {result.NumeroVersion} creada exitosamente"
                });
            }

            return Json(new { success = false, message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al copiar versión de cotización {CotizacionId}", cotizacionId);
            return Json(new { success = false, message = "Error al copiar la versión" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequierePermiso("COT_EDIT")]
    public async Task<IActionResult> CopiarVersionEspecifica(string cotizacionId, int versionId, bool esVersionAntigua)
    {
        try
        {
            var request = new CopiarVersionRequest(cotizacionId, versionId, esVersionAntigua);
            var result = await _cotizacionService.CopiarVersionEspecificaAsync(request);

            if (result.Success)
            {
                return Json(new
                {
                    success = true,
                    message = $"Nueva versión {result.NumeroVersion} creada exitosamente"
                });
            }

            return Json(new { success = false, message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al copiar versión específica {VersionId}", versionId);
            return Json(new { success = false, message = "Error al copiar la versión" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequierePermiso("COT_CREATE")]
    public async Task<IActionResult> Duplicar(string cotizacionId)
    {
        try
        {
            var request = new DuplicarCotizacionRequest(cotizacionId);
            var result = await _cotizacionService.DuplicarCotizacionAsync(request);

            if (result.Success)
            {
                return Json(new
                {
                    success = true,
                    message = $"Cotización {result.NuevaCotizacionId} creada exitosamente"
                });
            }

            return Json(new { success = false, message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al duplicar cotización {CotizacionId}", cotizacionId);
            return Json(new { success = false, message = "Error al duplicar la cotización" });
        }
    }

    private string ObtenerTextoEstado(char estado)
    {
        return estado switch
        {
            'B' => "Borrador",
            'P' => "Pendiente Aprobación",
            'A' => "Aprobada",
            'E' => "Enviada",
            'T' => "Aceptada",
            'R' => "Rechazada",
            'C' => "Cancelada", // Legacy - mantener por retrocompatibilidad
            'X' => "Archivada", // Legacy - mantener por retrocompatibilidad
            _ => "Desconocido"
        };
    }
}
