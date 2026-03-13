-- ============================================
-- Script para insertar datos de prueba de cotizaciones
-- ============================================

-- Insertar Interesados de prueba
INSERT INTO Interesado (HubspotObjectId, HubspotObjectType, TipoInteresado, Activo, CreatedAt, CreatedBy) VALUES
('12345', 'contact', 'P', 1, GETUTCDATE(), 'system'),
('67890', 'company', 'E', 1, GETUTCDATE(), 'system'),
('11111', 'contact', 'P', 1, GETUTCDATE(), 'system');

-- Insertar Parámetros de prueba
INSERT INTO Parametros (Descripcion, Valor, Tipo, CreatedAt, CreatedBy) VALUES
('TasaImpuesto', '13', 'N', GETUTCDATE(), 'system'),
('MonedaDefault', 'CRC', 'S', GETUTCDATE(), 'system'),
('TiempoExpiracionCotizacion', '30', 'N', GETUTCDATE(), 'system'),
('TipoCambioBase', '520.50', 'N', GETUTCDATE(), 'system');

-- Obtener IDs de interesados
DECLARE @InteresadoId1 INT = (SELECT TOP 1 InteresadoId FROM Interesado WHERE HubspotObjectId = '12345');
DECLARE @InteresadoId2 INT = (SELECT TOP 1 InteresadoId FROM Interesado WHERE HubspotObjectId = '67890');
DECLARE @InteresadoId3 INT = (SELECT TOP 1 InteresadoId FROM Interesado WHERE HubspotObjectId = '11111');

-- Insertar Cotización 1 (Borrador)
INSERT INTO Cotizacion (CotizacionId, InteresadoId, EstadoActual, VersionActual, FechaCreacion, MontoCotizacion, CreatedAt, CreatedBy) 
VALUES ('COT-0001', @InteresadoId1, 'B', 1, GETUTCDATE(), 150000.00, GETUTCDATE(), 'admin@cotizaciones.com');

DECLARE @CotId1 INT = SCOPE_IDENTITY();

-- Insertar Versión 1 de Cotización 1
INSERT INTO CotizacionVersion (CotizacionId, NumeroVersion, FechaVersion, NombreInteresado, EmailInteresado, EmpresaInteresado, SubTotal, Impuesto, Descuento, Total, Moneda, TipoCambio, VersionActual, CreatedAt, CreatedBy) 
VALUES ('COT-0001', 1, GETUTCDATE(), 'Juan Pérez', 'juan.perez@email.com', 'Tech Solutions SA', 132743.36, 17256.64, 0.00, 150000.00, 'CRC', 520.50, '1', GETUTCDATE(), 'admin@cotizaciones.com');

DECLARE @VersionId1 INT = SCOPE_IDENTITY();

-- Insertar detalles de Versión 1
INSERT INTO DetalleCotizacionVersion (VersionId, ProductoId, Cantidad, PrecioUnitario, Descuento, TotalLinea, CreatedAt, CreatedBy) VALUES
(@VersionId1, 'PROD-001', 10.00, 5000.00, 0.00, 50000.00, GETUTCDATE(), 'admin@cotizaciones.com'),
(@VersionId1, 'PROD-002', 5.00, 10000.00, 5000.00, 45000.00, GETUTCDATE(), 'admin@cotizaciones.com'),
(@VersionId1, 'PROD-003', 3.00, 12581.12, 0.00, 37743.36, GETUTCDATE(), 'admin@cotizaciones.com');

-- Insertar historial de Versión 1
INSERT INTO HistorialCotizacion (VersionId, TipoEvento, FechaEvento, Comentario, CreatedAt, CreatedBy) 
VALUES (@VersionId1, 'Creada', GETUTCDATE(), 'Cotización creada inicialmente', GETUTCDATE(), 'admin@cotizaciones.com');

-- ============================================
-- Insertar Cotización 2 (Enviada con 2 versiones)
INSERT INTO Cotizacion (CotizacionId, InteresadoId, EstadoActual, VersionActual, FechaCreacion, MontoCotizacion, FechaEnvio, CreatedAt, CreatedBy) 
VALUES ('COT-0002', @InteresadoId2, 'E', 2, DATEADD(DAY, -5, GETUTCDATE()), 250000.00, DATEADD(DAY, -1, GETUTCDATE()), GETUTCDATE(), 'admin@cotizaciones.com');

DECLARE @CotId2 INT = SCOPE_IDENTITY();

-- Insertar Versión 1 de Cotización 2 (histórica)
INSERT INTO CotizacionVersion (CotizacionId, NumeroVersion, FechaVersion, NombreInteresado, EmailInteresado, EmpresaInteresado, SubTotal, Impuesto, Descuento, Total, Moneda, VersionActual, CreatedAt, CreatedBy) 
VALUES ('COT-0002', 1, DATEADD(DAY, -5, GETUTCDATE()), 'María González', 'maria.gonzalez@empresa.com', 'Innovatech Corp', 176991.15, 23008.85, 0.00, 200000.00, 'CRC', '0', GETUTCDATE(), 'admin@cotizaciones.com');

DECLARE @VersionId2_1 INT = SCOPE_IDENTITY();

-- Insertar detalles de Versión 1 de Cotización 2
INSERT INTO DetalleCotizacionVersion (VersionId, ProductoId, Cantidad, PrecioUnitario, Descuento, TotalLinea, CreatedAt, CreatedBy) VALUES
(@VersionId2_1, 'PROD-001', 20.00, 5000.00, 0.00, 100000.00, GETUTCDATE(), 'admin@cotizaciones.com'),
(@VersionId2_1, 'PROD-004', 10.00, 7699.12, 0.00, 76991.15, GETUTCDATE(), 'admin@cotizaciones.com');

-- Insertar historial de Versión 1 de Cotización 2
INSERT INTO HistorialCotizacion (VersionId, TipoEvento, FechaEvento, Comentario, CreatedAt, CreatedBy) 
VALUES (@VersionId2_1, 'Creada', DATEADD(DAY, -5, GETUTCDATE()), 'Cotización creada inicialmente', GETUTCDATE(), 'admin@cotizaciones.com');

-- Insertar Versión 2 de Cotización 2 (actual)
INSERT INTO CotizacionVersion (CotizacionId, NumeroVersion, FechaVersion, NombreInteresado, EmailInteresado, EmpresaInteresado, SubTotal, Impuesto, Descuento, Total, Moneda, VersionActual, Notas, CreatedAt, CreatedBy) 
VALUES ('COT-0002', 2, DATEADD(DAY, -2, GETUTCDATE()), 'María González', 'maria.gonzalez@empresa.com', 'Innovatech Corp', 221238.94, 28761.06, 0.00, 250000.00, 'CRC', '1', 'Cliente solicitó agregar más productos', GETUTCDATE(), 'admin@cotizaciones.com');

DECLARE @VersionId2_2 INT = SCOPE_IDENTITY();

-- Insertar detalles de Versión 2 de Cotización 2
INSERT INTO DetalleCotizacionVersion (VersionId, ProductoId, Cantidad, PrecioUnitario, Descuento, TotalLinea, CreatedAt, CreatedBy) VALUES
(@VersionId2_2, 'PROD-001', 25.00, 5000.00, 0.00, 125000.00, GETUTCDATE(), 'admin@cotizaciones.com'),
(@VersionId2_2, 'PROD-004', 10.00, 7699.12, 0.00, 76991.15, GETUTCDATE(), 'admin@cotizaciones.com'),
(@VersionId2_2, 'PROD-005', 2.00, 9623.89, 0.00, 19247.79, GETUTCDATE(), 'admin@cotizaciones.com');

-- Insertar historial de Versión 2 de Cotización 2
INSERT INTO HistorialCotizacion (VersionId, TipoEvento, FechaEvento, Comentario, CreatedAt, CreatedBy) VALUES
(@VersionId2_2, 'Versión Generada', DATEADD(DAY, -2, GETUTCDATE()), 'Nueva versión generada con productos adicionales', GETUTCDATE(), 'admin@cotizaciones.com'),
(@VersionId2_2, 'Enviada', DATEADD(DAY, -1, GETUTCDATE()), 'Cotización enviada al cliente por email', GETUTCDATE(), 'admin@cotizaciones.com');

-- ============================================
-- Insertar Cotización 3 (Aprobada)
INSERT INTO Cotizacion (CotizacionId, InteresadoId, EstadoActual, VersionActual, FechaCreacion, MontoCotizacion, FechaEnvio, CreatedAt, CreatedBy) 
VALUES ('COT-0003', @InteresadoId3, 'A', 1, DATEADD(DAY, -10, GETUTCDATE()), 75000.00, DATEADD(DAY, -8, GETUTCDATE()), GETUTCDATE(), 'admin@cotizaciones.com');

DECLARE @CotId3 INT = SCOPE_IDENTITY();

-- Insertar Versión 1 de Cotización 3
INSERT INTO CotizacionVersion (CotizacionId, NumeroVersion, FechaVersion, NombreInteresado, EmailInteresado, EmpresaInteresado, SubTotal, Impuesto, Descuento, Total, Moneda, VersionActual, CreatedAt, CreatedBy) 
VALUES ('COT-0003', 1, DATEADD(DAY, -10, GETUTCDATE()), 'Carlos Ramírez', 'carlos.ramirez@company.com', 'Digital Services', 66371.68, 8628.32, 0.00, 75000.00, 'CRC', '1', GETUTCDATE(), 'admin@cotizaciones.com');

DECLARE @VersionId3 INT = SCOPE_IDENTITY();

-- Insertar detalles de Versión 1 de Cotización 3
INSERT INTO DetalleCotizacionVersion (VersionId, ProductoId, Cantidad, PrecioUnitario, Descuento, TotalLinea, CreatedAt, CreatedBy) VALUES
(@VersionId3, 'PROD-002', 5.00, 10000.00, 0.00, 50000.00, GETUTCDATE(), 'admin@cotizaciones.com'),
(@VersionId3, 'PROD-003', 2.00, 8185.84, 0.00, 16371.68, GETUTCDATE(), 'admin@cotizaciones.com');

-- Insertar historial de Versión 1 de Cotización 3
INSERT INTO HistorialCotizacion (VersionId, TipoEvento, FechaEvento, Comentario, CreatedAt, CreatedBy) VALUES
(@VersionId3, 'Creada', DATEADD(DAY, -10, GETUTCDATE()), 'Cotización creada inicialmente', GETUTCDATE(), 'admin@cotizaciones.com'),
(@VersionId3, 'Enviada', DATEADD(DAY, -8, GETUTCDATE()), 'Cotización enviada al cliente', GETUTCDATE(), 'admin@cotizaciones.com'),
(@VersionId3, 'Aprobada', DATEADD(DAY, -6, GETUTCDATE()), 'Cliente aprobó la cotización', GETUTCDATE(), 'admin@cotizaciones.com');

GO

-- Verificar datos insertados
SELECT 'Cotizaciones' AS Tabla, COUNT(*) AS Total FROM Cotizacion
UNION ALL
SELECT 'CotizacionVersion', COUNT(*) FROM CotizacionVersion
UNION ALL
SELECT 'DetalleCotizacionVersion', COUNT(*) FROM DetalleCotizacionVersion
UNION ALL
SELECT 'HistorialCotizacion', COUNT(*) FROM HistorialCotizacion
UNION ALL
SELECT 'Interesado', COUNT(*) FROM Interesado
UNION ALL
SELECT 'Parametros', COUNT(*) FROM Parametros;

GO

-- Ver cotizaciones con su versión actual
SELECT 
    c.CotizacionId,
    c.EstadoActual,
    c.VersionActual,
    cv.NumeroVersion,
    cv.NombreInteresado,
    cv.Total,
    c.FechaCreacion
FROM Cotizacion c
INNER JOIN CotizacionVersion cv ON c.CotizacionId = cv.CotizacionId AND cv.VersionActual = '1'
ORDER BY c.FechaCreacion DESC;
