-- ============================================
-- Script para actualizar permisos de Cotizaciones
-- Fecha: 2026
-- Ejecutar SOLO si ya existen permisos en la BD
-- ============================================

PRINT '========================================';
PRINT 'Iniciando actualizacion de permisos...';
PRINT '========================================';

-- 1. Actualizar descripcion del permiso COT_VIEW para aclarar que es ver el mantenimiento
PRINT 'Actualizando COT_VIEW...';
UPDATE Permisos 
SET Descripcion = 'Ver mantenimiento de cotizaciones'
WHERE Codigo = 'COT_VIEW';

-- 2. Verificar si existe COT_VERSION para eliminarlo
IF EXISTS (SELECT 1 FROM Permisos WHERE Codigo = 'COT_VERSION')
BEGIN
    PRINT 'Eliminando permiso obsoleto COT_VERSION...';
    
    -- Primero eliminar las relaciones en PermisosRoles
    DELETE FROM PermisosRoles 
    WHERE PermisoId IN (SELECT PermisoId FROM Permisos WHERE Codigo = 'COT_VERSION');
    
    -- Luego eliminar el permiso
    DELETE FROM Permisos WHERE Codigo = 'COT_VERSION';
    
    PRINT 'Permiso COT_VERSION eliminado correctamente.';
END
ELSE
BEGIN
    PRINT 'Permiso COT_VERSION no existe (omitiendo).';
END

-- 3. Agregar nuevos permisos especificos (solo si no existen)
PRINT 'Agregando nuevos permisos...';

-- COT_VIEW_DETAIL
IF NOT EXISTS (SELECT 1 FROM Permisos WHERE Codigo = 'COT_VIEW_DETAIL')
BEGIN
    INSERT INTO Permisos (Codigo, Categoria, Descripcion) 
    VALUES ('COT_VIEW_DETAIL', 'Cotizaciones', 'Ver detalle de cotizacion (solo lectura)');
    PRINT '  + COT_VIEW_DETAIL agregado';
END
ELSE
BEGIN
    PRINT '  - COT_VIEW_DETAIL ya existe (omitiendo)';
END

-- COT_VIEW_HISTORY
IF NOT EXISTS (SELECT 1 FROM Permisos WHERE Codigo = 'COT_VIEW_HISTORY')
BEGIN
    INSERT INTO Permisos (Codigo, Categoria, Descripcion) 
    VALUES ('COT_VIEW_HISTORY', 'Cotizaciones', 'Ver historial de cotizacion');
    PRINT '  + COT_VIEW_HISTORY agregado';
END
ELSE
BEGIN
    PRINT '  - COT_VIEW_HISTORY ya existe (omitiendo)';
END

-- COT_VIEW_VERSIONS
IF NOT EXISTS (SELECT 1 FROM Permisos WHERE Codigo = 'COT_VIEW_VERSIONS')
BEGIN
    INSERT INTO Permisos (Codigo, Categoria, Descripcion) 
    VALUES ('COT_VIEW_VERSIONS', 'Cotizaciones', 'Ver versiones de cotizacion');
    PRINT '  + COT_VIEW_VERSIONS agregado';
END
ELSE
BEGIN
    PRINT '  - COT_VIEW_VERSIONS ya existe (omitiendo)';
END

-- COT_COPY
IF NOT EXISTS (SELECT 1 FROM Permisos WHERE Codigo = 'COT_COPY')
BEGIN
    INSERT INTO Permisos (Codigo, Categoria, Descripcion) 
    VALUES ('COT_COPY', 'Cotizaciones', 'Copiar version de cotizacion');
    PRINT '  + COT_COPY agregado';
END
ELSE
BEGIN
    PRINT '  - COT_COPY ya existe (omitiendo)';
END

-- COT_ARCHIVE
IF NOT EXISTS (SELECT 1 FROM Permisos WHERE Codigo = 'COT_ARCHIVE')
BEGIN
    INSERT INTO Permisos (Codigo, Categoria, Descripcion) 
    VALUES ('COT_ARCHIVE', 'Cotizaciones', 'Archivar cotizaciones');
    PRINT '  + COT_ARCHIVE agregado';
END
ELSE
BEGIN
    PRINT '  - COT_ARCHIVE ya existe (omitiendo)';
END

-- COT_SEND_ERP
IF NOT EXISTS (SELECT 1 FROM Permisos WHERE Codigo = 'COT_SEND_ERP')
BEGIN
    INSERT INTO Permisos (Codigo, Categoria, Descripcion) 
    VALUES ('COT_SEND_ERP', 'Cotizaciones', 'Enviar cotizaciones al ERP');
    PRINT '  + COT_SEND_ERP agregado';
END
ELSE
BEGIN
    PRINT '  - COT_SEND_ERP ya existe (omitiendo)';
END

-- COT_SEND_CLIENT
IF NOT EXISTS (SELECT 1 FROM Permisos WHERE Codigo = 'COT_SEND_CLIENT')
BEGIN
    INSERT INTO Permisos (Codigo, Categoria, Descripcion) 
    VALUES ('COT_SEND_CLIENT', 'Cotizaciones', 'Enviar cotizacion al cliente (Aprobada -> Enviada)');
    PRINT '  + COT_SEND_CLIENT agregado';
END
ELSE
BEGIN
    PRINT '  - COT_SEND_CLIENT ya existe (omitiendo)';
END

-- COT_ACCEPT
IF NOT EXISTS (SELECT 1 FROM Permisos WHERE Codigo = 'COT_ACCEPT')
BEGIN
    INSERT INTO Permisos (Codigo, Categoria, Descripcion) 
    VALUES ('COT_ACCEPT', 'Cotizaciones', 'Aceptar cotizaciones (Enviada -> Aceptada)');
    PRINT '  + COT_ACCEPT agregado';
END
ELSE
BEGIN
    PRINT '  - COT_ACCEPT ya existe (omitiendo)';
END

GO

-- Verificar permisos de cotizaciones actualizados
PRINT '';
PRINT '========================================';
PRINT 'Permisos de Cotizaciones (Estado Final)';
PRINT '========================================';

SELECT 
    PermisoId,
    Codigo, 
    Descripcion 
FROM Permisos 
WHERE Categoria = 'Cotizaciones'
ORDER BY Codigo;

PRINT '';
PRINT '========================================';
PRINT 'Actualizacion completada exitosamente';
PRINT '========================================';
PRINT '';
PRINT 'RESUMEN DE CAMBIOS:';
PRINT '  Nuevos permisos:';
PRINT '    - COT_VIEW_DETAIL: Ver detalle de cotizacion (solo lectura)';
PRINT '    - COT_VIEW_HISTORY: Ver historial de cotizacion';
PRINT '    - COT_VIEW_VERSIONS: Ver versiones de cotizacion';
PRINT '    - COT_COPY: Copiar version de cotizacion';
PRINT '    - COT_ARCHIVE: Archivar cotizaciones';
PRINT '    - COT_SEND_ERP: Enviar cotizaciones al ERP';
PRINT '    - COT_SEND_CLIENT: Enviar cotizacion al cliente (Aprobada -> Enviada)';
PRINT '    - COT_ACCEPT: Aceptar cotizaciones (Enviada -> Aceptada)';
PRINT '';
PRINT '  Permisos eliminados:';
PRINT '    - COT_VERSION (obsoleto, reemplazado por COT_COPY)';
PRINT '';
PRINT '  Permisos actualizados:';
PRINT '    - COT_VIEW: Ahora especifica "Ver mantenimiento de cotizaciones"';
PRINT '';
PRINT 'IMPORTANTE: Asignar estos nuevos permisos a los roles correspondientes.';
PRINT '========================================';
