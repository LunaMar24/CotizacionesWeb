-- ============================================
-- Script para poblar parámetros del sistema
-- ACTUALIZADO: incluye EsSensitivo
-- ============================================

USE CotizacionesWeb;
GO

-- Verificar si existe la tabla de parámetros expandida
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Codigo' AND object_id = OBJECT_ID('Parametros'))
BEGIN
    PRINT 'ERROR: La tabla Parametros no ha sido expandida. Ejecutar migración primero.';
    RETURN;
END

-- Verificar si existe la columna EsSensitivo
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'EsSensitivo' AND object_id = OBJECT_ID('Parametros'))
BEGIN
    PRINT 'ERROR: La columna EsSensitivo no existe en Parametros. Ejecutar migración primero.';
    RETURN;
END

-- Verificar si ya existen parámetros
IF EXISTS (SELECT 1 FROM Parametros)
BEGIN
    PRINT 'ADVERTENCIA: Ya existen parámetros en la base de datos.';
    PRINT 'Si desea recrearlos, primero ejecute: DELETE FROM Parametros;';
    PRINT 'Saltando inserción para evitar duplicados.';
    RETURN;
END

-- ============================================
-- PARÁMETROS DEL SISTEMA
-- ============================================

INSERT INTO Parametros
(
    Codigo,
    Descripcion,
    Valor,
    TipoValor,
    Categoria,
    EsModificable,
    ValorPorDefecto,
    Notas,
    EsSensitivo
)
VALUES

-- ============================================
-- PARÁMETROS FINANCIEROS
-- ============================================
('TASA_IMPUESTO', 'Tasa de impuesto por defecto (%)', '19', 'N', 'Financiero', 1, '19', 'Impuesto aplicado a cotizaciones', 0),
('MONEDA_DEFECTO', 'Moneda por defecto del sistema', 'CRC', 'S', 'Financiero', 1, 'CRC', 'Código ISO 4217 de 3 letras', 0),
('TIPO_CAMBIO_BASE', 'Tipo de cambio base USD/CRC', '540.00', 'N', 'Financiero', 1, '540.00', 'Tipo de cambio de referencia', 0),

-- ============================================
-- PARÁMETROS DE CONSECUTIVOS
-- ============================================
('MASCARA_CONSECUTIVO_COTIZACION', 'Máscara para generar IDs de cotización', 'COT-9999', 'S', 'Consecutivos', 1, 'COT-9999', 'A=letra, 9=número, -=separador', 0),
('CONSECUTIVO_COTIZACION', 'Consecutivo actual para nuevas cotizaciones', 'COT-0007', 'S', 'Consecutivos', 1, 'COT-0001', 'Se actualiza automáticamente', 0),

-- ============================================
-- PARÁMETROS DE INTEGRACIÓN HUBSPOT
-- ============================================
('HUBSPOT_ENABLED', 'Activa integración con HubSpot', '0', 'B', 'Integracion_HubSpot', 1, '0', 'Activar solo después de configurar token', 0),
('HUBSPOT_AUTH_TYPE', 'Tipo de autenticación HubSpot', 'PRIVATE_APP', 'S', 'Integracion_HubSpot', 1, 'PRIVATE_APP', 'PRIVATE_APP o OAUTH', 0),
('HUBSPOT_API_BASE_URL', 'URL base del API de HubSpot', 'https://api.hubapi.com', 'S', 'Integracion_HubSpot', 1, 'https://api.hubapi.com', NULL, 0),
('HUBSPOT_ACCESS_TOKEN', 'Token de acceso HubSpot', '', 'S', 'Integracion_HubSpot', 0, '', 'Una vez asignado, no se podrá modificar', 1),
('HUBSPOT_OBJECT_CONTACT', 'Objeto contacto HubSpot', 'contacts', 'S', 'Integracion_HubSpot', 1, 'contacts', NULL, 0),
('HUBSPOT_OBJECT_COMPANY', 'Objeto compañía HubSpot', 'companies', 'S', 'Integracion_HubSpot', 1, 'companies', NULL, 0),
('HUBSPOT_PAGE_SIZE', 'Cantidad máxima por consulta', '100', 'N', 'Integracion_HubSpot', 1, '100', 'Límite API: 100', 0),
('HUBSPOT_TIMEOUT_SECONDS', 'Timeout en segundos', '30', 'N', 'Integracion_HubSpot', 1, '30', NULL, 0),
('HUBSPOT_RETRY_COUNT', 'Cantidad de reintentos', '3', 'N', 'Integracion_HubSpot', 1, '3', NULL, 0),
('HUBSPOT_ACCOUNT_NAME', 'Nombre de la cuenta HubSpot', 'Cuenta Principal', 'S', 'Integracion_HubSpot', 1, 'Mi Cuenta', NULL, 0),
('HUBSPOT_CONTACT_SEARCH_FIELDS', 'Campos de búsqueda contacto', 'email,firstname,lastname', 'S', 'Integracion_HubSpot', 1, 'email', 'Campos de búsqueda de contactos en HubSpot. Separados por coma. Máximo 4 campos.', 0),
('HUBSPOT_COMPANY_SEARCH_FIELDS', 'Campos de búsqueda compañía', 'name,domain', 'S', 'Integracion_HubSpot', 1, 'name', 'Campos de búsqueda de compañías en HubSpot. Separados por coma. Máximo 4 campos.', 0),
('HUBSPOT_CONTACTO_CAMPO_CLIENTE_ERP', 'Nombre del campo en HubSpot (Contacto) que almacena el código de cliente ERP', 'cliente_erp', 'S', 'Integracion_HubSpot', 1, 'cliente_erp', 'Debe coincidir con el nombre interno de la propiedad en HubSpot para contactos', 0),
('HUBSPOT_EMPRESA_CAMPO_CLIENTE_ERP', 'Nombre del campo en HubSpot (Empresa) que almacena el código de cliente ERP', 'cliente_erp', 'S', 'Integracion_HubSpot', 1, 'cliente_erp', 'Debe coincidir con el nombre interno de la propiedad en HubSpot para empresas', 0),

-- ============================================
-- PARÁMETROS DE INTEGRACIÓN ERP
-- ============================================
('ERP_ENABLED', 'Activa integración con ERP', 'N', 'B', 'Integracion_ERP', 1, 'N', 'Activar solo después de configurar conexion al ERP', 0),
('ERP_NIVELPRECIO_LOCAL', 'Nivel Precio para moneda local', 'ND-LOCAL', 'S', 'Integracion_ERP', 1, 'ND-LOCAL', NULL, 0),
('ERP_NIVELPRECIO_DOLAR', 'Nivel Precio para moneda dólar', 'ND-DOLAR', 'S', 'Integracion_ERP', 1, 'ND-DOLAR', NULL, 0),
('ERP_CIA', 'Compañía a utilizar en el ERP', '', 'S', 'Integracion_ERP', 1, '', NULL, 0),
('ERP_USAR_IMPUESTOS', 'Calcular impuestos basado en el ERP', 'N', 'B', 'Integracion_ERP', 1, 'N', NULL, 0),
('ERP_CONDICION_PAGO', 'Condición de pago para pedidos ERP', 'CONT', 'S', 'Integracion_ERP', 1, '', NULL, 0),
('ERP_BODEGA', 'Bodega por defecto para pedidos ERP', '01', 'S', 'Integracion_ERP', 1, '', NULL, 0),
('ERP_USUARIO', 'Usuario ERP para creación de pedidos', 'SA', 'S', 'Integracion_ERP', 1, 'SA', NULL, 0),
('ERP_ACTIVIDAD_COMERCIAL', 'Actividad comercial para pedidos ERP', '751401', 'S', 'Integracion_ERP', 1, '', NULL, 0),

-- ============================================
-- PARÁMETROS DE NOTIFICACIONES
-- ============================================
('NOTIFICACIONES_COTIZACIONES_ENABLED', 'Activa o desactiva las notificaciones automáticas de cotizaciones', 'N', 'B', 'Notificaciones', 1, 'N', 'Valores esperados: S/N', 0),
('EMAIL_NOTIFICACIONES_PEND_APROBAR', 'Email para notificaciones por cotizaciones pendientes de aprobación', 'admin@cotizaciones.com', 'S', 'Notificaciones', 1, 'admin@cotizaciones.com', 'Correo destino para avisos de cotizaciones en estado Pendiente de Aprobación', 0),
('DIAS_NOTIFICACION_ENVIADAS', 'Cantidad de días para notificar cotizaciones enviadas sin respuesta', '3', 'N', 'Notificaciones', 1, '5', 'Cantidad de días a partir de la fecha de envío para generar notificación de seguimiento', 0),
('NOTIFICACIONES_FRECUENCIA_MINUTOS', 'Frecuencia en minutos para ejecución del proceso de notificaciones', '10', 'N', 'Notificaciones', 1, '10', 'Si cambia este valor, el proceso de notificaciones debe usar la nueva frecuencia en la siguiente iteración', 0),

-- ============================================
-- PARÁMETROS DE SEGURIDAD
-- ============================================
('SESSION_TIMEOUT_MINUTOS', 'Timeout de sesión en minutos', '60', 'N', 'Seguridad', 1, '60', NULL, 0),
('MAX_INTENTOS_LOGIN', 'Máximo intentos de login fallidos', '5', 'N', 'Seguridad', 1, '5', NULL, 0),
('BLOQUEO_CUENTA_MINUTOS', 'Minutos de bloqueo tras intentos fallidos', '15', 'N', 'Seguridad', 1, '15', NULL, 0),

-- ============================================
-- PARÁMETROS DE ARCHIVOS
-- ============================================
('MAX_SIZE_ARCHIVO_MB', 'Tamaño máximo de archivo en MB', '10', 'N', 'Archivos', 1, '10', NULL, 0),
('FORMATOS_PERMITIDOS', 'Formatos de archivo permitidos (separados por coma)', 'pdf,doc,docx,xls,xlsx,jpg,png', 'S', 'Archivos', 1, 'pdf,doc,xls,jpg', NULL, 0),
('RUTA_ALMACENAMIENTO', 'Ruta base para almacenar archivos', '/uploads/cotizaciones/', 'S', 'Archivos', 1, '/uploads/', NULL, 0),

-- ============================================
-- PARÁMETROS DE REPORTES
-- ============================================
('LOGO_EMPRESA_URL', 'URL del logo de la empresa para reportes', '/images/logo-empresa.png', 'S', 'Reportes', 1, '/images/logo.png', NULL, 0),
('NOMBRE_EMPRESA', 'Nombre de la empresa para reportes', 'Mi Empresa S.A.', 'S', 'Reportes', 1, 'Empresa', NULL, 0),
('DIRECCION_EMPRESA', 'Dirección de la empresa', 'Av. Principal 123, Santiago', 'S', 'Reportes', 1, 'Dirección no configurada', NULL, 0),
('TELEFONO_EMPRESA', 'Teléfono de contacto empresa', '+56 2 2345 6789', 'S', 'Reportes', 1, '+56 2 0000 0000', NULL, 0),

-- ============================================
-- PARÁMETROS DE WORKFLOW
-- ============================================
('APROBACION_AUTOMATICA_CRC_MONTO', 'Monto máximo en colones para aprobación de cotizaciones automáticas', '0', 'N', 'Workflow', 1, '0', 'Si es 0, no hay aprobación automática', 0),
('APROBACION_AUTOMATICA_DOL_MONTO', 'Monto máximo en dólares para aprobación de cotizaciones automáticas', '0', 'N', 'Workflow', 1, '0', 'Si es 0, no hay aprobación automática', 0),
('REQUIERE_APROBACION_DESCUENTO', 'Porcentaje de descuento que requiere aprobación', '10', 'N', 'Workflow', 1, '15', NULL, 0),
('DIAS_VIGENCIA_COTIZACION', 'Días de vigencia por defecto de cotización', '15', 'N', 'Workflow', 1, '15', NULL, 0),

-- ============================================
-- PARÁMETROS PLANTILLA COTIZACION
-- ============================================
('FORMATO_COTIZACION', 'Formato de exportación por defecto', 'PDF', 'S', 'Plantilla Cotización', 1, 'PDF', 'Valores: PDF, EXCEL, WORD', 0),
('COT_VIGENCIA',  'Vigencia de la cotización (ej: 2 semanas)', '2 semanas', 'S', 'Plantilla Cotización', 1, '2 semanas', 'Texto mostrado en la sección de vigencia de la propuesta', 0),
('COT_CONDICIONES_PAGO', 'Condiciones de pago específicas para la cotización', '', 'S', 'Plantilla Cotización', 1, '', 'Condiciones de pago que se muestran en la propuesta', 0),
('COT_NOTAS_COMERCIALES', 'Notas o comentarios comerciales adicionales de la propuesta', '', 'S', 'Plantilla Cotización', 1, '', 'Texto opcional para incluir notas comerciales en la cotización', 0),
('COT_TITULO_DETALLE', 'Título de la sección de detalle de la propuesta', 'Detalle de la propuesta', 'S', 'Plantilla Cotización', 1, 'Detalle de la propuesta', 'Permite cambiar el título del bloque de detalle en la cotización', 0),
('COT_TITULO_RESUMEN', 'Título de la sección de resumen de la cotización', 'Resumen', 'S', 'Plantilla Cotización', 1, 'Resumen', 'Permite cambiar el título del bloque de totales', 0),

-- ============================================
-- PARÁMETROS DE NEGOCIO
-- ============================================
('TIEMPO_EXPIRACION_COTIZACION', 'Días para expiración de cotización', '30', 'N', 'Negocio', 1, '30', 'Días hasta que expira una cotización enviada', 0);

GO

-- ============================================
-- VERIFICACIÓN
-- ============================================

PRINT '==============================================';
PRINT 'PARÁMETROS DEL SISTEMA CREADOS EXITOSAMENTE';
PRINT '==============================================';
PRINT '';

SELECT 
    ParametroId,
    Codigo,
    LEFT(Descripcion, 50) AS Descripcion,
    Valor,
    TipoValor,
    Categoria,
    CASE WHEN EsModificable = 1 THEN 'Sí' ELSE 'No' END AS Modificable,
    CASE WHEN EsSensitivo = 1 THEN 'Sí' ELSE 'No' END AS Sensitivo
FROM Parametros
ORDER BY Categoria, Codigo;

-- Mostrar resumen por categoría
PRINT '';
PRINT 'RESUMEN POR CATEGORÍA:';
PRINT '----------------------';

SELECT 
    Categoria,
    COUNT(*) AS Total,
    SUM(CASE WHEN EsModificable = 1 THEN 1 ELSE 0 END) AS Modificables,
    SUM(CASE WHEN EsModificable = 0 THEN 1 ELSE 0 END) AS NoModificables,
    SUM(CASE WHEN EsSensitivo = 1 THEN 1 ELSE 0 END) AS Sensitivos
FROM Parametros
GROUP BY Categoria
ORDER BY Categoria;

PRINT '';
PRINT '==============================================';
PRINT 'CATEGORÍAS ACTIVAS:';
PRINT '==============================================';
PRINT '  • Financiero (3 parámetros)';
PRINT '  • Consecutivos (2 parámetros)';
PRINT '  • Integracion_HubSpot (13 parámetros)';
PRINT '  • Integracion_ERP (9 parámetros)';
PRINT '  • Notificaciones (4 parámetros)';
PRINT '  • Seguridad (3 parámetros)';
PRINT '  • Archivos (3 parámetros)';
PRINT '  • Reportes (4 parámetros)';
PRINT '  • Workflow (4 parámetros)';
PRINT '  • Plantilla Cotización (6 parámetros)';
PRINT '  • Negocio (1 parámetro)';
PRINT '';
PRINT 'TOTAL: 52 parámetros activos';
PRINT '==============================================';
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
PRINT '==============================================';
PRINT '• MASCARA_CONSECUTIVO_COTIZACION:';
PRINT '  - A = letra, 9 = número, - = separador';
PRINT '';
PRINT '• CONSECUTIVO_COTIZACION:';
PRINT '  - Se actualiza automáticamente al crear cotizaciones';
PRINT '  - Configurar según último ID existente';
PRINT '';
PRINT '• HUBSPOT_ACCESS_TOKEN:';
PRINT '  - Debe configurarse con el token real';
PRINT '  - No modificable desde UI por seguridad';
PRINT '  - Marcado como sensitivo';
PRINT '';
PRINT '• HUBSPOT_ENABLED:';
PRINT '  - Mantener en 0 hasta configurar token';
PRINT '  - Activar a 1 cuando esté listo';
PRINT '';
PRINT '• TIPOS DE VALOR:';
PRINT '  - S = Texto (String)';
PRINT '  - N = Decimal/Numérico';
PRINT '  - B = Booleano (0/1)';
PRINT '  - D = Fecha (Date)';
PRINT '';
PRINT '• CATEGORÍAS ELIMINADAS:';
PRINT '  - Legacy (obsoleta)';
PRINT '  - Integracion genérica (consolidada en específicas)';
PRINT '==============================================';

GO