-- ============================================
-- Script para poblar parámetros del sistema
-- ACTUALIZADO: Para tabla Parametros expandida
-- ============================================

USE CotizacionesWeb;
GO

-- Verificar si existe la tabla de parámetros expandida
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Codigo' AND object_id = OBJECT_ID('Parametros'))
BEGIN
    PRINT 'ERROR: La tabla Parametros no ha sido expandida. Ejecutar migración primero.';
    RETURN;
END

-- Limpiar parámetros existentes (solo para desarrollo/testing)
-- DELETE FROM Parametros;

-- ============================================
-- PARÁMETROS GENERALES DEL SISTEMA
-- ============================================

INSERT INTO Parametros (Codigo, Descripcion, Valor, TipoValor, Categoria, EsModificable, ValorPorDefecto) VALUES
-- Parámetros financieros (usando enum: N=Decimal, S=Texto)
('TASA_IMPUESTO', 'Tasa de impuesto por defecto (%)', '19', 'N', 'Financiero', 1, '19'),
('MONEDA_DEFECTO', 'Moneda por defecto del sistema', 'CLP', 'S', 'Financiero', 1, 'CLP'),
('TIPO_CAMBIO_BASE', 'Tipo de cambio base USD/CLP', '850.50', 'N', 'Financiero', 1, '800.00'),

-- Parámetros de notificaciones
('EMAIL_NOTIFICACIONES', 'Email para notificaciones del sistema', 'admin@cotizaciones.com', 'S', 'Notificaciones', 1, 'admin@sistema.com'),
('TIEMPO_EXPIRACION_COTIZACION', 'Días para expiración de cotización', '30', 'E', 'Negocio', 1, '30'),

-- Parámetros de formato y presentación
('FORMATO_COTIZACION', 'Formato de exportación por defecto', 'PDF', 'S', 'Formato', 1, 'PDF'),

-- ============================================
-- PARÁMETROS DE CONSECUTIVOS
-- ============================================

-- Máscara para generación de IDs de cotización
('MASCARA_CONSECUTIVO_COTIZACION', 'Máscara para generar IDs de cotización (A=letra, 9=número, -=separador)', 'COT-9999', 'S', 'Consecutivos', 1, 'COT-9999'),

-- Consecutivo actual de cotizaciones
('CONSECUTIVO_COTIZACION', 'Consecutivo actual para nuevas cotizaciones', 'COT-0007', 'S', 'Consecutivos', 1, 'COT-0001'),

-- ============================================
-- PARÁMETROS DE INTEGRACIÓN
-- ============================================

-- Configuración ERP
('ERP_ENDPOINT', 'URL del endpoint del ERP', 'https://api.erp.empresa.com', 'S', 'Integracion', 1, 'https://api.erp.local'),
('ERP_TIMEOUT_SEGUNDOS', 'Timeout para conexiones al ERP (segundos)', '30', 'E', 'Integracion', 1, '30'),

-- Configuración HubSpot
('HUBSPOT_API_KEY', 'Clave API de HubSpot', '', 'S', 'Integracion', 0, ''),
('HUBSPOT_SYNC_ENABLED', 'Sincronización con HubSpot habilitada (S/N)', 'N', 'B', 'Integracion', 1, 'N'),

-- ============================================
-- PARÁMETROS DE SEGURIDAD
-- ============================================

('SESSION_TIMEOUT_MINUTOS', 'Timeout de sesión en minutos', '60', 'E', 'Seguridad', 1, '60'),
('MAX_INTENTOS_LOGIN', 'Máximo intentos de login fallidos', '5', 'E', 'Seguridad', 1, '5'),
('BLOQUEO_CUENTA_MINUTOS', 'Minutos de bloqueo tras intentos fallidos', '15', 'E', 'Seguridad', 1, '15'),

-- ============================================
-- PARÁMETROS DE ARCHIVOS Y ALMACENAMIENTO
-- ============================================

('MAX_SIZE_ARCHIVO_MB', 'Tamaño máximo de archivo en MB', '10', 'E', 'Archivos', 1, '10'),
('FORMATOS_PERMITIDOS', 'Formatos de archivo permitidos (separados por coma)', 'pdf,doc,docx,xls,xlsx,jpg,png', 'S', 'Archivos', 1, 'pdf,doc,xls,jpg'),
('RUTA_ALMACENAMIENTO', 'Ruta base para almacenar archivos', '/uploads/cotizaciones/', 'S', 'Archivos', 1, '/uploads/'),

-- ============================================
-- PARÁMETROS DE REPORTES
-- ============================================

('LOGO_EMPRESA_URL', 'URL del logo de la empresa para reportes', '/images/logo-empresa.png', 'S', 'Reportes', 1, '/images/logo.png'),
('NOMBRE_EMPRESA', 'Nombre de la empresa para reportes', 'Mi Empresa S.A.', 'S', 'Reportes', 1, 'Empresa'),
('DIRECCION_EMPRESA', 'Dirección de la empresa', 'Av. Principal 123, Santiago', 'S', 'Reportes', 1, 'Dirección no configurada'),
('TELEFONO_EMPRESA', 'Teléfono de contacto empresa', '+56 2 2345 6789', 'S', 'Reportes', 1, '+56 2 0000 0000'),

-- ============================================
-- PARÁMETROS DE WORKFLOW
-- ============================================

('APROBACION_AUTOMATICA_MONTO', 'Monto máximo para aprobación automática', '0', 'N', 'Workflow', 1, '0'),
('REQUIERE_APROBACION_DESCUENTO', 'Porcentaje de descuento que requiere aprobación', '10', 'N', 'Workflow', 1, '15'),
('DIAS_VIGENCIA_COTIZACION', 'Días de vigencia por defecto de cotización', '15', 'E', 'Workflow', 1, '15');

GO

-- Verificar parámetros creados
PRINT '==============================================';
PRINT 'PARÁMETROS DEL SISTEMA CREADOS EXITOSAMENTE';
PRINT '==============================================';

SELECT 
    ParametroId,
    Codigo,
    Descripcion,
    Valor,
    TipoValor,
    Categoria,
    CASE WHEN EsModificable = 1 THEN 'Sí' ELSE 'No' END as Modificable
FROM Parametros
WHERE Codigo NOT LIKE 'PARAM_%'  -- Excluir parámetros legacy temporales
ORDER BY Categoria, Codigo;

-- Mostrar resumen por categoría
PRINT '';
PRINT 'RESUMEN POR CATEGORÍA:';
PRINT '----------------------';
SELECT 
    Categoria,
    COUNT(*) as TotalParametros,
    SUM(CASE WHEN EsModificable = 1 THEN 1 ELSE 0 END) as Modificables,
    SUM(CASE WHEN EsModificable = 0 THEN 1 ELSE 0 END) as NoModificables
FROM Parametros
WHERE Codigo NOT LIKE 'PARAM_%'
GROUP BY Categoria
ORDER BY Categoria;

PRINT '';
PRINT 'PARÁMETROS CRÍTICOS DE CONSECUTIVOS:';
PRINT '------------------------------------';
SELECT 
    Codigo,
    Valor,
    Descripcion
FROM Parametros
WHERE Categoria = 'Consecutivos'
ORDER BY Codigo;

PRINT '';
PRINT '==============================================';
PRINT 'NOTAS IMPORTANTES:';
PRINT '- MASCARA_CONSECUTIVO_COTIZACION: A=letra, 9=número, -=separador';
PRINT '- CONSECUTIVO_COTIZACION: Se debe actualizar automáticamente';
PRINT '- Parámetros marcados como no modificables requieren migración';
PRINT '- Configurar CONSECUTIVO_COTIZACION según último ID existente';
PRINT '==============================================';