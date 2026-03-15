-- ============================================
-- Script para poblar permisos iniciales
-- ACTUALIZADO PARA NUEVA ESTRUCTURA DE LLAVES PRIMARIAS
-- ============================================

-- NOTA: Permisos NO tienen auditoría según el modelo (sin CreatedAt/CreatedBy)

-- Permisos de Usuarios
INSERT INTO Permisos (Codigo, Categoria, Descripcion) VALUES
('USR_VIEW', 'Usuarios', 'Ver usuarios del sistema'),
('USR_CREATE', 'Usuarios', 'Crear nuevos usuarios'),
('USR_EDIT', 'Usuarios', 'Editar usuarios existentes'),
('USR_DELETE', 'Usuarios', 'Eliminar usuarios'),
('USR_ROLES', 'Usuarios', 'Gestionar roles de usuarios'),
('USR_RESET_PWD', 'Usuarios', 'Resetear contraseñas');

-- Permisos de Roles
INSERT INTO Permisos (Codigo, Categoria, Descripcion) VALUES
('ROL_VIEW', 'Roles', 'Ver roles del sistema'),
('ROL_CREATE', 'Roles', 'Crear nuevos roles'),
('ROL_EDIT', 'Roles', 'Editar roles existentes'),
('ROL_DELETE', 'Roles', 'Eliminar roles'),
('ROL_PERMISOS', 'Roles', 'Gestionar permisos de roles');

-- Permisos de Cotizaciones
INSERT INTO Permisos (Codigo, Categoria, Descripcion) VALUES
('COT_VIEW', 'Cotizaciones', 'Ver mantenimiento de cotizaciones'),
('COT_VIEW_DETAIL', 'Cotizaciones', 'Ver detalle de cotizacion (solo lectura)'),
('COT_VIEW_HISTORY', 'Cotizaciones', 'Ver historial de cotizacion'),
('COT_VIEW_VERSIONS', 'Cotizaciones', 'Ver versiones de cotizacion'),
('COT_CREATE', 'Cotizaciones', 'Crear nuevas cotizaciones'),
('COT_EDIT', 'Cotizaciones', 'Editar cotizaciones'),
('COT_DELETE', 'Cotizaciones', 'Eliminar cotizaciones'),
('COT_APPROVE', 'Cotizaciones', 'Aprobar cotizaciones (Pendiente -> Aprobada)'),
('COT_REJECT', 'Cotizaciones', 'Rechazar cotizaciones (Enviada -> Rechazada)'),
('COT_ACCEPT', 'Cotizaciones', 'Aceptar cotizaciones (Enviada -> Aceptada)'),
('COT_SEND_CLIENT', 'Cotizaciones', 'Enviar cotizacion al cliente (Aprobada -> Enviada)'),
('COT_EXPORT', 'Cotizaciones', 'Exportar cotizaciones'),
('COT_COPY', 'Cotizaciones', 'Copiar version de cotizacion'),
('COT_DUPLICATE', 'Cotizaciones', 'Duplicar cotizaciones'),
('COT_ARCHIVE', 'Cotizaciones', 'Archivar cotizaciones'),
('COT_SEND_ERP', 'Cotizaciones', 'Enviar cotizaciones al ERP');

-- Permisos de Clientes/Interesados
INSERT INTO Permisos (Codigo, Categoria, Descripcion) VALUES
('CLI_VIEW', 'Clientes', 'Ver clientes/interesados'),
('CLI_CREATE', 'Clientes', 'Crear nuevos clientes/interesados'),
('CLI_EDIT', 'Clientes', 'Editar clientes/interesados'),
('CLI_DELETE', 'Clientes', 'Eliminar clientes/interesados'),
('CLI_SYNC', 'Clientes', 'Sincronizar con HubSpot');

-- Permisos de Reportes
INSERT INTO Permisos (Codigo, Categoria, Descripcion) VALUES
('RPT_VIEW', 'Reportes', 'Ver reportes'),
('RPT_EXPORT', 'Reportes', 'Exportar reportes'),
('RPT_DASHBOARD', 'Reportes', 'Acceso al dashboard ejecutivo'),
('RPT_HISTORIAL', 'Reportes', 'Ver historial de cotizaciones');

-- Permisos de Configuración
INSERT INTO Permisos (Codigo, Categoria, Descripcion) VALUES
('CFG_VIEW', 'Configuración', 'Ver configuración del sistema'),
('CFG_EDIT', 'Configuración', 'Editar configuración del sistema'),
('CFG_LOGS', 'Configuración', 'Ver logs del sistema'),
('CFG_PARAMS', 'Configuración', 'Gestionar parámetros del sistema');

-- Permisos de Integraciones
INSERT INTO Permisos (Codigo, Categoria, Descripcion) VALUES
('INT_HUBSPOT', 'Integraciones', 'Gestionar integración con HubSpot'),
('INT_ERP', 'Integraciones', 'Gestionar integración con ERP'),
('INT_CONFIG', 'Integraciones', 'Configurar integraciones');

GO

-- Verificar los permisos creados
SELECT 
    PermisoId,  -- NUEVA ESTRUCTURA: PermisoId en lugar de Id
    Codigo, 
    Categoria, 
    Descripcion 
FROM Permisos 
ORDER BY Categoria, Codigo;

PRINT 'Permisos creados exitosamente con nueva estructura';

-- Mostrar resumen por categoría
SELECT 
    Categoria,
    COUNT(*) as TotalPermisos
FROM Permisos 
GROUP BY Categoria
ORDER BY Categoria;

-- ============================================
-- Script para actualizar permisos de Cotizaciones
-- Fecha: 2026
-- ============================================

-- 1. Actualizar descripción del permiso COT_VIEW
UPDATE Permisos 
SET Descripcion = 'Ver cotizaciones (solo lectura)'
WHERE Codigo = 'COT_VIEW';

-- 2. Eliminar permiso COT_VERSION (obsoleto, se reemplaza por COT_COPY)
-- Primero eliminar las relaciones en PermisosRoles
DELETE FROM PermisosRoles 
WHERE PermisoId IN (SELECT PermisoId FROM Permisos WHERE Codigo = 'COT_VERSION');

-- Luego eliminar el permiso
DELETE FROM Permisos WHERE Codigo = 'COT_VERSION';

-- 3. Agregar nuevos permisos específicos
INSERT INTO Permisos (Codigo, Categoria, Descripcion) VALUES
('COT_COPY', 'Cotizaciones', 'Copiar versión de cotización'),
('COT_ARCHIVE', 'Cotizaciones', 'Archivar cotizaciones'),
('COT_SEND_ERP', 'Cotizaciones', 'Enviar cotizaciones al ERP');

GO

-- Verificar permisos de cotizaciones actualizados
SELECT 
    PermisoId,
    Codigo, 
    Descripcion 
FROM Permisos 
WHERE Categoria = 'Cotizaciones'
ORDER BY Codigo;

PRINT '========================================';
PRINT 'Permisos de cotizaciones actualizados';
PRINT '========================================';
PRINT 'Nuevos permisos:';
PRINT '  - COT_COPY: Copiar versión de cotización';
PRINT '  - COT_ARCHIVE: Archivar cotizaciones';
PRINT '  - COT_SEND_ERP: Enviar al ERP';
PRINT '';
PRINT 'Permisos eliminados:';
PRINT '  - COT_VERSION (reemplazado por COT_COPY)';
PRINT '';
PRINT 'Permisos actualizados:';
PRINT '  - COT_VIEW: Ahora específica "solo lectura"';