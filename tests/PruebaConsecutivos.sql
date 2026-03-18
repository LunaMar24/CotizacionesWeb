-- ============================================
-- Script de prueba del sistema de consecutivos
-- ACTUALIZADO: Para tabla Parametros expandida
-- ============================================

USE CotizacionesWeb;
GO

PRINT '==============================================';
PRINT 'PRUEBAS DEL SISTEMA DE CONSECUTIVOS';
PRINT '==============================================';

-- 1. Verificar parámetros iniciales
PRINT '';
PRINT '1. PARÁMETROS DE CONSECUTIVOS:';
PRINT '-------------------------------';
SELECT 
    Codigo,
    Valor,
    Descripcion
FROM Parametros
WHERE Categoria = 'Consecutivos'
ORDER BY Codigo;

-- 2. Simular generación de consecutivos
PRINT '';
PRINT '2. SIMULACIÓN DE CONSECUTIVOS:';
PRINT '------------------------------';

DECLARE @Mascara VARCHAR(100) = (SELECT Valor FROM Parametros WHERE Codigo = 'MASCARA_CONSECUTIVO_COTIZACION');
DECLARE @ConsecutivoActual VARCHAR(100) = (SELECT Valor FROM Parametros WHERE Codigo = 'CONSECUTIVO_COTIZACION');

PRINT 'Máscara actual: ' + ISNULL(@Mascara, 'NO CONFIGURADA');
PRINT 'Consecutivo actual: ' + ISNULL(@ConsecutivoActual, 'NO CONFIGURADO');

-- 3. Ejemplos de secuencias válidas
PRINT '';
PRINT '3. EJEMPLOS DE SECUENCIAS:';
PRINT '---------------------------';

-- Simular secuencia COT-9999
WITH SecuenciaSimulada AS (
    SELECT 1 as Orden, 'COT-0001' as Consecutivo
    UNION ALL
    SELECT 2, 'COT-0002'
    UNION ALL
    SELECT 3, 'COT-0009'
    UNION ALL
    SELECT 4, 'COT-0010'
    UNION ALL
    SELECT 5, 'COT-0099'
    UNION ALL
    SELECT 6, 'COT-0100'
    UNION ALL
    SELECT 7, 'COT-9999'
)
SELECT 
    Orden,
    Consecutivo,
    CASE 
        WHEN Consecutivo = 'COT-9999' THEN 'MÁXIMO ALCANZADO'
        ELSE 'VÁLIDO'
    END as Estado
FROM SecuenciaSimulada
ORDER BY Orden;

-- 4. Validar máscaras
PRINT '';
PRINT '4. VALIDACIÓN DE MÁSCARAS:';
PRINT '---------------------------';

WITH MascarasTest AS (
    SELECT 1 as Id, 'COT-9999' as Mascara, 1 as EsValida, 'Máscara estándar'as Descripcion
    UNION ALL
    SELECT 2, 'AAA-999', 1, 'Tres letras + tres números'
    UNION ALL
    SELECT 3, 'A9A-999-AAA', 1, 'Patrón complejo válido'
    UNION ALL
    SELECT 4, 'COT--999', 0, 'INVÁLIDA: separadores consecutivos'
    UNION ALL
    SELECT 5, '-COT-999', 0, 'INVÁLIDA: empieza con separador'
    UNION ALL
    SELECT 6, 'COT-999-', 0, 'INVÁLIDA: termina con separador'
    UNION ALL
    SELECT 7, 'COTX999', 0, 'INVÁLIDA: carácter X no permitido'
)
SELECT 
    Id,
    Mascara,
    CASE WHEN EsValida = 1 THEN '? VÁLIDA' ELSE '? INVÁLIDA' END as Estado,
    Descripcion
FROM MascarasTest
ORDER BY Id;

-- 5. Probar algoritmo de incremento manual (simulación SQL)
PRINT '';
PRINT '5. SIMULACIÓN DE INCREMENTOS:';
PRINT '------------------------------';

-- Función SQL para simular incremento de COT-9999
DECLARE @TestConsecutivo VARCHAR(20) = 'COT-0009';
DECLARE @PartePrefijo VARCHAR(10) = LEFT(@TestConsecutivo, 4); -- COT-
DECLARE @ParteNumero VARCHAR(10) = RIGHT(@TestConsecutivo, 4); -- 0009
DECLARE @NumeroInt INT = CAST(@ParteNumero AS INT); -- 9
DECLARE @SiguienteNumero INT = @NumeroInt + 1; -- 10
DECLARE @SiguienteConsecutivo VARCHAR(20) = @PartePrefijo + RIGHT('0000' + CAST(@SiguienteNumero AS VARCHAR(4)), 4);

SELECT 
    @TestConsecutivo as ConsecutivoActual,
    @SiguienteConsecutivo as SiguienteConsecutivo,
    'Simulación SQL básica' as Metodo;

-- 6. Verificar configuración recomendada
PRINT '';
PRINT '6. CONFIGURACIÓN RECOMENDADA:';
PRINT '------------------------------';

SELECT 
    'CONFIGURACIÓN ACTUAL' as Tipo,
    (SELECT COUNT(*) FROM Parametros WHERE Categoria = 'Consecutivos') as ParametrosConsecutivos,
    (SELECT COUNT(*) FROM Cotizacion) as CotizacionesExistentes,
    (SELECT MAX(RIGHT(CotizacionId, 4)) FROM Cotizacion WHERE CotizacionId LIKE 'COT-%') as UltimoNumeroUsado;

-- Mostrar configuración recomendada basada en datos actuales
DECLARE @UltimoNumero INT = (SELECT ISNULL(MAX(CAST(RIGHT(CotizacionId, 4) AS INT)), 0) FROM Cotizacion WHERE CotizacionId LIKE 'COT-%');
DECLARE @SiguienteRecomendado VARCHAR(20) = 'COT-' + RIGHT('0000' + CAST(@UltimoNumero + 1 AS VARCHAR(4)), 4);

SELECT 
    'RECOMENDACIÓN' as Tipo,
    'COT-9999' as MascaraRecomendada,
    @SiguienteRecomendado as ConsecutivoRecomendado,
    'Compatible con sistema actual' as Justificacion;

PRINT '';
PRINT '==============================================';
PRINT 'PASOS SIGUIENTES:';
PRINT '1. ? Migración aplicada - tabla expandida';
PRINT '2. ? Parámetros iniciales creados';
PRINT '3. ?? Registrar servicios en Program.cs';
PRINT '4. ?? Probar duplicación de cotización';
PRINT '==============================================';