-- ============================================
-- Script para insertar datos de prueba de cotizaciones
-- ACTUALIZADO PARA ESTRUCTURA CORREGIDA CON FOREIGN KEYS
-- ============================================

-- PASO 1: Verificar que existe usuario sistema (ID = 1) para foreign keys
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE UsuarioId = 1)
BEGIN
    PRINT 'ERROR: Debe ejecutar CrearUsuarioAdmin.sql primero para crear usuario sistema';
    RETURN;
END

-- PASO 2: Insertar Parámetros de prueba (SIN auditoría ni FK según modelo)
INSERT INTO Parametros (Descripcion, Valor, Tipo) VALUES
('TasaImpuesto', '13', 'N'),
('MonedaDefault', 'CRC', 'S'),
('TiempoExpiracionCotizacion', '30', 'N'),
('TipoCambioBase', '520.50', 'N'),
('EmailNotificaciones', 'admin@cotizaciones.com', 'S'),
('FormatoCotizacion', 'PDF', 'S');

-- PASO 3: Insertar Interesados de prueba (CON FK de auditoría)
INSERT INTO Interesado (HubspotObjectId, HubspotObjectType, TipoInteresado, Activo, FechaUltSync, CreatedAt, CreatedBy) VALUES
('12345', 'contact', 'P', 1, NULL, GETDATE(), 1),
('67890', 'company', 'E', 1, DATEADD(DAY, -1, GETDATE()), GETDATE(), 1),
('11111', 'contact', 'P', 1, NULL, GETDATE(), 1);

-- Obtener IDs de interesados
DECLARE @InteresadoId1 INT = (SELECT InteresadoId FROM Interesado WHERE HubspotObjectId = '12345');
DECLARE @InteresadoId2 INT = (SELECT InteresadoId FROM Interesado WHERE HubspotObjectId = '67890');
DECLARE @InteresadoId3 INT = (SELECT InteresadoId FROM Interesado WHERE HubspotObjectId = '11111');

-- ============================================
-- COTIZACIÓN 1 (Borrador)
-- ============================================

-- Insertar Cotización 1 (CON FK de auditoría)
INSERT INTO Cotizacion (CotizacionId, InteresadoId, EstadoActual, VersionActual, MontoCotizacion, CreatedAt, CreatedBy) 
VALUES ('COT-0001', @InteresadoId1, 'B', 1, 150000.00, GETDATE(), 1);

-- Insertar Versión 1 de Cotización 1 (CON FK de auditoría)
INSERT INTO CotizacionVersion (CotizacionId, NumeroVersion, FechaVersion, NombreInteresado, EmailInteresado, EmpresaInteresado, SubTotal, Impuesto, Descuento, Total, Moneda, TipoCambio, VersionActual, CreatedAt, CreatedBy) 
VALUES ('COT-0001', 1, GETDATE(), 'Juan Pérez', 'juan.perez@email.com', 'Tech Solutions SA', 132743.36, 17256.64, 0.00, 150000.00, 'CRC', 520.50, 1, GETDATE(), 1);

DECLARE @VersionId1 INT = SCOPE_IDENTITY();

-- Insertar detalles de Versión 1 (CON FK de auditoría)
INSERT INTO DetalleCotizacionVersion (VersionId, ProductoId, Cantidad, PrecioUnitario, Descuento, TotalLinea, CreatedAt, CreatedBy) VALUES
(@VersionId1, 'PROD-001', 10.00, 5000.00, 0.00, 50000.00, GETDATE(), 1),
(@VersionId1, 'PROD-002', 5.00, 10000.00, 5000.00, 45000.00, GETDATE(), 1),
(@VersionId1, 'PROD-003', 3.00, 12581.12, 0.00, 37743.36, GETDATE(), 1);

-- Insertar historial de Versión 1 (CON FK UsuarioEvento)
INSERT INTO HistorialCotizacion (VersionId, TipoEvento, FechaEvento, UsuarioEvento, Comentario) 
VALUES (@VersionId1, 'Creada', GETDATE(), 1, 'Cotización creada inicialmente');

-- ============================================
-- COTIZACIÓN 2 (Enviada con 2 versiones)
-- ============================================

INSERT INTO Cotizacion (CotizacionId, InteresadoId, EstadoActual, VersionActual, MontoCotizacion, FechaEnvio, CreatedAt, CreatedBy) 
VALUES ('COT-0002', @InteresadoId2, 'E', 2, 250000.00, DATEADD(DAY, -1, GETDATE()), DATEADD(DAY, -5, GETDATE()), 1);

-- Insertar Versión 1 de Cotización 2 (histórica)
INSERT INTO CotizacionVersion (CotizacionId, NumeroVersion, FechaVersion, NombreInteresado, EmailInteresado, EmpresaInteresado, SubTotal, Impuesto, Descuento, Total, Moneda, TipoCambio, VersionActual, CreatedAt, CreatedBy) 
VALUES ('COT-0002', 1, DATEADD(DAY, -5, GETDATE()), 'María González', 'maria.gonzalez@empresa.com', 'Innovatech Corp', 176991.15, 23008.85, 0.00, 200000.00, 'CRC', 520.50, 1, DATEADD(DAY, -5, GETDATE()), 1);

DECLARE @VersionId2_1 INT = SCOPE_IDENTITY();

-- Insertar detalles de Versión 1 de Cotización 2
INSERT INTO DetalleCotizacionVersion (VersionId, ProductoId, Cantidad, PrecioUnitario, Descuento, TotalLinea, CreatedAt, CreatedBy) VALUES
(@VersionId2_1, 'PROD-001', 20.00, 5000.00, 0.00, 100000.00, DATEADD(DAY, -5, GETDATE()), 1),
(@VersionId2_1, 'PROD-004', 10.00, 7699.12, 0.00, 76991.15, DATEADD(DAY, -5, GETDATE()), 1);

-- Insertar historial de Versión 1 de Cotización 2
INSERT INTO HistorialCotizacion (VersionId, TipoEvento, FechaEvento, UsuarioEvento, Comentario) 
VALUES (@VersionId2_1, 'Creada', DATEADD(DAY, -5, GETDATE()), 1, 'Cotización creada inicialmente');

-- Insertar Versión 2 de Cotización 2 (actual)
INSERT INTO CotizacionVersion (CotizacionId, NumeroVersion, FechaVersion, NombreInteresado, EmailInteresado, EmpresaInteresado, SubTotal, Impuesto, Descuento, Total, Moneda, TipoCambio, VersionActual, Notas, CreatedAt, CreatedBy) 
VALUES ('COT-0002', 2, DATEADD(DAY, -2, GETDATE()), 'María González', 'maria.gonzalez@empresa.com', 'Innovatech Corp', 221238.94, 28761.06, 0.00, 250000.00, 'CRC', 520.50, 2, 'Cliente solicitó agregar más productos', DATEADD(DAY, -2, GETDATE()), 1);

DECLARE @VersionId2_2 INT = SCOPE_IDENTITY();

-- Insertar detalles de Versión 2 de Cotización 2
INSERT INTO DetalleCotizacionVersion (VersionId, ProductoId, Cantidad, PrecioUnitario, Descuento, TotalLinea, CreatedAt, CreatedBy) VALUES
(@VersionId2_2, 'PROD-001', 25.00, 5000.00, 0.00, 125000.00, DATEADD(DAY, -2, GETDATE()), 1),
(@VersionId2_2, 'PROD-004', 10.00, 7699.12, 0.00, 76991.15, DATEADD(DAY, -2, GETDATE()), 1),
(@VersionId2_2, 'PROD-005', 2.00, 9623.89, 0.00, 19247.79, DATEADD(DAY, -2, GETDATE()), 1);

-- Insertar historial de Versión 2 de Cotización 2
INSERT INTO HistorialCotizacion (VersionId, TipoEvento, FechaEvento, UsuarioEvento, Comentario) VALUES
(@VersionId2_2, 'VersionGenerada', DATEADD(DAY, -2, GETDATE()), 1, 'Nueva versión generada con productos adicionales'),
(@VersionId2_2, 'Enviada', DATEADD(DAY, -1, GETDATE()), 1, 'Cotización enviada al cliente por email');

-- ============================================
-- COTIZACIÓN 3 (Aprobada)
-- ============================================

INSERT INTO Cotizacion (CotizacionId, InteresadoId, EstadoActual, VersionActual, MontoCotizacion, FechaEnvio, FechaAceptacion, CreatedAt, CreatedBy) 
VALUES ('COT-0003', @InteresadoId3, 'A', 1, 75000.00, DATEADD(DAY, -8, GETDATE()), DATEADD(DAY, -6, GETDATE()), DATEADD(DAY, -10, GETDATE()), 1);

-- Insertar Versión 1 de Cotización 3
INSERT INTO CotizacionVersion (CotizacionId, NumeroVersion, FechaVersion, NombreInteresado, EmailInteresado, EmpresaInteresado, SubTotal, Impuesto, Descuento, Total, Moneda, TipoCambio, VersionActual, CreatedAt, CreatedBy) 
VALUES ('COT-0003', 1, DATEADD(DAY, -10, GETDATE()), 'Carlos Ramírez', 'carlos.ramirez@company.com', 'Digital Services', 66371.68, 8628.32, 0.00, 75000.00, 'CRC', 520.50, 1, DATEADD(DAY, -10, GETDATE()), 1);

DECLARE @VersionId3 INT = SCOPE_IDENTITY();

-- Insertar detalles de Versión 1 de Cotización 3
INSERT INTO DetalleCotizacionVersion (VersionId, ProductoId, Cantidad, PrecioUnitario, Descuento, TotalLinea, CreatedAt, CreatedBy) VALUES
(@VersionId3, 'PROD-002', 5.00, 10000.00, 0.00, 50000.00, DATEADD(DAY, -10, GETDATE()), 1),
(@VersionId3, 'PROD-003', 2.00, 8185.84, 0.00, 16371.68, DATEADD(DAY, -10, GETDATE()), 1);

-- Insertar historial de Versión 1 de Cotización 3
INSERT INTO HistorialCotizacion (VersionId, TipoEvento, FechaEvento, UsuarioEvento, Comentario) VALUES
(@VersionId3, 'Creada', DATEADD(DAY, -10, GETDATE()), 1, 'Cotización creada inicialmente'),
(@VersionId3, 'Enviada', DATEADD(DAY, -8, GETDATE()), 1, 'Cotización enviada al cliente'),
(@VersionId3, 'Aprobada', DATEADD(DAY, -6, GETDATE()), 1, 'Cliente aprobó la cotización');

-- ============================================
-- COTIZACIÓN 4 (Rechazada con archivo)
-- ============================================

INSERT INTO Cotizacion (CotizacionId, InteresadoId, EstadoActual, VersionActual, MontoCotizacion, FechaEnvio, FechaRechazo, CreatedAt, CreatedBy) 
VALUES ('COT-0004', @InteresadoId1, 'R', 1, 95000.00, DATEADD(DAY, -12, GETDATE()), DATEADD(DAY, -9, GETDATE()), DATEADD(DAY, -15, GETDATE()), 1);

-- Insertar Versión 1 de Cotización 4
INSERT INTO CotizacionVersion (CotizacionId, NumeroVersion, FechaVersion, NombreInteresado, EmailInteresado, EmpresaInteresado, SubTotal, Impuesto, Descuento, Total, Moneda, TipoCambio, VersionActual, CreatedAt, CreatedBy) 
VALUES ('COT-0004', 1, DATEADD(DAY, -15, GETDATE()), 'Juan Pérez', 'juan.perez@email.com', 'Tech Solutions SA', 84070.79, 10929.21, 0.00, 95000.00, 'CRC', 520.50, 1, DATEADD(DAY, -15, GETDATE()), 1);

DECLARE @VersionId4 INT = SCOPE_IDENTITY();

-- Insertar detalles de Versión 1 de Cotización 4
INSERT INTO DetalleCotizacionVersion (VersionId, ProductoId, Cantidad, PrecioUnitario, Descuento, TotalLinea, CreatedAt, CreatedBy) VALUES
(@VersionId4, 'PROD-001', 8.00, 5000.00, 0.00, 40000.00, DATEADD(DAY, -15, GETDATE()), 1),
(@VersionId4, 'PROD-006', 6.00, 7345.13, 0.00, 44070.79, DATEADD(DAY, -15, GETDATE()), 1);

-- Insertar historial de Versión 1 de Cotización 4
INSERT INTO HistorialCotizacion (VersionId, TipoEvento, FechaEvento, UsuarioEvento, Comentario) VALUES
(@VersionId4, 'Creada', DATEADD(DAY, -15, GETDATE()), 1, 'Cotización creada inicialmente'),
(@VersionId4, 'Enviada', DATEADD(DAY, -12, GETDATE()), 1, 'Cotización enviada al cliente'),
(@VersionId4, 'Rechazada', DATEADD(DAY, -9, GETDATE()), 1, 'Cliente rechazó la cotización - precio muy alto');

-- Insertar archivo de cotización rechazada (CON FK de usuarios)
INSERT INTO ArchivoCotizacion (CotizacionId, VersionArchivada, FechaArchivado, UsuarioArchiva, TipoArchivo, MotivoArchivado, Comentario) 
VALUES ('COT-0004', 1, DATEADD(DAY, -8, GETDATE()), 1, 'R', 'Cotización rechazada por cliente', 'Precio considerado muy alto por el cliente');

GO

-- ============================================
-- VERIFICACIONES Y REPORTES
-- ============================================

-- Verificar datos insertados
PRINT 'Datos insertados exitosamente:';
SELECT 'Parametros' AS Tabla, COUNT(*) AS Total FROM Parametros
UNION ALL
SELECT 'Interesado', COUNT(*) FROM Interesado
UNION ALL
SELECT 'Cotizacion', COUNT(*) FROM Cotizacion
UNION ALL
SELECT 'CotizacionVersion', COUNT(*) FROM CotizacionVersion
UNION ALL
SELECT 'DetalleCotizacionVersion', COUNT(*) FROM DetalleCotizacionVersion
UNION ALL
SELECT 'HistorialCotizacion', COUNT(*) FROM HistorialCotizacion
UNION ALL
SELECT 'ArchivoCotizacion', COUNT(*) FROM ArchivoCotizacion;

-- Verificar Foreign Keys de auditoría
PRINT '';
PRINT 'Verificación de Foreign Keys de auditoría:';
SELECT 
    'Cotizacion' as Tabla,
    COUNT(DISTINCT CreatedBy) as UsuariosCreadores,
    COUNT(*) as TotalRegistros
FROM Cotizacion
UNION ALL
SELECT 
    'CotizacionVersion',
    COUNT(DISTINCT CreatedBy),
    COUNT(*)
FROM CotizacionVersion
UNION ALL
SELECT 
    'DetalleCotizacionVersion',
    COUNT(DISTINCT CreatedBy),
    COUNT(*)
FROM DetalleCotizacionVersion
UNION ALL
SELECT 
    'Interesado',
    COUNT(DISTINCT CreatedBy),
    COUNT(*)
FROM Interesado
UNION ALL
SELECT 
    'HistorialCotizacion',
    COUNT(DISTINCT UsuarioEvento),
    COUNT(*)
FROM HistorialCotizacion
UNION ALL
SELECT 
    'ArchivoCotizacion',
    COUNT(DISTINCT UsuarioArchiva),
    COUNT(*)
FROM ArchivoCotizacion;

-- Ver resumen de cotizaciones creadas
PRINT '';
PRINT 'Resumen de cotizaciones creadas:';
SELECT 
    c.CotizacionId,
    c.EstadoActual,
    CASE c.EstadoActual 
        WHEN 'B' THEN 'Borrador'
        WHEN 'E' THEN 'Enviada'
        WHEN 'A' THEN 'Aprobada'
        WHEN 'R' THEN 'Rechazada'
        ELSE 'Desconocido'
    END AS EstadoTexto,
    c.VersionActual,
    i.HubspotObjectId,
    cv.NombreInteresado,
    cv.Total,
    c.CreatedAt
FROM Cotizacion c
INNER JOIN Interesado i ON c.InteresadoId = i.InteresadoId
INNER JOIN CotizacionVersion cv ON c.CotizacionId = cv.CotizacionId 
    AND cv.VersionId = (SELECT TOP 1 VersionId FROM CotizacionVersion WHERE CotizacionId = c.CotizacionId AND VersionActual = c.VersionActual)
ORDER BY c.CreatedAt DESC;

PRINT 'Datos de prueba creados exitosamente con Foreign Keys funcionando correctamente';