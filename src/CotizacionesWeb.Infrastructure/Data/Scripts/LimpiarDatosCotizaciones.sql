-- ============================================
-- Script para eliminar datos de prueba de cotizaciones
-- ACTUALIZADO PARA ESTRUCTURA CORREGIDA CON FOREIGN KEYS
-- ============================================

USE CotizacionesWebDev;
GO

-- Deshabilitar restricciones de FK temporalmente
PRINT 'Deshabilitando restricciones de FK...';
ALTER TABLE HistorialCotizacion NOCHECK CONSTRAINT ALL;
ALTER TABLE DetalleCotizacionVersion NOCHECK CONSTRAINT ALL;
ALTER TABLE CotizacionVersion NOCHECK CONSTRAINT ALL;
ALTER TABLE ArchivoCotizacion NOCHECK CONSTRAINT ALL;
ALTER TABLE Cotizacion NOCHECK CONSTRAINT ALL;
ALTER TABLE Interesado NOCHECK CONSTRAINT ALL;
GO

-- Eliminar datos en orden inverso a las dependencias
PRINT 'Eliminando datos de tablas de cotizaciones...';

DELETE FROM HistorialCotizacion;
PRINT 'HistorialCotizacion: limpiado';

DELETE FROM DetalleCotizacionVersion;
PRINT 'DetalleCotizacionVersion: limpiado';

DELETE FROM CotizacionVersion;
PRINT 'CotizacionVersion: limpiado';

DELETE FROM ArchivoCotizacion;
PRINT 'ArchivoCotizacion: limpiado';

DELETE FROM Cotizacion;
PRINT 'Cotizacion: limpiado';

DELETE FROM Interesado;
PRINT 'Interesado: limpiado';

DELETE FROM Parametros;
PRINT 'Parametros: limpiado';

GO

-- Rehabilitar restricciones de FK
PRINT 'Rehabilitando restricciones de FK...';
ALTER TABLE HistorialCotizacion WITH CHECK CHECK CONSTRAINT ALL;
ALTER TABLE DetalleCotizacionVersion WITH CHECK CHECK CONSTRAINT ALL;
ALTER TABLE CotizacionVersion WITH CHECK CHECK CONSTRAINT ALL;
ALTER TABLE ArchivoCotizacion WITH CHECK CHECK CONSTRAINT ALL;
ALTER TABLE Cotizacion WITH CHECK CHECK CONSTRAINT ALL;
ALTER TABLE Interesado WITH CHECK CHECK CONSTRAINT ALL;
PRINT 'Restricciones de FK rehabilitadas';

GO

-- Resetear los IDENTITY seeds con estructura corregida
PRINT 'Reseteando IDENTITY seeds...';

-- Entidades con llaves específicas + IDENTITY
DBCC CHECKIDENT ('HistorialCotizacion', RESEED, 0);        -- HistorialId
DBCC CHECKIDENT ('DetalleCotizacionVersion', RESEED, 0);   -- DetalleVersionId
DBCC CHECKIDENT ('CotizacionVersion', RESEED, 0);          -- VersionId
DBCC CHECKIDENT ('ArchivoCotizacion', RESEED, 0);          -- ArchivoId
DBCC CHECKIDENT ('Interesado', RESEED, 0);                 -- InteresadoId
DBCC CHECKIDENT ('Parametros', RESEED, 0);                 -- ParametroId

-- Entidades de usuarios y roles (preservar estructura de auditoría)
-- NOTA: NO resetear Usuarios para preservar usuario sistema (ID=1)
-- DBCC CHECKIDENT ('Usuarios', RESEED, 1);
DBCC CHECKIDENT ('Roles', RESEED, 0);                      -- RolId
DBCC CHECKIDENT ('Permisos', RESEED, 0);                   -- PermisoId

PRINT 'IDENTITY seeds reseteados';

GO

-- Verificar que las tablas estén vacías
PRINT '';
PRINT 'Verificación de limpieza:';
SELECT 'Cotizacion' AS Tabla, COUNT(*) AS Total FROM Cotizacion
UNION ALL
SELECT 'CotizacionVersion', COUNT(*) FROM CotizacionVersion
UNION ALL
SELECT 'DetalleCotizacionVersion', COUNT(*) FROM DetalleCotizacionVersion
UNION ALL
SELECT 'HistorialCotizacion', COUNT(*) FROM HistorialCotizacion
UNION ALL
SELECT 'ArchivoCotizacion', COUNT(*) FROM ArchivoCotizacion
UNION ALL
SELECT 'Interesado', COUNT(*) FROM Interesado
UNION ALL
SELECT 'Parametros', COUNT(*) FROM Parametros;

-- Verificar Foreign Keys funcionando
PRINT '';
PRINT 'Verificación de Foreign Keys activas:';
SELECT 
    fk.name AS ForeignKeyName,
    tp.name AS ParentTable,
    cp.name AS ParentColumn,
    tr.name AS ReferencedTable,
    cr.name AS ReferencedColumn
FROM sys.foreign_keys fk
INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
INNER JOIN sys.columns cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
WHERE tp.name IN ('Cotizacion', 'CotizacionVersion', 'DetalleCotizacionVersion', 'Interesado', 'HistorialCotizacion', 'ArchivoCotizacion')
ORDER BY tp.name, fk.name;

PRINT '';
PRINT 'ADVERTENCIA: Usuario sistema (ID=1) preservado para foreign keys de auditoría';
PRINT 'Script de limpieza completado exitosamente con estructura corregida';