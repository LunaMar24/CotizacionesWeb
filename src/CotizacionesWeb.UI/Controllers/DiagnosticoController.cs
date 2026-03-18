using Microsoft.AspNetCore.Mvc;
using CotizacionesWeb.Infrastructure.Services;

namespace CotizacionesWeb.UI.Controllers;

public class DiagnosticoController : Controller
{
    private readonly ConsecutivoGenerator _consecutivoGenerator;
    private readonly IParametroSistemaService _parametroService;
    private readonly ILogger<DiagnosticoController> _logger;

    public DiagnosticoController(
        ConsecutivoGenerator consecutivoGenerator,
        IParametroSistemaService parametroService,
        ILogger<DiagnosticoController> logger)
    {
        _consecutivoGenerator = consecutivoGenerator;
        _parametroService = parametroService;
        _logger = logger;
    }

    public async Task<IActionResult> TestConsecutivos()
    {
        var resultados = new List<string>();

        try
        {
            // 1. Obtener parámetros actuales
            var mascara = await _parametroService.ObtenerValorParametroAsync("MASCARA_CONSECUTIVO_COTIZACION");
            var consecutivo = await _parametroService.ObtenerValorParametroAsync("CONSECUTIVO_COTIZACION");

            resultados.Add($"Máscara: {mascara}");
            resultados.Add($"Consecutivo: {consecutivo}");

            // 2. Validar máscara
            var mascaraValida = _consecutivoGenerator.ValidarMascara(mascara);
            resultados.Add($"Máscara válida: {mascaraValida}");

            // 3. Validar consecutivo contra máscara
            var consecutivoValido = _consecutivoGenerator.ValidarConsecutivo(consecutivo, mascara);
            resultados.Add($"Consecutivo válido: {consecutivoValido}");

            // 4. Verificar longitudes
            resultados.Add($"Longitud máscara: {mascara?.Length}");
            resultados.Add($"Longitud consecutivo: {consecutivo?.Length}");

            // 5. Analizar carácter por carácter
            if (!string.IsNullOrEmpty(mascara) && !string.IsNullOrEmpty(consecutivo))
            {
                resultados.Add("Análisis carácter por carácter:");
                for (int i = 0; i < Math.Min(mascara.Length, consecutivo.Length); i++)
                {
                    char mascaraChar = mascara[i];
                    char consecutivoChar = consecutivo[i];
                    bool coincide = false;

                    switch (mascaraChar)
                    {
                        case 'A':
                            coincide = char.IsLetter(consecutivoChar);
                            break;
                        case '9':
                            coincide = char.IsDigit(consecutivoChar);
                            break;
                        case '-':
                            coincide = consecutivoChar == '-';
                            break;
                    }

                    resultados.Add($"  Pos {i}: '{mascaraChar}' vs '{consecutivoChar}' = {coincide}");
                }
            }

            // 6. Intentar generar siguiente consecutivo
            if (consecutivoValido)
            {
                var siguiente = _consecutivoGenerator.GenerarSiguienteConsecutivo(consecutivo, mascara);
                resultados.Add($"Siguiente consecutivo: {siguiente}");
            }

            // 7. Probar validación de configuración completa
            var configValida = await _parametroService.ValidarConfiguracionConsecutivosAsync();
            resultados.Add($"Configuración de consecutivos válida: {configValida}");

        }
        catch (Exception ex)
        {
            resultados.Add($"ERROR: {ex.Message}");
            _logger.LogError(ex, "Error en diagnóstico de consecutivos");
        }

        ViewBag.Resultados = resultados;
        return View();
    }

    public async Task<IActionResult> TestDuplicacion()
    {
        var resultados = new List<string>();

        try
        {
            // Probar el siguiente consecutivo directamente
            var nuevoConsecutivo = await _parametroService.ObtenerSiguienteConsecutivoCotizacionAsync();
            resultados.Add($"Nuevo consecutivo generado: {nuevoConsecutivo}");

        }
        catch (Exception ex)
        {
            resultados.Add($"ERROR al generar consecutivo: {ex.Message}");
            if (ex.InnerException != null)
            {
                resultados.Add($"Error interno: {ex.InnerException.Message}");
            }
            _logger.LogError(ex, "Error en test de duplicación");
        }

        ViewBag.Resultados = resultados;
        return View("TestConsecutivos");
    }
}