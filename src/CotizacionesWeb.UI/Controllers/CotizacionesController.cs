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

                // Obtener el detalle de la versión actual usando el método correcto
                detalleDto = await _cotizacionService.GetCotizacionCurrentVersionDetailAsync(cotizacionId);
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
            _logger.LogInformation("=== MÉTODO EDITAR INICIADO ===");
            _logger.LogInformation("CotizacionId recibido en URL: '{CotizacionId}'", cotizacionId);
            _logger.LogInformation("Tipo del parámetro: {TipoParametro}", cotizacionId?.GetType().Name);
            _logger.LogInformation("Request.Path: {RequestPath}", Request.Path);
            _logger.LogInformation("Request.PathBase: {RequestPathBase}", Request.PathBase);
            _logger.LogInformation("Request.QueryString: {QueryString}", Request.QueryString);
            _logger.LogInformation("RouteValues completos: {RouteValues}", string.Join(", ", RouteData.Values.Select(kv => $"{kv.Key}={kv.Value}")));
            _logger.LogInformation("================================");

            // VERIFICACIÓN ESPECÍFICA: ¿El parámetro es nulo o está mal?
            if (string.IsNullOrEmpty(cotizacionId))
            {
                _logger.LogError("PROBLEMA CRÍTICO: cotizacionId está vacío o nulo");
                TempData["Error"] = "ID de cotización no válido";
                return RedirectToAction(nameof(Index));
            }
            
            // Cargar la versión actual de la cotización
            var cotizaciones = await _cotizacionService.GetCotizacionesListAsync(
                new GetCotizacionesListRequest(null, cotizacionId, null, null, null, null, null, null));
            
            var cotizacion = cotizaciones.FirstOrDefault(c => c.CotizacionId == cotizacionId);
            if (cotizacion == null)
            {
                _logger.LogWarning("Cotización {CotizacionId} no encontrada", cotizacionId);
                return NotFound($"Cotización {cotizacionId} no encontrada");
            }

            _logger.LogInformation("Cotización encontrada: {CotizacionId}, Estado: {Estado}", 
                cotizacion.CotizacionId, cotizacion.EstadoActual);

            // VALIDACIÓN CRÍTICA: Solo se puede editar en estado Borrador
            if (cotizacion.EstadoActual != 'B')
            {
                _logger.LogWarning("Intento de editar cotización {CotizacionId} en estado {Estado}", 
                    cotizacion.CotizacionId, cotizacion.EstadoActual);
                TempData["Error"] = $"No se puede editar la cotización {cotizacionId}. Solo las cotizaciones en estado Borrador pueden ser editadas.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener el detalle de la versión actual usando el nuevo método correcto
            _logger.LogInformation("Obteniendo detalle de versión actual para cotización {CotizacionId}", cotizacionId);
            
            var detalleDto = await _cotizacionService.GetCotizacionCurrentVersionDetailAsync(cotizacionId);
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
            
            _logger.LogInformation("ViewModel creado con CotizacionId: '{ViewModelCotizacionId}'", viewModel.CotizacionId);
            _logger.LogInformation("ViewModel VersionId: {ViewModelVersionId}", viewModel.VersionId);
            _logger.LogInformation("Puede cambiar moneda: {PuedeCambiarMoneda}", viewModel.PuedeCambiarMoneda);
            
            // Agregar monedas disponibles para el filtro
            ViewBag.MonedasDisponibles = FormatHelper.GetMonedasDisponiblesParaJson();
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar cotización para editar {CotizacionId}", cotizacionId);
            TempData["Error"] = "Error al cargar la cotización para edición";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequierePermiso("COT_EDIT")]
    public async Task<IActionResult> GuardarEdicion(CotizacionEditarViewModel viewModel)
    {
        try
        {
            _logger.LogInformation("Iniciando guardado de cotización {CotizacionId}, VersionId: {VersionId}", 
                viewModel.CotizacionId, viewModel.VersionId);

            // Validación de estado: usar directamente el servicio de actualización que ya valida
            // En lugar de hacer consulta separada que puede tener timing issues

            // Validaciones básicas PRIMERO
            if (string.IsNullOrWhiteSpace(viewModel.NombreInteresado))
            {
                _logger.LogWarning("Guardado rechazado: Nombre del interesado vacío para {CotizacionId}", viewModel.CotizacionId);
                return Json(new { success = false, message = "El nombre del interesado es obligatorio" });
            }

            if (string.IsNullOrWhiteSpace(viewModel.EmailInteresado))
            {
                _logger.LogWarning("Guardado rechazado: Email del interesado vacío para {CotizacionId}", viewModel.CotizacionId);
                return Json(new { success = false, message = "El email del interesado es obligatorio" });
            }

            if (viewModel.Detalles == null || !viewModel.Detalles.Any())
            {
                _logger.LogInformation("Guardando cotización {CotizacionId} sin líneas de detalle (estado borrador permitido)", viewModel.CotizacionId);
                // ? PERMITIDO: En estado Borrador se puede guardar sin líneas
                // No bloquear, solo registrar para auditoría
            }

            // Validar detalles
            for (int i = 0; i < viewModel.Detalles.Count; i++)
            {
                var detalle = viewModel.Detalles[i];
                if (string.IsNullOrWhiteSpace(detalle.ProductoId))
                {
                    _logger.LogWarning("Guardado rechazado: ProductoId vacío en línea {Linea} para {CotizacionId}", i + 1, viewModel.CotizacionId);
                    return Json(new { success = false, message = $"El producto de la línea {i + 1} es obligatorio" });
                }
                if (detalle.Cantidad <= 0)
                {
                    _logger.LogWarning("Guardado rechazado: Cantidad inválida en línea {Linea} para {CotizacionId}", i + 1, viewModel.CotizacionId);
                    return Json(new { success = false, message = $"La cantidad de la línea {i + 1} debe ser mayor a cero" });
                }
                if (detalle.PrecioUnitario < 0)
                {
                    _logger.LogWarning("Guardado rechazado: Precio inválido en línea {Linea} para {CotizacionId}", i + 1, viewModel.CotizacionId);
                    return Json(new { success = false, message = $"El precio unitario de la línea {i + 1} no puede ser negativo" });
                }
            }

            // Crear request para el servicio
            var request = new ActualizarCotizacionRequest
            {
                CotizacionId = viewModel.CotizacionId,
                VersionId = viewModel.VersionId,
                // ?? NUEVO: Incluir número de versión si fue modificado
                NumeroVersion = viewModel.NumeroVersion,
                NombreInteresado = viewModel.NombreInteresado.Trim(),
                EmailInteresado = viewModel.EmailInteresado.Trim(),
                EmpresaInteresado = viewModel.EmpresaInteresado?.Trim() ?? "",
                TipoInteresado = viewModel.TipoInteresado,
                Moneda = viewModel.Moneda, // Incluir la moneda
                TipoCambio = viewModel.TipoCambio, // Incluir el tipo de cambio
                Notas = viewModel.Notas?.Trim() ?? "",
                // ?? NUEVO: Incluir totales calculados
                SubTotal = viewModel.SubTotal,
                TotalDescuentos = viewModel.Descuento,
                Impuesto = viewModel.Impuesto,
                Total = viewModel.Total,
                Detalles = viewModel.Detalles.Select(d => new ActualizarDetalleRequest
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

            _logger.LogInformation("?? Request para actualización creado con los siguientes datos importantes:");
            _logger.LogInformation("  - CotizacionId: {CotizacionId}", request.CotizacionId);
            _logger.LogInformation("  - VersionId: {VersionId}", request.VersionId);
            _logger.LogInformation("  - Moneda en request: '{Moneda}'", request.Moneda);
            _logger.LogInformation("  - NumeroVersion: {NumeroVersion}", request.NumeroVersion);
            _logger.LogInformation("  - TipoCambio: {TipoCambio}", request.TipoCambio);
            _logger.LogInformation("  - Cantidad de detalles: {DetallesCount}", request.Detalles.Count);

            _logger.LogInformation("Request creado para {CotizacionId}: {DetallesCount} detalles", 
                viewModel.CotizacionId, request.Detalles.Count);

            var currentUserId = GetCurrentUserId();
            
            // El servicio ActualizarCotizacionAsync ya valida el estado internamente
            var resultado = await _cotizacionService.ActualizarCotizacionAsync(request, currentUserId);

            if (resultado.Success)
            {
                _logger.LogInformation("Cotización {CotizacionId} guardada exitosamente", viewModel.CotizacionId);
                return Json(new { success = true, message = "Cotización guardada exitosamente" });
            }
            else
            {
                _logger.LogWarning("Guardado falló para {CotizacionId}: {ErrorMessage}", viewModel.CotizacionId, resultado.ErrorMessage);
                return Json(new { success = false, message = resultado.ErrorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar cotización {CotizacionId}", viewModel?.CotizacionId);
            return Json(new { success = false, message = "Error interno al guardar la cotización" });
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

    // MÉTODO DE DEBUGGING TEMPORAL - Remover en producción
    [HttpGet("Cotizaciones/Debug/{cotizacionId}")]
    [RequierePermiso("COT_VIEW")]
    public async Task<IActionResult> Debug(string cotizacionId)
    {
        try
        {
            _logger.LogInformation("=== DEBUGGING COTIZACIÓN {CotizacionId} ===", cotizacionId);
            
            // Usar el servicio respetando arquitectura limpia
            var debugInfo = await _cotizacionService.GetCotizacionDebugInfoAsync(cotizacionId);
            
            // También consultar a través del listado para comparar
            var cotizacionesServicio = await _cotizacionService.GetCotizacionesListAsync(
                new GetCotizacionesListRequest(null, cotizacionId, null, null, null, null, null, null));
            
            var cotizacionServicio = cotizacionesServicio.FirstOrDefault(c => c.CotizacionId == cotizacionId);
            
            _logger.LogInformation("Información de debugging obtenida. Estado: {Estado}, Existe: {Existe}", 
                debugInfo.EstadoActual, debugInfo.ExisteEnBase);
            
            return Json(new
            {
                debugInfo = new {
                    debugInfo.CotizacionId,
                    debugInfo.EstadoActual,
                    debugInfo.EstadoTexto,
                    debugInfo.VersionActual,
                    debugInfo.FechaCreacion,
                    debugInfo.FechaModificacion,
                    debugInfo.VersionInfo,
                    debugInfo.ExisteEnBase
                },
                cotizacionServicio = cotizacionServicio != null ? new {
                    cotizacionServicio.CotizacionId,
                    cotizacionServicio.EstadoActual,
                    EstadoTexto = ObtenerTextoEstado(cotizacionServicio.EstadoActual)
                } : null,
                timestamp = DateTime.Now,
                arquitecturaLimpia = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en debugging de cotización {CotizacionId}", cotizacionId);
            return Json(new { error = ex.Message });
        }
    }

    // ?? MÉTODO ESPECÍFICO PARA DEBUG DE MONEDA
    [HttpGet("Cotizaciones/DebugMoneda/{cotizacionId}")]
    [RequierePermiso("COT_VIEW")]
    public async Task<IActionResult> DebugMoneda(string cotizacionId)
    {
        try
        {
            _logger.LogInformation("=== DEBUGGING MONEDA COTIZACIÓN {CotizacionId} ===", cotizacionId);
            
            // Obtener información directa de la base de datos
            var cotizaciones = await _cotizacionService.GetCotizacionesListAsync(
                new GetCotizacionesListRequest(null, cotizacionId, null, null, null, null, null, null));
            
            var cotizacion = cotizaciones.FirstOrDefault(c => c.CotizacionId == cotizacionId);
            
            if (cotizacion == null)
            {
                return Json(new { error = "Cotización no encontrada", timestamp = DateTime.Now });
            }

            // Obtener detalle de la versión actual
            var detalle = await _cotizacionService.GetCotizacionCurrentVersionDetailAsync(cotizacionId);
            
            var result = new
            {
                cotizacionId = cotizacion.CotizacionId,
                monedaCotizacion = cotizacion.Moneda,
                estado = cotizacion.EstadoActual,
                versionActual = cotizacion.VersionActual,
                detalleVersion = detalle != null ? new
                {
                    versionId = detalle.Version.VersionId,
                    numeroVersion = detalle.Version.NumeroVersion,
                    tipoCambio = detalle.Version.TipoCambio,
                    cantidadDetalles = detalle.Detalles.Count,
                    lineasPersistentes = detalle.Detalles.Count(d => d.DetalleVersionId > 0)
                } : null,
                timestamp = DateTime.Now,
                puedeEditarMoneda = cotizacion.EstadoActual == 'B' && 
                                   (detalle?.Detalles.Count(d => d.DetalleVersionId > 0) ?? 0) == 0
            };

            _logger.LogInformation("Debug moneda completado: Moneda={Moneda}, Estado={Estado}, PuedeEditar={PuedeEditar}",
                result.monedaCotizacion, result.estado, result.puedeEditarMoneda);

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en debugging de moneda para cotización {CotizacionId}", cotizacionId);
            return Json(new { error = ex.Message, timestamp = DateTime.Now });
        }
    }

    // ?? MÉTODO PARA FORZAR ELIMINACIÓN DE LÍNEAS Y PERMITIR CAMBIO DE MONEDA
    [HttpPost("Cotizaciones/EliminarLineasParaCambioMoneda/{cotizacionId}")]
    [ValidateAntiForgeryToken]
    [RequierePermiso("COT_EDIT")]
    public async Task<IActionResult> EliminarLineasParaCambioMoneda(string cotizacionId)
    {
        try
        {
            _logger.LogInformation("=== ELIMINANDO LÍNEAS PARA CAMBIO DE MONEDA {CotizacionId} ===", cotizacionId);
            
            // ?? CORRECCIÓN CRÍTICA: Obtener también la información de la cotización para la moneda
            var cotizaciones = await _cotizacionService.GetCotizacionesListAsync(
                new GetCotizacionesListRequest(null, cotizacionId, null, null, null, null, null, null));
            
            var cotizacion = cotizaciones.FirstOrDefault(c => c.CotizacionId == cotizacionId);
            if (cotizacion == null)
            {
                return Json(new { success = false, message = "Cotización no encontrada" });
            }

            // Obtener detalle actual
            var detalle = await _cotizacionService.GetCotizacionCurrentVersionDetailAsync(cotizacionId);
            if (detalle == null)
            {
                return Json(new { success = false, message = "Detalle de versión no encontrado" });
            }

            var lineasPersistentes = detalle.Detalles.Where(d => d.DetalleVersionId > 0).ToList();
            
            if (!lineasPersistentes.Any())
            {
                return Json(new { success = true, message = "No hay líneas persistentes que eliminar" });
            }

            _logger.LogInformation("?? Monedas disponibles:");
            _logger.LogInformation("  - Cotización.Moneda: '{MonedaCotizacion}'", cotizacion.Moneda);
            _logger.LogInformation("  - Version.Moneda: '{MonedaVersion}'", detalle.Version.Moneda);
            _logger.LogInformation("  - Usando moneda de cotización: '{MonedaUsada}'", cotizacion.Moneda);

            // ?? CORRECCIÓN CRÍTICA: Crear request usando la moneda correcta desde la entidad Cotizacion
            var request = new ActualizarCotizacionRequest
            {
                CotizacionId = cotizacionId,
                VersionId = detalle.Version.VersionId,
                NombreInteresado = detalle.Version.NombreInteresado,
                EmailInteresado = detalle.Version.EmailInteresado,
                EmpresaInteresado = detalle.Version.EmpresaInteresado,
                TipoInteresado = detalle.Version.TipoInteresado,
                Moneda = cotizacion.Moneda, // ?? CORREGIDO: Usar moneda desde Cotizacion, no desde Version
                TipoCambio = detalle.Version.TipoCambio,
                Notas = detalle.Version.Notas,
                Detalles = new List<ActualizarDetalleRequest>() // Sin detalles = eliminar todos
            };

            var currentUserId = GetCurrentUserId();
            var resultado = await _cotizacionService.ActualizarCotizacionAsync(request, currentUserId);

            if (resultado.Success)
            {
                _logger.LogInformation("Líneas eliminadas exitosamente para cotización {CotizacionId}", cotizacionId);
                return Json(new { 
                    success = true, 
                    message = $"Se eliminaron {lineasPersistentes.Count} líneas persistentes. Ahora puede cambiar la moneda." 
                });
            }
            else
            {
                return Json(new { success = false, message = resultado.ErrorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar líneas para cambio de moneda {CotizacionId}", cotizacionId);
            return Json(new { success = false, message = "Error interno al eliminar líneas" });
        }
    }
}
