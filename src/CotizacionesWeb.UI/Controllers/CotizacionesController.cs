using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CotizacionesWeb.Application.Cotizaciones;
using CotizacionesWeb.UI.Models;
using CotizacionesWeb.UI.Filters;
using CotizacionesWeb.UI.Helpers;
using System.Security.Claims;
using CotizacionesWeb.Application.Integrations;
using CotizacionesWeb.Domain.Enums;
using CotizacionesWeb.Infrastructure.Services;

namespace CotizacionesWeb.UI.Controllers;

[Authorize(Roles = "Admin,Administrador")]
public class CotizacionesController : Controller
{
  private readonly ICotizacionService _cotizacionService;
  private readonly ILogger<CotizacionesController> _logger;
  private readonly IAssignInteresadoHubSpotService _assignInteresadoHubSpotService;
  private readonly IHubSpotService _hubSpotService;
  private readonly IErpService _erpService;
  private readonly IParametroSistemaService _parametroSistemaService;
  private readonly PermisoHelper _permisoHelper;

  public CotizacionesController(
      ICotizacionService cotizacionService,
      ILogger<CotizacionesController> logger,
      IAssignInteresadoHubSpotService assignInteresadoHubSpotService,
      IHubSpotService hubSpotService,
      IErpService erpService,
      IParametroSistemaService parametroSistemaService,
      PermisoHelper permisoHelper)
  {
    _cotizacionService = cotizacionService;
    _logger = logger;
    _assignInteresadoHubSpotService = assignInteresadoHubSpotService;
    _hubSpotService = hubSpotService;
    _erpService = erpService;
    _parametroSistemaService = parametroSistemaService;
    _permisoHelper = permisoHelper;
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
      // ? Archivada (X) se excluye automáticamente en el servicio - las cotizaciones archivadas salen del ámbito de gestión

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
          PorcentajeImpuesto = d.PorcentajeImpuesto,
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

  [HttpGet("Cotizaciones/Crear")]
  [RequierePermiso("COT_CREATE")]
  public async Task<IActionResult> Crear()
  {
    try
    {
      // Obtener parámetros de configuración para valores iniciales
      var monedaDefecto = await _parametroSistemaService.ObtenerValorParametroAsync("MONEDA_DEFECTO") ?? "CRC";
      var tipoCambioBase = await _parametroSistemaService.ObtenerValorParametroAsync<decimal?>("TIPO_CAMBIO_BASE");

      // Obtener parámetros de configuración de impuestos
      var usarImpuestosErp = await _parametroSistemaService.ObtenerValorParametroAsync("ERP_USAR_IMPUESTOS") ?? "S";
      var tasaImpuesto = await _parametroSistemaService.ObtenerValorParametroAsync<decimal?>("TASA_IMPUESTO") ?? 13.0m;

      _logger.LogInformation("Crear cotización - Configuración cargada: Moneda={Moneda}, TipoCambio={TipoCambio}, UsarImpuestosERP={UsarImpuestos}, TasaImpuesto={TasaImpuesto}", 
        monedaDefecto, tipoCambioBase, usarImpuestosErp, tasaImpuesto);

      // Crear ViewModel para nueva cotización con valores predeterminados
      var viewModel = new CotizacionEditarViewModel
      {
        // ID especial para nueva cotización
        CotizacionId = "<Nueva>",
        EstadoActual = 'B', // Siempre comienza en Borrador
        EstadoActualTexto = "Borrador",
        FechaCreacion = DateTime.Now,
        FechaVersion = DateTime.Now,

        // Nuevos IDs temporales (se asignarán al guardar)
        VersionId = 0, // Temporal
        NumeroVersion = 1.0m, // Primera versión

        // Interesado vacío (debe asignarse durante la creación)
        InteresadoId = null,
        NombreInteresado = "",
        EmailInteresado = "",
        EmpresaInteresado = "",
        TipoInteresado = 'P', // Persona por defecto

        // Totales en cero para nueva cotización
        SubTotal = 0,
        Impuesto = 0,
        Descuento = 0,
        Total = 0,
        
        // Configuración inicial desde parámetros
        Moneda = monedaDefecto,
        TipoCambio = tipoCambioBase,

        // Sin fechas especiales (nueva cotización)
        FechaEnvio = null,
        FechaAceptacion = null,
        FechaRechazo = null,
        EnviadoERP = 'N',
        FechaEnvioERP = null,

        // Sin notas iniciales
        Notas = "",

        // Sin líneas de detalle inicialmente
        Detalles = new List<DetalleEditarViewModel>(),

        // Configuración de impuestos desde parámetros del sistema
        UsarImpuestosErp = usarImpuestosErp,
        TasaImpuesto = tasaImpuesto,
        
        // Marca especial para identificar que es creación
        EsNuevaCotizacion = true
      };

      ViewBag.MonedasDisponibles = FormatHelper.GetMonedasDisponiblesParaJson();
      ViewBag.EsCreacion = true; // Para diferenciar comportamiento en la vista

      return View("Editor", viewModel); // Reutilizar la misma vista Editor
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al cargar la pantalla de creación de cotización");
      TempData["Error"] = "Error al cargar la pantalla de creación";
      return RedirectToAction(nameof(Index));
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [RequierePermiso("COT_CREATE")]
  public async Task<IActionResult> GuardarCreacion(CotizacionEditarViewModel viewModel)
  {
    try
    {
      // Validaciones específicas para creación
      if (string.IsNullOrWhiteSpace(viewModel.NombreInteresado))
      {
        return Json(new { success = false, message = "El nombre del interesado es obligatorio para crear la cotización" });
      }

      if (viewModel.Detalles == null || !viewModel.Detalles.Any())
      {
        return Json(new { success = false, message = "Debe agregar al menos una línea de producto para crear la cotización" });
      }

      // Validar líneas de detalle
      for (int i = 0; i < viewModel.Detalles.Count; i++)
      {
        var detalle = viewModel.Detalles[i];
        if (string.IsNullOrWhiteSpace(detalle.ProductoId))
        {
          return Json(new { success = false, message = $"El producto de la línea {i + 1} es obligatorio" });
        }
        if (detalle.Cantidad <= 0)
        {
          return Json(new { success = false, message = $"La cantidad de la línea {i + 1} debe ser mayor a cero" });
        }
        if (detalle.PrecioUnitario < 0)
        {
          return Json(new { success = false, message = $"El precio unitario de la línea {i + 1} no puede ser negativo" });
        }
      }

      // Crear request de creación (reutilizando la estructura de duplicación)
      var currentUserId = GetCurrentUserId();
      
      // Usar el servicio para crear la nueva cotización (necesitaremos agregarlo al servicio)
      var crearRequest = new CrearCotizacionRequest
      {
        NombreInteresado = viewModel.NombreInteresado.Trim(),
        EmailInteresado = viewModel.EmailInteresado?.Trim() ?? "",
        EmpresaInteresado = viewModel.EmpresaInteresado?.Trim() ?? "",
        TipoInteresado = viewModel.TipoInteresado,
        Moneda = viewModel.Moneda ?? "CRC",
        TipoCambio = viewModel.TipoCambio,
        Notas = viewModel.Notas?.Trim() ?? "",
        SubTotal = viewModel.SubTotal,
        TotalDescuentos = viewModel.Descuento,
        Impuesto = viewModel.Impuesto,
        Total = viewModel.Total,
        Detalles = viewModel.Detalles.Select(d => new CrearDetalleRequest
        {
          ProductoId = d.ProductoId,
          ProductoNombre = d.ProductoNombre,
          Cantidad = d.Cantidad,
          PrecioUnitario = d.PrecioUnitario,
          Descuento = d.Descuento,
          PorcentajeImpuesto = d.PorcentajeImpuesto,
          TotalLinea = d.TotalLinea
        }).ToList()
      };

      var resultado = await _cotizacionService.CrearCotizacionAsync(crearRequest, currentUserId);

      if (resultado.Success)
      {
        _logger.LogInformation("Cotización {CotizacionId} creada exitosamente", resultado.CotizacionId);
        return Json(new { 
          success = true, 
          message = $"Cotización {resultado.CotizacionId} creada exitosamente",
          cotizacionId = resultado.CotizacionId
        });
      }
      else
      {
        _logger.LogWarning("Creación de cotización falló: {ErrorMessage}", resultado.ErrorMessage);
        return Json(new { success = false, message = resultado.ErrorMessage });
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al crear nueva cotización");
      return Json(new { success = false, message = "Error interno al crear la cotización" });
    }
  }

  [HttpGet("Cotizaciones/Editor/{cotizacionId}")]
  [RequierePermiso("COT_EDIT")]
  public async Task<IActionResult> Editor(string cotizacionId)
  {
    try
    {
      if (string.IsNullOrEmpty(cotizacionId))
      {
        _logger.LogError("cotizacionId está vacío o nulo");
        TempData["Error"] = "ID de cotización no válido";
        return RedirectToAction(nameof(Index));
      }

      var cotizaciones = await _cotizacionService.GetCotizacionesListAsync(
          new GetCotizacionesListRequest(null, cotizacionId, null, null, null, null, null, null));

      var cotizacion = cotizaciones.FirstOrDefault(c => c.CotizacionId == cotizacionId);
      if (cotizacion == null)
      {
        _logger.LogWarning("Cotización {CotizacionId} no encontrada", cotizacionId);
        return NotFound($"Cotización {cotizacionId} no encontrada");
      }

      if (cotizacion.EstadoActual != 'B')
      {
        _logger.LogWarning("Intento de editar cotización {CotizacionId} en estado {Estado}",
            cotizacion.CotizacionId, cotizacion.EstadoActual);
        TempData["Error"] = $"No se puede editar la cotización {cotizacionId}. Solo las cotizaciones en estado Borrador pueden ser editadas.";
        return RedirectToAction(nameof(Index));
      }

      var detalleDto = await _cotizacionService.GetCotizacionCurrentVersionDetailAsync(cotizacionId);
      if (detalleDto == null)
      {
        return NotFound($"Detalle de cotización {cotizacionId} no encontrado");
      }

      // Obtener parámetros de configuración de impuestos
      var usarImpuestosErp = await _parametroSistemaService.ObtenerValorParametroAsync("ERP_USAR_IMPUESTOS") ?? "S";
      var tasaImpuesto = await _parametroSistemaService.ObtenerValorParametroAsync<decimal?>("TASA_IMPUESTO") ?? 13.0m;

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

        InteresadoId = cotizacion.InteresadoId,
        NombreInteresado = detalleDto.Version.NombreInteresado,
        EmailInteresado = detalleDto.Version.EmailInteresado,
        EmpresaInteresado = detalleDto.Version.EmpresaInteresado,
        TipoInteresado = detalleDto.Version.TipoInteresado,

        SubTotal = detalleDto.Version.SubTotal,
        Impuesto = detalleDto.Version.Impuesto,
        Descuento = detalleDto.Version.Descuento,
        Total = detalleDto.Version.Total,
        Moneda = cotizacion.Moneda,
        TipoCambio = detalleDto.Version.TipoCambio,

        FechaEnvio = cotizacion.FechaEnvio,
        FechaAceptacion = cotizacion.FechaAceptacion,
        FechaRechazo = cotizacion.FechaRechazo,
        EnviadoERP = cotizacion.EnviadoERP,
        FechaEnvioERP = cotizacion.FechaEnvioERP,

        Notas = detalleDto.Version.Notas,

        // Configuración de impuestos desde parámetros del sistema
        UsarImpuestosErp = usarImpuestosErp,
        TasaImpuesto = tasaImpuesto,

        Detalles = detalleDto.Detalles.Select(d => new DetalleEditarViewModel
        {
          DetalleVersionId = d.DetalleVersionId,
          ProductoId = d.ProductoId,
          ProductoNombre = d.ProductoNombre,
          Cantidad = d.Cantidad,
          PrecioUnitario = d.PrecioUnitario,
          Descuento = d.Descuento,
          PorcentajeImpuesto = d.PorcentajeImpuesto,
          TotalLinea = d.TotalLinea
        }).ToList()
      };

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
  [RequiereAlgunPermiso("COT_CREATE", "COT_EDIT")]
  public async Task<IActionResult> BuscarInteresadosHubSpot(char tipoInteresado, string textoBusqueda)
  {
    if (string.IsNullOrWhiteSpace(textoBusqueda))
    {
      return Json(new
      {
        success = true,
        data = new List<HubSpotInteresadoSearchItem>()
      });
    }

    if (tipoInteresado != (char)TipoInteresado.Persona &&
    tipoInteresado != (char)TipoInteresado.Empresa)
    {
      return Json(new
      {
        success = false,
        message = "Tipo de interesado inválido."
      });
    }

    var tipo = (TipoInteresado)tipoInteresado;

    var request = new BuscarInteresadosHubSpotRequest(
        tipo,
        textoBusqueda.Trim());

    var results = await _hubSpotService.BuscarInteresadosAsync(request);

    return Json(new
    {
      success = true,
      data = results
    });
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [RequiereAlgunPermiso("COT_CREATE", "COT_EDIT")]
  public async Task<IActionResult> AsignarInteresadoHubSpot(
    string cotizacionId,
    string hubSpotObjectId,
    string hubSpotObjectType,
    char tipoInteresado,
    string nombreInteresado,
    string? emailInteresado,
    string? empresaInteresado)
  {
    if (string.IsNullOrWhiteSpace(cotizacionId))
    {
      return Json(new
      {
        success = false,
        message = "La cotización es requerida."
      });
    }

    if (string.IsNullOrWhiteSpace(hubSpotObjectId))
    {
      return Json(new
      {
        success = false,
        message = "El identificador del interesado es requerido."
      });
    }

    if (string.IsNullOrWhiteSpace(hubSpotObjectType))
    {
      return Json(new
      {
        success = false,
        message = "El tipo de objeto de HubSpot es requerido."
      });
    }

    if (tipoInteresado != (char)TipoInteresado.Persona &&
        tipoInteresado != (char)TipoInteresado.Empresa)
    {
      return Json(new
      {
        success = false,
        message = "Tipo de interesado inválido."
      });
    }

    var request = new AssignInteresadoHubSpotRequest(
        CotizacionId: cotizacionId.Trim(),
        HubSpotObjectId: hubSpotObjectId.Trim(),
        HubSpotObjectType: hubSpotObjectType.Trim(),
        TipoInteresado: (TipoInteresado)tipoInteresado,
        NombreInteresado: nombreInteresado?.Trim() ?? string.Empty,
        EmailInteresado: string.IsNullOrWhiteSpace(emailInteresado) ? null : emailInteresado.Trim(),
        EmpresaInteresado: string.IsNullOrWhiteSpace(empresaInteresado) ? null : empresaInteresado.Trim()
    );

    var result = await _assignInteresadoHubSpotService.AssignAsync(
        request,
        GetCurrentUserId());

    if (!result.Success)
    {
      return Json(new
      {
        success = false,
        message = result.ErrorMessage ?? "No fue posible asignar el interesado."
      });
    }

    return Json(new
    {
      success = true,
      data = new
      {
        interesadoId = result.InteresadoId,
        tipoInteresado = result.TipoInteresado,
        nombreInteresado = result.NombreInteresado,
        emailInteresado = result.EmailInteresado,
        empresaInteresado = result.EmpresaInteresado
      }
    });
  }

  [HttpGet]
  [RequiereAlgunPermiso("COT_CREATE", "COT_EDIT")]
  public async Task<IActionResult> BuscarProductosErp(string moneda, string? textoBusqueda = null)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(moneda))
      {
        return Json(new
        {
          success = false,
          message = "La moneda es requerida"
        });
      }

      if (!string.IsNullOrWhiteSpace(textoBusqueda) && textoBusqueda.Trim().Length < 2)
      {
        return Json(new
        {
          success = false,
          message = "El texto de búsqueda debe tener al menos 2 caracteres"
        });
      }

      var productos = await _erpService.ObtenerProductosAsync(
          moneda.Trim(),
          string.IsNullOrWhiteSpace(textoBusqueda) ? null : textoBusqueda.Trim());

      var productosFormateados = productos.Select(p => new
      {
        value = p.Producto,
        text = $"{p.Producto} - {p.Descripcion}",
        precio = p.Precio,
        porcentajeImpuesto = p.Porcentaje,
        impuesto = p.CodigoImpuesto
      }).ToList();

      return Json(new
      {
        success = true,
        data = productosFormateados
      });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al consultar productos ERP con moneda {Moneda} y búsqueda '{TextoBusqueda}'",
          moneda, textoBusqueda);
      return Json(new
      {
        success = false,
        message = "Error al consultar productos ERP"
      });
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [RequierePermiso("COT_EDIT")]
  public async Task<IActionResult> GuardarEdicion(CotizacionEditarViewModel viewModel)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(viewModel.NombreInteresado))
      {
        return Json(new { success = false, message = "El nombre del interesado es obligatorio" });
      }

      if (viewModel.Detalles == null || !viewModel.Detalles.Any())
      {
        _logger.LogInformation("Guardando cotización {CotizacionId} sin líneas de detalle (estado borrador)", viewModel.CotizacionId);
      }

      for (int i = 0; i < viewModel.Detalles.Count; i++)
      {
        var detalle = viewModel.Detalles[i];
        if (string.IsNullOrWhiteSpace(detalle.ProductoId))
        {
          return Json(new { success = false, message = $"El producto de la línea {i + 1} es obligatorio" });
        }
        if (detalle.Cantidad <= 0)
        {
          return Json(new { success = false, message = $"La cantidad de la línea {i + 1} debe ser mayor a cero" });
        }
        if (detalle.PrecioUnitario < 0)
        {
          return Json(new { success = false, message = $"El precio unitario de la línea {i + 1} no puede ser negativo" });
        }
      }

      var request = new ActualizarCotizacionRequest
      {
        CotizacionId = viewModel.CotizacionId,
        VersionId = viewModel.VersionId,
        NumeroVersion = viewModel.NumeroVersion,
        NombreInteresado = viewModel.NombreInteresado.Trim(),
        EmailInteresado = viewModel.EmailInteresado?.Trim() ?? "",
        EmpresaInteresado = viewModel.EmpresaInteresado?.Trim() ?? "",
        TipoInteresado = viewModel.TipoInteresado,
        Moneda = viewModel.Moneda,
        TipoCambio = viewModel.TipoCambio,
        Notas = viewModel.Notas?.Trim() ?? "",
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
          PorcentajeImpuesto = d.PorcentajeImpuesto,
          TotalLinea = d.TotalLinea
        }).ToList()
      };

      var currentUserId = GetCurrentUserId();
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

  // ========================================
  // ENDPOINTS DE CAMBIO DE ESTADO
  // ========================================

  [HttpPost]
  [ValidateAntiForgeryToken]
  [RequierePermiso("COT_EDIT")]
  public async Task<IActionResult> EnviarAprobacion(string cotizacionId)
  {
    try
    {
      var result = await _cotizacionService.CambiarEstadoCotizacionAsync(
        cotizacionId, 
        (char)EstadoCotizacion.Borrador, 
        (char)EstadoCotizacion.PendienteAprobacion, 
        "Enviada a aprobación", 
        GetCurrentUserId());
      
      if (result.Success)
      {
        return Json(new { success = true, message = "Cotización enviada a aprobación exitosamente" });
      }
      
      return Json(new { success = false, message = result.ErrorMessage });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al enviar cotización {CotizacionId} a aprobación", cotizacionId);
      return Json(new { success = false, message = "Error interno al cambiar el estado" });
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [RequierePermiso("COT_APPROVE")]
  public async Task<IActionResult> Aprobar(string cotizacionId)
  {
    try
    {
      var result = await _cotizacionService.CambiarEstadoCotizacionAsync(
        cotizacionId, 
        (char)EstadoCotizacion.PendienteAprobacion, 
        (char)EstadoCotizacion.Aprobada, 
        "Cotización aprobada", 
        GetCurrentUserId());
      
      if (result.Success)
      {
        return Json(new { success = true, message = "Cotización aprobada exitosamente" });
      }
      
      return Json(new { success = false, message = result.ErrorMessage });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al aprobar cotización {CotizacionId}", cotizacionId);
      return Json(new { success = false, message = "Error interno al cambiar el estado" });
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [RequiereAlgunPermiso("COT_APPROVE", "COT_REJECT", "COT_ARCHIVE")]
  public async Task<IActionResult> CambiarEstadoConNota(string cotizacionId, char estadoOrigen, char estadoDestino, string nota, string? comentarioAdicional = null)
  {
    try
    {
      // Validar que la nota sea obligatoria
      if (string.IsNullOrWhiteSpace(nota))
      {
        return Json(new { success = false, message = "La nota explicativa es obligatoria para este cambio de estado" });
      }

      if (nota.Trim().Length < 10)
      {
        return Json(new { success = false, message = "La nota debe tener al menos 10 caracteres" });
      }

      // Validar transición válida y permiso específico
      var (esValida, mensaje, permisoRequerido) = ValidarTransicionConNota(estadoOrigen, estadoDestino);
      
      if (!esValida)
      {
        return Json(new { success = false, message = "Transición de estado no válida" });
      }

      // Verificar que el usuario tenga el permiso específico para esta transición
      var tienePermisoEspecifico = await _permisoHelper.UsuarioTienePermisoAsync(User, permisoRequerido);
      
      if (!tienePermisoEspecifico)
      {
        _logger.LogWarning("Usuario {UserId} intentó cambiar estado sin permiso {Permiso}", GetCurrentUserId(), permisoRequerido);
        return Json(new { success = false, message = $"No tiene permisos para realizar esta operación. Se requiere: {permisoRequerido}" });
      }

      // Para archivado, usar el método específico que maneja el comentario adicional
      if (estadoDestino == 'X') // Archivada
      {
        // Usar ambos campos por separado: motivo obligatorio y comentario adicional
        var result = await _cotizacionService.CambiarEstadoCotizacionBasicoAsync(
          cotizacionId, 
          estadoDestino, 
          nota.Trim(), // Este va como motivo obligatorio (MotivoArchivado)
          comentarioAdicional, // Este va como comentario adicional (Comentario)
          GetCurrentUserId());
        
        if (result.Success)
        {
          return Json(new { success = true, message = mensaje, shouldReload = true });
        }
        
        return Json(new { success = false, message = result.ErrorMessage });
      }
      else
      {
        // Para otros cambios de estado, usar el método normal
        var result = await _cotizacionService.CambiarEstadoCotizacionAsync(
          cotizacionId, 
          estadoOrigen, 
          estadoDestino, 
          nota.Trim(),
          GetCurrentUserId());
        
        if (result.Success)
        {
          return Json(new { success = true, message = mensaje, shouldReload = true });
        }
        
        return Json(new { success = false, message = result.ErrorMessage });
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al cambiar estado de cotización {CotizacionId}", cotizacionId);
      return Json(new { success = false, message = "Error interno al cambiar el estado" });
    }
  }

  private static (bool esValida, string mensaje, string permisoRequerido) ValidarTransicionConNota(char estadoOrigen, char estadoDestino)
  {
    return (estadoOrigen, estadoDestino) switch
    {
      ('P', 'B') => (true, "Cotización devuelta a borrador exitosamente", "COT_APPROVE"),
      ('P', 'A') => (true, "Cotización aprobada exitosamente", "COT_APPROVE"),
      ('E', 'R') => (true, "Cotización marcada como rechazada exitosamente", "COT_REJECT"),
      (_, 'X') => (true, "Cotización archivada exitosamente", "COT_ARCHIVE"),
      _ => (false, "", "")
    };
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [RequierePermiso("COT_APPROVE")]
  public async Task<IActionResult> DevolverBorrador(string cotizacionId, string nota)
  {
    // Redireccionar al método genérico
    return await CambiarEstadoConNota(cotizacionId, 'P', 'B', nota);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [RequierePermiso("COT_SEND_CLIENT")]
  public async Task<IActionResult> EnviarCliente(string cotizacionId)
  {
    try
    {
      var result = await _cotizacionService.CambiarEstadoCotizacionAsync(
        cotizacionId, 
        (char)EstadoCotizacion.Aprobada, 
        (char)EstadoCotizacion.Enviada, 
        "Enviada al cliente", 
        GetCurrentUserId());
      
      if (result.Success)
      {
        return Json(new { success = true, message = "Cotización enviada al cliente exitosamente" });
      }
      
      return Json(new { success = false, message = result.ErrorMessage });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al enviar cotización {CotizacionId} al cliente", cotizacionId);
      return Json(new { success = false, message = "Error interno al cambiar el estado" });
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [RequierePermiso("COT_ACCEPT")]
  public async Task<IActionResult> MarcarAceptada(string cotizacionId)
  {
    try
    {
      var result = await _cotizacionService.CambiarEstadoCotizacionAsync(
        cotizacionId, 
        (char)EstadoCotizacion.Enviada, 
        (char)EstadoCotizacion.Aceptada, 
        "Marcada como aceptada por el cliente", 
        GetCurrentUserId());
      
      if (result.Success)
      {
        return Json(new { success = true, message = "Cotización marcada como aceptada exitosamente" });
      }
      
      return Json(new { success = false, message = result.ErrorMessage });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al marcar cotización {CotizacionId} como aceptada", cotizacionId);
      return Json(new { success = false, message = "Error interno al cambiar el estado" });
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [RequierePermiso("COT_REJECT")]
  public async Task<IActionResult> MarcarRechazada(string cotizacionId)
  {
    try
    {
      var result = await _cotizacionService.CambiarEstadoCotizacionAsync(
        cotizacionId, 
        (char)EstadoCotizacion.Enviada, 
        (char)EstadoCotizacion.Rechazada, 
        "Marcada como rechazada por el cliente", 
        GetCurrentUserId());
      
      if (result.Success)
      {
        return Json(new { success = true, message = "Cotización marcada como rechazada exitosamente" });
      }
      
      return Json(new { success = false, message = result.ErrorMessage });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al marcar cotización {CotizacionId} como rechazada", cotizacionId);
      return Json(new { success = false, message = "Error interno al cambiar el estado" });
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [RequierePermiso("COT_ARCHIVE")]
  public async Task<IActionResult> Archivar(string cotizacionId)
  {
    try
    {
      var result = await _cotizacionService.CambiarEstadoCotizacionBasicoAsync(
        cotizacionId, 
        (char)EstadoCotizacion.Archivada, 
        "Cotización archivada", // Motivo por defecto
        null, // Sin comentario adicional
        GetCurrentUserId());
      
      if (result.Success)
      {
        return Json(new { success = true, message = "Cotización archivada exitosamente" });
      }
      
      return Json(new { success = false, message = result.ErrorMessage });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al archivar cotización {CotizacionId}", cotizacionId);
      return Json(new { success = false, message = "Error interno al cambiar el estado" });
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [RequierePermiso("COT_SEND_ERP")]
  public async Task<IActionResult> EnviarERP(string cotizacionId)
  {
    try
    {
      var result = await _cotizacionService.MarcarEnvioERPAsync(cotizacionId, GetCurrentUserId());
      
      if (result.Success)
      {
        return Json(new { success = true, message = "Cotización enviada al ERP exitosamente" });
      }
      
      return Json(new { success = false, message = result.ErrorMessage });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al enviar cotización {CotizacionId} al ERP", cotizacionId);
      return Json(new { success = false, message = "Error interno al enviar al ERP" });
    }
  }

  private int GetCurrentUserId()
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (int.TryParse(userIdClaim, out int userId))
    {
      return userId;
    }
    return 0;
  }
}