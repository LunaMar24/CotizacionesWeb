using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CotizacionesWeb.Application.Cotizaciones;
using CotizacionesWeb.UI.Models;
using CotizacionesWeb.UI.Filters;
using CotizacionesWeb.UI.Helpers;
using System.Security.Claims;

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
            // Establecer filtro de fecha predeterminado: últimos 30 días
            // Solo si no se han especificado fechas manualmente
            if (!filtros.FechaDesde.HasValue && !filtros.FechaHasta.HasValue)
            {
                filtros.FechaDesde = DateTime.Today.AddDays(-30);
                filtros.FechaHasta = DateTime.Today.AddDays(1); // Incluir todo el día de hoy
            }

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

            // Validar que la moneda sea válida si se proporciona
            if (!string.IsNullOrWhiteSpace(filtros.Moneda) && !FormatHelper.IsSupportedCurrency(filtros.Moneda))
            {
                _logger.LogWarning("Filtro de moneda inválida: {Moneda}", filtros.Moneda);
                filtros.Moneda = null; // Limpiar filtro inválido
                TempData["Warning"] = "Moneda no soportada. Se muestran todas las monedas.";
            }

            var request = new GetCotizacionesListRequest(
                estadosSeleccionados.Any() ? estadosSeleccionados : null,
                filtros.Busqueda,
                filtros.FechaDesde,
                filtros.FechaHasta,
                filtros.MontoDesde,
                filtros.MontoHasta,
                filtros.Version,
                filtros.Moneda
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
                    // Información financiera
                    Moneda = c.Moneda,
                    // Nuevos campos según lineamientos funcionales
                    FechaAceptacion = c.FechaAceptacion,
                    FechaRechazo = c.FechaRechazo,
                    EnviadoERP = c.EnviadoERP,
                    FechaEnvioERP = c.FechaEnvioERP
                }).ToList(),
                Filtros = filtros
            };

            // Agregar monedas disponibles para el filtro (reutilizando FormatHelper)
            ViewBag.MonedasDisponibles = FormatHelper.GetMonedasDisponibles();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener listado de cotizaciones");
            TempData["Error"] = "Error al cargar las cotizaciones";
            return View(new CotizacionIndexViewModel());
        }
    }

    [HttpGet("Cotizaciones/Historial/{cotizacionId}")]
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

    [HttpGet("Cotizaciones/HistorialVersion/{versionId}")]
    [RequierePermiso("COT_VIEW")]
    public async Task<IActionResult> HistorialVersion(int versionId, string cotizacionId, decimal numeroVersion)
    {
        try
        {
            var historiales = await _cotizacionService.GetCotizacionVersionHistoryAsync(versionId);

            var viewModel = historiales.Select(h => new HistorialViewModel
            {
                HistorialId = h.HistorialId,
                TipoEvento = h.TipoEvento,
                FechaEvento = h.FechaEvento,
                NombreUsuario = h.NombreUsuario,
                Comentario = h.Comentario
            }).ToList();

            ViewBag.CotizacionId = cotizacionId;
            ViewBag.NumeroVersion = numeroVersion;
            ViewBag.EsHistorialVersion = true;
            return PartialView("_HistorialModal", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de versión {VersionId}", versionId);
            return PartialView("_HistorialModal", new List<HistorialViewModel>());
        }
    }

    [HttpGet("Cotizaciones/Versiones/{cotizacionId}")]
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
                Moneda = v.Moneda,
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
    public async Task<IActionResult> CopiarVersion(string cotizacionId, string comentario)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _cotizacionService.CopiarVersionActualAsync(cotizacionId, comentario, currentUserId);

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
    public async Task<IActionResult> CopiarVersionEspecifica(string cotizacionId, int versionId, bool esVersionAntigua, string comentario)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var request = new CopiarVersionRequest(cotizacionId, versionId, esVersionAntigua, comentario);
            var result = await _cotizacionService.CopiarVersionEspecificaAsync(request, currentUserId);

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
    [RequierePermiso("COT_DUPLICATE")]
    public async Task<IActionResult> Duplicar(string cotizacionId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var request = new DuplicarCotizacionRequest(cotizacionId);
            var result = await _cotizacionService.DuplicarCotizacionAsync(request, currentUserId);

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

    [HttpGet("Cotizaciones/Detalle/{cotizacionId}")]
    [RequierePermiso("COT_VIEW_DETAIL")]
    public async Task<IActionResult> Detalle(string cotizacionId, int? versionId = null)
    {
        try
        {
            // Si se especifica una versión específica, usar esa; sino, usar la versión actual
            CotizacionVersionDetalleDto? detalleDto;
            
            if (versionId.HasValue)
            {
                // Cargar versión específica
                detalleDto = await _cotizacionService.GetCotizacionVersionDetailAsync(versionId.Value);
            }
            else
            {
                // Cargar la versión actual de la cotización
                // Primero obtenemos la información básica para saber cuál es la versión actual
                var cotizaciones = await _cotizacionService.GetCotizacionesListAsync(
                    new GetCotizacionesListRequest(null, cotizacionId, null, null, null, null, null, null));
                
                var cotizacion = cotizaciones.FirstOrDefault(c => c.CotizacionId == cotizacionId);
                if (cotizacion == null)
                {
                    return NotFound($"Cotización {cotizacionId} no encontrada");
                }

                // Obtener el detalle de la versión actual
                detalleDto = await _cotizacionService.GetCotizacionVersionDetailAsync(cotizacion.VersionActual);
            }

            if (detalleDto == null)
            {
                return NotFound($"Detalle de cotización {cotizacionId} no encontrado");
            }

            // Obtener información adicional de la cotización para campos que no están en la versión
            var cotizacionInfo = await _cotizacionService.GetCotizacionesListAsync(
                new GetCotizacionesListRequest(null, cotizacionId, null, null, null, null, null, null));
            
            var cotizacionBase = cotizacionInfo.FirstOrDefault(c => c.CotizacionId == cotizacionId);

            // Mapear a ViewModel
            var viewModel = new CotizacionDetalleViewModel
            {
                CotizacionId = detalleDto.Version.CotizacionId,
                EstadoActual = cotizacionBase?.EstadoActual ?? 'B',
                EstadoActualTexto = ObtenerTextoEstado(cotizacionBase?.EstadoActual ?? 'B'),
                FechaCreacion = cotizacionBase?.FechaCreacion ?? DateTime.Now,
                FechaUltimaActualizacion = cotizacionBase?.FechaUltimaActualizacion,
                
                VersionId = detalleDto.Version.VersionId,
                NumeroVersion = detalleDto.Version.NumeroVersion,
                FechaVersion = detalleDto.Version.FechaVersion,
                
                NombreInteresado = detalleDto.Version.NombreInteresado,
                EmailInteresado = detalleDto.Version.EmailInteresado,
                EmpresaInteresado = detalleDto.Version.EmpresaInteresado,
                TipoInteresado = detalleDto.Version.TipoInteresado, // Nuevo campo
                
                SubTotal = detalleDto.Version.SubTotal,
                Impuesto = detalleDto.Version.Impuesto,
                Descuento = detalleDto.Version.Descuento,
                Total = detalleDto.Version.Total,
                Moneda = cotizacionBase?.Moneda ?? "CRC", // ? CORREGIDO: Moneda desde Cotizacion
                TipoCambio = detalleDto.Version.TipoCambio,
                
                FechaEnvio = cotizacionBase?.FechaEnvio,
                FechaAceptacion = cotizacionBase?.FechaAceptacion,
                FechaRechazo = cotizacionBase?.FechaRechazo,
                EnviadoERP = cotizacionBase?.EnviadoERP ?? 'N',
                FechaEnvioERP = cotizacionBase?.FechaEnvioERP,
                
                Notas = detalleDto.Version.Notas,
                
                Detalles = detalleDto.Detalles.Select(d => new DetalleCotizacionViewModel
                {
                    DetalleVersionId = d.DetalleVersionId,
                    ProductoId = d.ProductoId,
                    ProductoNombre = d.ProductoNombre,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Descuento = d.Descuento,
                    TotalLinea = d.TotalLinea
                }).ToList()
            };

            ViewBag.EsVersionEspecifica = versionId.HasValue;
            ViewBag.NumeroVersionMostrada = detalleDto.Version.NumeroVersion;
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalle de cotización {CotizacionId}", cotizacionId);
            TempData["Error"] = "Error al cargar el detalle de la cotización";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet("Cotizaciones/Editar/{cotizacionId}")]
    [RequierePermiso("COT_EDIT")]
    public async Task<IActionResult> Editar(string cotizacionId)
    {
        try
        {
            // Cargar la versión actual de la cotización
            var cotizaciones = await _cotizacionService.GetCotizacionesListAsync(
                new GetCotizacionesListRequest(null, cotizacionId, null, null, null, null, null, null));
            
            var cotizacion = cotizaciones.FirstOrDefault(c => c.CotizacionId == cotizacionId);
            if (cotizacion == null)
            {
                return NotFound($"Cotización {cotizacionId} no encontrada");
            }

            // VALIDACIÓN CRÍTICA: Solo se puede editar en estado Borrador
            if (cotizacion.EstadoActual != 'B')
            {
                TempData["Error"] = $"No se puede editar la cotización {cotizacionId}. Solo las cotizaciones en estado Borrador pueden ser editadas.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener el detalle de la versión actual
            var detalleDto = await _cotizacionService.GetCotizacionVersionDetailAsync(cotizacion.VersionActual);
            if (detalleDto == null)
            {
                return NotFound($"Detalle de cotización {cotizacionId} no encontrado");
            }

            // Mapear a ViewModel de edición
            var viewModel = new CotizacionEditarViewModel
            {
                CotizacionId = detalleDto.Version.CotizacionId,
                EstadoActual = cotizacion.EstadoActual,
                EstadoActualTexto = ObtenerTextoEstado(cotizacion.EstadoActual),
                FechaCreacion = cotizacion.FechaCreacion,
                FechaUltimaActualizacion = cotizacion.FechaUltimaActualizacion,
                
                VersionId = detalleDto.Version.VersionId,
                NumeroVersion = detalleDto.Version.NumeroVersion,
                FechaVersion = detalleDto.Version.FechaVersion,
                
                // Información del interesado (editable)
                InteresadoId = cotizacion.InteresadoId,
                NombreInteresado = detalleDto.Version.NombreInteresado,
                EmailInteresado = detalleDto.Version.EmailInteresado,
                EmpresaInteresado = detalleDto.Version.EmpresaInteresado,
                TipoInteresado = detalleDto.Version.TipoInteresado,
                
                // Información financiera
                SubTotal = detalleDto.Version.SubTotal,
                Impuesto = detalleDto.Version.Impuesto,
                Descuento = detalleDto.Version.Descuento,
                Total = detalleDto.Version.Total,
                Moneda = cotizacion.Moneda, // ? CORREGIDO: Moneda desde Cotizacion
                TipoCambio = detalleDto.Version.TipoCambio,
                
                // Fechas importantes
                FechaEnvio = cotizacion.FechaEnvio,
                FechaAceptacion = cotizacion.FechaAceptacion,
                FechaRechazo = cotizacion.FechaRechazo,
                EnviadoERP = cotizacion.EnviadoERP,
                FechaEnvioERP = cotizacion.FechaEnvioERP,
                
                // Notas (editable)
                Notas = detalleDto.Version.Notas,
                
                // Líneas de detalle
                Detalles = detalleDto.Detalles.Select(d => new DetalleEditarViewModel
                {
                    DetalleVersionId = d.DetalleVersionId,
                    ProductoId = d.ProductoId,
                    ProductoNombre = d.ProductoNombre,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Descuento = d.Descuento,
                    TotalLinea = d.TotalLinea
                }).ToList()
            };
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar cotización para editar {CotizacionId}", cotizacionId);
            TempData["Error"] = "Error al cargar la cotización para edición";
            return RedirectToAction(nameof(Index));
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

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
        {
            return userId;
        }
        return 0; // Fallback para casos donde no se puede obtener el ID
    }
}
