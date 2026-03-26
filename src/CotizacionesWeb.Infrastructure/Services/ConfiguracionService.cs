using CotizacionesWeb.Application.Configuracion;
using CotizacionesWeb.Domain.Entities;
using CotizacionesWeb.Domain.Enums;
using CotizacionesWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Infrastructure.Services;

/// <summary>
/// Servicio para gestionar la configuración del sistema desde la UI
/// </summary>
public interface IConfiguracionService
{
    Task<List<CategoriaParametrosDto>> ObtenerParametrosPorCategoriaAsync();
    Task<CategoriaParametrosDto?> ObtenerParametrosCategoriaAsync(string categoria);
    Task<ValidacionParametroResult> ValidarParametroAsync(string codigo, string valor, TipoParametro tipo);
    Task<bool> ActualizarParametroAsync(ActualizarParametroRequest request, string usuarioId);
    Task<bool> ResetearParametroAsync(string codigo, string usuarioId);
    Task<bool> ValidarConsecutivosAsync();
}

public class ConfiguracionService : IConfiguracionService
{
    private readonly DbContextCotizaciones _context;
    private readonly IParametroSistemaService _parametroService;
    private readonly ConsecutivoGenerator _consecutivoGenerator;
    private readonly ILogger<ConfiguracionService> _logger;

    // Configuración de categorías con metadatos de UI
    private readonly Dictionary<string, (string Descripcion, string IconoClass)> _categoriasMetadata = new()
    {
        { "Consecutivos", ("Configuración de generación de IDs automáticos", "fas fa-list-ol") },
        { "Financiero", ("Parámetros financieros y monetarios", "fas fa-dollar-sign") },
        { "Notificaciones", ("Configuración de emails y alertas", "fas fa-envelope") },
        { "Seguridad", ("Configuración de seguridad y autenticación", "fas fa-shield-alt") },
        { "Integracion_HubSpot", ("Configuración de integración con HubSpot CRM", "fab fa-hubspot") },
        { "Workflow", ("Reglas de flujo de trabajo", "fas fa-project-diagram") },
        { "Archivos", ("Configuración de almacenamiento", "fas fa-folder") },
        { "Reportes", ("Configuración de informes", "fas fa-chart-bar") },
        { "Negocio", ("Reglas generales del negocio", "fas fa-briefcase") },
        { "Formato", ("Configuración de formatos", "fas fa-file-alt") }
    };

    public ConfiguracionService(
        DbContextCotizaciones context,
        IParametroSistemaService parametroService,
        ConsecutivoGenerator consecutivoGenerator,
        ILogger<ConfiguracionService> logger)
    {
        _context = context;
        _parametroService = parametroService;
        _consecutivoGenerator = consecutivoGenerator;
        _logger = logger;
    }

    public async Task<List<CategoriaParametrosDto>> ObtenerParametrosPorCategoriaAsync()
    {
        try
        {
            var parametros = await _context.Parametros
                .OrderBy(p => p.Categoria)
                .ThenBy(p => p.Codigo)
                .ToListAsync();

            var categorias = parametros
                .GroupBy(p => p.Categoria)
                .Select(g => new CategoriaParametrosDto(
                    g.Key,
                    _categoriasMetadata.TryGetValue(g.Key, out var meta) ? meta.Item1 : g.Key,  // Descripción
                    _categoriasMetadata.TryGetValue(g.Key, out var icon) ? icon.Item2 : "fas fa-cog",  // IconoClass
                    g.Select(p => new ParametroDto(
                        p.ParametroId,
                        p.Codigo,
                        p.Descripcion,
                        p.Valor,
                        ObtenerDescripcionTipo(p.TipoValor),
                        p.Categoria,
                        p.EsModificable,
                        p.ValorPorDefecto,
                        p.Notas
                    )).ToList()
                ))
                .OrderBy(c => c.Categoria)
                .ToList();

            return categorias;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener parámetros por categoría");
            return new List<CategoriaParametrosDto>();
        }
    }

    public async Task<CategoriaParametrosDto?> ObtenerParametrosCategoriaAsync(string categoria)
    {
        try
        {
            var parametros = await _context.Parametros
                .Where(p => p.Categoria == categoria)
                .OrderBy(p => p.Codigo)
                .ToListAsync();

            if (!parametros.Any())
                return null;

            var meta = _categoriasMetadata.TryGetValue(categoria, out var metadata) 
                ? metadata 
                : ("Configuración general", "fas fa-cog");

            return new CategoriaParametrosDto(
                categoria,
                meta.Item1,  // Descripción 
                meta.Item2,  // IconoClass
                parametros.Select(p => new ParametroDto(
                    p.ParametroId,
                    p.Codigo,
                    p.Descripcion,
                    p.Valor,
                    ObtenerDescripcionTipo(p.TipoValor),
                    p.Categoria,
                    p.EsModificable,
                    p.ValorPorDefecto,
                    p.Notas
                )).ToList()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener parámetros de categoría {Categoria}", categoria);
            return null;
        }
    }

    public async Task<ValidacionParametroResult> ValidarParametroAsync(string codigo, string valor, TipoParametro tipo)
    {
        try
        {
            // Validaciones específicas por tipo
            switch (tipo)
            {
                case TipoParametro.Decimal:
                    if (!decimal.TryParse(valor, out var decimalVal))
                        return new ValidacionParametroResult(false, "El valor debe ser un número decimal válido", null);
                    return new ValidacionParametroResult(true, null, decimalVal);

                case TipoParametro.Entero:
                    if (!int.TryParse(valor, out var intVal))
                        return new ValidacionParametroResult(false, "El valor debe ser un número entero válido", null);
                    if (intVal < 0)
                        return new ValidacionParametroResult(false, "El valor no puede ser negativo", null);
                    return new ValidacionParametroResult(true, null, intVal);

                case TipoParametro.Booleano:
                    var valorLower = valor.ToLower().Trim();
                    if (valorLower != "s" && valorLower != "n" && valorLower != "true" && valorLower != "false")
                        return new ValidacionParametroResult(false, "El valor debe ser 'S', 'N', 'true' o 'false'", null);
                    var boolVal = valorLower == "s" || valorLower == "true";
                    return new ValidacionParametroResult(true, null, boolVal);

                case TipoParametro.Fecha:
                    if (!DateTime.TryParse(valor, out var fechaVal))
                        return new ValidacionParametroResult(false, "El valor debe ser una fecha válida", null);
                    return new ValidacionParametroResult(true, null, fechaVal);

                case TipoParametro.Texto:
                    // Validaciones específicas por código de parámetro
                    var resultadoValidacion = await ValidarParametroEspecificoAsync(codigo, valor);
                    return resultadoValidacion;

                default:
                    return new ValidacionParametroResult(true, null, valor);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar parámetro {Codigo}", codigo);
            return new ValidacionParametroResult(false, "Error interno de validación", null);
        }
    }

    public async Task<bool> ActualizarParametroAsync(ActualizarParametroRequest request, string usuarioId)
    {
        try
        {
            var parametro = await _context.Parametros
                .FirstOrDefaultAsync(p => p.ParametroId == request.ParametroId);

            if (parametro == null)
            {
                _logger.LogWarning("Parámetro no encontrado: {ParametroId}", request.ParametroId);
                return false;
            }

            if (!parametro.EsModificable)
            {
                _logger.LogWarning("Intento de modificar parámetro no modificable: {Codigo}", parametro.Codigo);
                return false;
            }

            // Validar el nuevo valor
            var validacion = await ValidarParametroAsync(parametro.Codigo, request.Valor, parametro.TipoValor);
            if (!validacion.EsValido)
            {
                _logger.LogWarning("Valor inválido para parámetro {Codigo}: {Error}", parametro.Codigo, validacion.MensajeError);
                return false;
            }

            var valorAnterior = parametro.Valor;
            parametro.Valor = request.Valor.Trim();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Parámetro {Codigo} actualizado por {Usuario}: {ValorAnterior} ? {ValorNuevo}",
                parametro.Codigo, usuarioId, valorAnterior, request.Valor);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parámetro {Codigo}", request.Codigo);
            return false;
        }
    }

    public async Task<bool> ResetearParametroAsync(string codigo, string usuarioId)
    {
        try
        {
            var parametro = await _context.Parametros
                .FirstOrDefaultAsync(p => p.Codigo == codigo);

            if (parametro == null || !parametro.EsModificable)
                return false;

            if (string.IsNullOrEmpty(parametro.ValorPorDefecto))
            {
                _logger.LogWarning("Parámetro {Codigo} no tiene valor por defecto configurado", codigo);
                return false;
            }

            var valorAnterior = parametro.Valor;
            parametro.Valor = parametro.ValorPorDefecto;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Parámetro {Codigo} reseteado por {Usuario}: {ValorAnterior} ? {ValorDefecto}",
                codigo, usuarioId, valorAnterior, parametro.ValorPorDefecto);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al resetear parámetro {Codigo}", codigo);
            return false;
        }
    }

    public async Task<bool> ValidarConsecutivosAsync()
    {
        return await _parametroService.ValidarConfiguracionConsecutivosAsync();
    }

    private string ObtenerDescripcionTipo(TipoParametro tipo)
    {
        return tipo switch
        {
            TipoParametro.Texto => "Texto",
            TipoParametro.Decimal => "Decimal",
            TipoParametro.Entero => "Entero",
            TipoParametro.Booleano => "Booleano (S/N)",
            TipoParametro.Fecha => "Fecha",
            _ => "Desconocido"
        };
    }

    private async Task<ValidacionParametroResult> ValidarParametroEspecificoAsync(string codigo, string valor)
    {
        try
        {
            // Validaciones específicas por código de parámetro
            switch (codigo)
            {
                case "MASCARA_CONSECUTIVO_COTIZACION":
                    if (!_consecutivoGenerator.ValidarMascara(valor))
                        return new ValidacionParametroResult(false, "Máscara inválida. Use A=letra, 9=número, -=separador", null);
                    break;

                case "CONSECUTIVO_COTIZACION":
                    var mascara = await _parametroService.ObtenerValorParametroAsync("MASCARA_CONSECUTIVO_COTIZACION");
                    if (!string.IsNullOrEmpty(mascara) && !_consecutivoGenerator.ValidarConsecutivo(valor, mascara))
                        return new ValidacionParametroResult(false, $"Consecutivo no coincide con la máscara {mascara}", null);
                    break;

                case "EMAIL_NOTIFICACIONES":
                    if (!valor.Contains("@") || !valor.Contains("."))
                        return new ValidacionParametroResult(false, "Debe ser un email válido", null);
                    break;

                case "MONEDA_DEFECTO":
                    if (valor.Length != 3)
                        return new ValidacionParametroResult(false, "Código de moneda debe tener 3 caracteres (ej: CLP, USD)", null);
                    break;

                case "FORMATO_COTIZACION":
                    var formatosValidos = new[] { "PDF", "EXCEL", "WORD" };
                    if (!formatosValidos.Contains(valor.ToUpper()))
                        return new ValidacionParametroResult(false, "Formato debe ser: PDF, EXCEL o WORD", null);
                    break;

                // Validaciones de HubSpot
                case "HUBSPOT_AUTH_TYPE":
                    var authTypesValidos = new[] { "PRIVATE_APP", "OAUTH" };
                    if (!authTypesValidos.Contains(valor.ToUpper()))
                        return new ValidacionParametroResult(false, "Tipo de autenticación debe ser: PRIVATE_APP o OAUTH", null);
                    break;

                case "HUBSPOT_API_BASE_URL":
                    if (!Uri.TryCreate(valor, UriKind.Absolute, out _))
                        return new ValidacionParametroResult(false, "Debe ser una URL válida", null);
                    break;

                case "HUBSPOT_ACCESS_TOKEN":
                    if (string.IsNullOrWhiteSpace(valor))
                        return new ValidacionParametroResult(false, "El token de acceso no puede estar vacío", null);
                    if (valor.Length < 20)
                        return new ValidacionParametroResult(false, "El token parece demasiado corto para ser válido", null);
                    break;

                case "HUBSPOT_PAGE_SIZE":
                    if (!int.TryParse(valor, out var pageSize) || pageSize < 1 || pageSize > 100)
                        return new ValidacionParametroResult(false, "El tamaño de página debe estar entre 1 y 100", null);
                    break;

                case "HUBSPOT_TIMEOUT_SECONDS":
                case "HUBSPOT_RETRY_COUNT":
                    if (!int.TryParse(valor, out var intValue) || intValue < 1)
                        return new ValidacionParametroResult(false, "Debe ser un número entero mayor a 0", null);
                    break;
            }

            return new ValidacionParametroResult(true, null, valor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en validación específica de parámetro {Codigo}", codigo);
            return new ValidacionParametroResult(false, "Error en validación específica", null);
        }
    }
}