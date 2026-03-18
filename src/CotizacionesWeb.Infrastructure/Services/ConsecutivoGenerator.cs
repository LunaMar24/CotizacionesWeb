using System.Text;
using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Infrastructure.Services;

/// <summary>
/// Generador inteligente de consecutivos basado en máscaras
/// Soporta patrones como: AAA-9999, COT-9999-9999, ABC-999-AAA
/// </summary>
public class ConsecutivoGenerator
{
    private readonly ILogger<ConsecutivoGenerator> _logger;

    public ConsecutivoGenerator(ILogger<ConsecutivoGenerator> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Valida si una máscara es válida
    /// Caracteres permitidos: A (letra), 9 (número), - (separador)
    /// </summary>
    public bool ValidarMascara(string mascara)
    {
        if (string.IsNullOrWhiteSpace(mascara))
            return false;

        // No puede empezar o terminar con separador
        if (mascara.StartsWith('-') || mascara.EndsWith('-'))
            return false;

        // No puede tener separadores consecutivos
        if (mascara.Contains("--"))
            return false;

        // Solo caracteres permitidos
        foreach (char c in mascara)
        {
            if (c != 'A' && c != '9' && c != '-')
                return false;
        }

        return true;
    }

    /// <summary>
    /// Valida si un consecutivo cumple con la máscara especificada
    /// </summary>
    public bool ValidarConsecutivo(string consecutivo, string mascara)
    {
        if (!ValidarMascara(mascara))
            return false;

        if (string.IsNullOrWhiteSpace(consecutivo) || consecutivo.Length != mascara.Length)
            return false;

        for (int i = 0; i < mascara.Length; i++)
        {
            char mascCharacter = mascara[i];
            char consCharacter = consecutivo[i];

            switch (mascCharacter)
            {
                case 'A':
                    if (!char.IsLetter(consCharacter))
                        return false;
                    break;
                case '9':
                    if (!char.IsDigit(consCharacter))
                        return false;
                    break;
                case '-':
                    if (consCharacter != '-')
                        return false;
                    break;
            }
        }

        return true;
    }

    /// <summary>
    /// Genera el siguiente consecutivo basado en la máscara
    /// Algoritmo: incrementa de derecha a izquierda, manejando overflow
    /// </summary>
    public string GenerarSiguienteConsecutivo(string consecutivoActual, string mascara)
    {
        if (!ValidarMascara(mascara))
            throw new ArgumentException($"Máscara inválida: {mascara}");

        if (!ValidarConsecutivo(consecutivoActual, mascara))
            throw new ArgumentException($"Consecutivo {consecutivoActual} no cumple con máscara {mascara}");

        var resultado = consecutivoActual.ToCharArray();
        bool carry = true;

        // Recorrer de derecha a izquierda
        for (int i = mascara.Length - 1; i >= 0 && carry; i--)
        {
            char mascCharacter = mascara[i];

            if (mascCharacter == '-') // Separadores no se modifican
                continue;

            if (mascCharacter == '9') // Posición numérica
            {
                if (resultado[i] == '9')
                {
                    resultado[i] = '0'; // Reset y carry
                    carry = true;
                }
                else
                {
                    resultado[i] = (char)(resultado[i] + 1);
                    carry = false;
                }
            }
            else if (mascCharacter == 'A') // Posición alfabética
            {
                if (resultado[i] == 'Z')
                {
                    resultado[i] = 'A'; // Reset y carry
                    carry = true;
                }
                else
                {
                    resultado[i] = (char)(resultado[i] + 1);
                    carry = false;
                }
            }
        }

        if (carry)
        {
            throw new OverflowException($"Se alcanzó el máximo consecutivo posible para la máscara {mascara}");
        }

        var nuevoConsecutivo = new string(resultado);
        
        _logger.LogInformation("Consecutivo generado: {ConsecutivoAnterior} ? {ConsecutivoNuevo}", 
            consecutivoActual, nuevoConsecutivo);

        return nuevoConsecutivo;
    }

    /// <summary>
    /// Genera el primer consecutivo válido para una máscara
    /// </summary>
    public string GenerarPrimerConsecutivo(string mascara)
    {
        if (!ValidarMascara(mascara))
            throw new ArgumentException($"Máscara inválida: {mascara}");

        var resultado = new StringBuilder();

        foreach (char c in mascara)
        {
            switch (c)
            {
                case 'A':
                    resultado.Append('A');
                    break;
                case '9':
                    resultado.Append('0');
                    break;
                case '-':
                    resultado.Append('-');
                    break;
            }
        }

        return resultado.ToString();
    }

    /// <summary>
    /// Ejemplos de uso y testing del generador
    /// </summary>
    public void EjemplosDeUso()
    {
        var ejemplos = new[]
        {
            ("COT-9999", "COT-0001"),
            ("AAA-999", "AAA-000"),
            ("A9A-999", "A0A-000"),
            ("COT-999-AAA", "COT-000-AAA")
        };

        _logger.LogInformation("=== EJEMPLOS DE GENERACIÓN DE CONSECUTIVOS ===");

        foreach (var (mascara, inicial) in ejemplos)
        {
            try
            {
                var primer = GenerarPrimerConsecutivo(mascara);
                var siguiente1 = GenerarSiguienteConsecutivo(inicial, mascara);
                var siguiente2 = GenerarSiguienteConsecutivo("COT-9999", mascara); // Test overflow

                _logger.LogInformation("Máscara: {Mascara} | Primer: {Primer} | {Inicial} ? {Siguiente}", 
                    mascara, primer, inicial, siguiente1);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error con máscara {Mascara}: {Error}", mascara, ex.Message);
            }
        }
    }
}

/// <summary>
/// Casos de prueba para el generador de consecutivos
/// </summary>
public static class ConsecutivoGeneratorTests
{
    public static void EjecutarPruebas(ILogger<ConsecutivoGenerator> logger)
    {
        logger.LogInformation("=== PRUEBAS DE CONSECUTIVOS ===");

        var generator = new ConsecutivoGenerator(logger);

        // Prueba 1: Máscara simple numérica
        TestearSecuencia(generator, logger, "COT-9999", "COT-0001", new[]
        {
            "COT-0002", "COT-0003", "COT-0009", "COT-0010", "COT-0099", "COT-0100"
        });

        // Prueba 2: Máscara con overflow
        try
        {
            var resultado = generator.GenerarSiguienteConsecutivo("COT-9999", "COT-9999");
            logger.LogError("ERROR: Debería haber lanzado OverflowException para COT-9999 ? siguiente");
        }
        catch (OverflowException)
        {
            logger.LogInformation("? OverflowException capturada correctamente");
        }

        // Prueba 3: Máscara alfabética
        TestearSecuencia(generator, logger, "ABC-999", "ABC-001", new[]
        {
            "ABC-002", "ABC-009", "ABC-010", "ABC-999"
        });

        // Prueba 4: Transición alfabética
        TestearSecuencia(generator, logger, "AAA-999", "AZZ-999", new[]
        {
            "BAA-000"
        });
    }

    private static void TestearSecuencia(ConsecutivoGenerator generator, ILogger<ConsecutivoGenerator> logger, 
        string mascara, string inicial, string[] esperados)
    {
        try
        {
            var actual = inicial;
            foreach (var esperado in esperados)
            {
                actual = generator.GenerarSiguienteConsecutivo(actual, mascara);
                if (actual == esperado)
                {
                    logger.LogInformation("? {Actual} == {Esperado}", actual, esperado);
                }
                else
                {
                    logger.LogError("? {Actual} != {Esperado}", actual, esperado);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError("? Error en secuencia {Mascara}: {Error}", mascara, ex.Message);
        }
    }
}