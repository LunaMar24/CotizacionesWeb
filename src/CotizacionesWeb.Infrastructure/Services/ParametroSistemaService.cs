using CotizacionesWeb.Domain.Entities;
using CotizacionesWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Infrastructure.Services;

/// <summary>
/// Servicio para manejar parámetros del sistema y consecutivos
/// ACTUALIZADO: Para usar la entidad Parametros existente
/// </summary>
public interface IParametroSistemaService
{
    Task<string?> ObtenerValorParametroAsync(string codigo);
    Task<T?> ObtenerValorParametroAsync<T>(string codigo);
    Task<bool> ActualizarParametroAsync(string codigo, string nuevoValor);
    Task<string> ObtenerSiguienteConsecutivoCotizacionAsync();
    Task<bool> ValidarConfiguracionConsecutivosAsync();
}

public class ParametroSistemaService : IParametroSistemaService
{
    private readonly DbContextCotizaciones _context;
    private readonly ConsecutivoGenerator _consecutivoGenerator;
    private readonly ILogger<ParametroSistemaService> _logger;

    // Constantes para códigos de parámetros críticos
    public const string MASCARA_CONSECUTIVO_COTIZACION = "MASCARA_CONSECUTIVO_COTIZACION";
    public const string CONSECUTIVO_COTIZACION = "CONSECUTIVO_COTIZACION";

    public ParametroSistemaService(
        DbContextCotizaciones context,
        ConsecutivoGenerator consecutivoGenerator,
        ILogger<ParametroSistemaService> logger)
    {
        _context = context;
        _consecutivoGenerator = consecutivoGenerator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene el valor de un parámetro como string
    /// </summary>
    public async Task<string?> ObtenerValorParametroAsync(string codigo)
    {
        try
        {
            var parametro = await _context.Parametros  // Usar tabla Parametros existente
                .FirstOrDefaultAsync(p => p.Codigo == codigo);

            if (parametro == null)
            {
                _logger.LogWarning("Parámetro no encontrado: {Codigo}", codigo);
                return null;
            }

            return parametro.Valor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener parámetro {Codigo}", codigo);
            return null;
        }
    }

    /// <summary>
    /// Obtiene el valor de un parámetro convertido al tipo especificado
    /// </summary>
    public async Task<T?> ObtenerValorParametroAsync<T>(string codigo)
    {
        var valor = await ObtenerValorParametroAsync(codigo);
        
        if (valor == null)
            return default(T);

        try
        {
            return (T)Convert.ChangeType(valor, typeof(T));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al convertir parámetro {Codigo} al tipo {Tipo}", codigo, typeof(T).Name);
            return default(T);
        }
    }

    /// <summary>
    /// Actualiza el valor de un parámetro
    /// </summary>
    public async Task<bool> ActualizarParametroAsync(string codigo, string nuevoValor)
    {
        try
        {
            var parametro = await _context.Parametros  // Usar tabla Parametros existente
                .FirstOrDefaultAsync(p => p.Codigo == codigo);

            if (parametro == null)
            {
                _logger.LogWarning("Parámetro no encontrado para actualizar: {Codigo}", codigo);
                return false;
            }

            if (!parametro.EsModificable)
            {
                _logger.LogWarning("Parámetro {Codigo} no es modificable", codigo);
                return false;
            }

            var valorAnterior = parametro.Valor;
            parametro.Valor = nuevoValor;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Parámetro {Codigo} actualizado: {ValorAnterior} ? {ValorNuevo}", 
                codigo, valorAnterior, nuevoValor);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parámetro {Codigo}", codigo);
            return false;
        }
    }

    /// <summary>
    /// Obtiene y actualiza el siguiente consecutivo para cotizaciones
    /// </summary>
    public async Task<string> ObtenerSiguienteConsecutivoCotizacionAsync()
    {
        try
        {
            // Validar configuración antes de proceder
            if (!await ValidarConfiguracionConsecutivosAsync())
            {
                throw new InvalidOperationException("Configuración de consecutivos inválida");
            }

            // Obtener parámetros críticos
            var mascara = await ObtenerValorParametroAsync(MASCARA_CONSECUTIVO_COTIZACION);
            var consecutivoActual = await ObtenerValorParametroAsync(CONSECUTIVO_COTIZACION);

            if (string.IsNullOrEmpty(mascara) || string.IsNullOrEmpty(consecutivoActual))
            {
                throw new InvalidOperationException($"Parámetros de consecutivo no configurados: Mascara={mascara}, Consecutivo={consecutivoActual}");
            }

            // Verificar si ya hay una transacción activa
            var transaccionExterna = _context.Database.CurrentTransaction != null;
            
            if (transaccionExterna)
            {
                // Ya hay una transacción activa, ejecutar dentro de ella
                _logger.LogDebug("Usando transacción existente para generar consecutivo");
                return await ExecutarGeneracionConsecutivoAsync(consecutivoActual, mascara);
            }
            else
            {
                // No hay transacción, crear una nueva
                _logger.LogDebug("Creando nueva transacción para generar consecutivo");
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var resultado = await ExecutarGeneracionConsecutivoAsync(consecutivoActual, mascara);
                    await transaction.CommitAsync();
                    return resultado;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico al generar consecutivo de cotización");
            throw new InvalidOperationException($"Error al generar consecutivo de cotización: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Ejecuta la lógica de generación de consecutivo (sin transacción)
    /// </summary>
    private async Task<string> ExecutarGeneracionConsecutivoAsync(string consecutivoActual, string mascara)
    {
        // El consecutivo actual es el que se va a retornar
        var consecutivoParaUsar = consecutivoActual;

        // Generar el siguiente consecutivo
        var siguienteConsecutivo = _consecutivoGenerator.GenerarSiguienteConsecutivo(consecutivoActual, mascara);

        // Actualizar en la base de datos
        var actualizado = await ActualizarParametroAsync(CONSECUTIVO_COTIZACION, siguienteConsecutivo);

        if (!actualizado)
        {
            throw new InvalidOperationException("No se pudo actualizar el consecutivo en la base de datos");
        }

        _logger.LogInformation("Consecutivo de cotización generado: {ConsecutivoUsado}, siguiente será: {SiguienteConsecutivo}", 
            consecutivoParaUsar, siguienteConsecutivo);

        return consecutivoParaUsar;
    }

    /// <summary>
    /// Valida que la configuración de consecutivos sea correcta
    /// </summary>
    public async Task<bool> ValidarConfiguracionConsecutivosAsync()
    {
        try
        {
            var mascara = await ObtenerValorParametroAsync(MASCARA_CONSECUTIVO_COTIZACION);
            var consecutivo = await ObtenerValorParametroAsync(CONSECUTIVO_COTIZACION);

            if (string.IsNullOrEmpty(mascara))
            {
                _logger.LogError("Parámetro MASCARA_CONSECUTIVO_COTIZACION no configurado");
                return false;
            }

            if (string.IsNullOrEmpty(consecutivo))
            {
                _logger.LogError("Parámetro CONSECUTIVO_COTIZACION no configurado");
                return false;
            }

            // Validar máscara
            if (!_consecutivoGenerator.ValidarMascara(mascara))
            {
                _logger.LogError("Máscara de consecutivo inválida: {Mascara}", mascara);
                return false;
            }

            // Validar que el consecutivo actual coincida con la máscara
            if (!_consecutivoGenerator.ValidarConsecutivo(consecutivo, mascara))
            {
                _logger.LogError("Consecutivo {Consecutivo} no coincide con máscara {Mascara}", consecutivo, mascara);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar configuración de consecutivos");
            return false;
        }
    }

    /// <summary>
    /// Obtiene todos los parámetros de una categoría específica
    /// </summary>
    public async Task<List<Parametros>> ObtenerParametrosPorCategoriaAsync(string categoria)
    {
        return await _context.Parametros  // Usar tabla Parametros existente
            .Where(p => p.Categoria == categoria)
            .OrderBy(p => p.Codigo)
            .ToListAsync();
    }

    /// <summary>
    /// Inicializa parámetros por defecto si no existen
    /// </summary>
    public async Task<bool> InicializarParametrosDefectoAsync()
    {
        try
        {
            // Verificar si ya existen parámetros
            var existenParametros = await _context.Parametros.AnyAsync();  // Usar tabla Parametros existente
            if (existenParametros)
            {
                _logger.LogInformation("Los parámetros del sistema ya están inicializados");
                return true;
            }

            // TODO: Agregar lógica para crear parámetros por defecto
            // Esto se puede ejecutar automáticamente en migraciones o startup

            _logger.LogWarning("Parámetros del sistema no inicializados. Ejecutar script de seed.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar parámetros por defecto");
            return false;
        }
    }
}