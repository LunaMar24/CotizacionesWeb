-- ============================================
-- Script para eliminar datos de prueba de cotizaciones
-- Autor: Sistema
-- Fecha: 2026-03-12
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
PRINT 'Restricciones de FK rehabilitadas';

GO

-- Resetear los IDENTITY seeds a 1
PRINT 'Reseteando IDENTITY seeds...';
DBCC CHECKIDENT ('HistorialCotizacion', RESEED, 0);
DBCC CHECKIDENT ('DetalleCotizacionVersion', RESEED, 0);
DBCC CHECKIDENT ('CotizacionVersion', RESEED, 0);
DBCC CHECKIDENT ('ArchivoCotizacion', RESEED, 0);
DBCC CHECKIDENT ('Cotizacion', RESEED, 0);
DBCC CHECKIDENT ('Interesado', RESEED, 0);
DBCC CHECKIDENT ('Parametros', RESEED, 0);
PRINT 'IDENTITY seeds reseteados';

GO

-- Verificar que las tablas están vacías
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

PRINT 'Script de limpieza completado exitosamente';

