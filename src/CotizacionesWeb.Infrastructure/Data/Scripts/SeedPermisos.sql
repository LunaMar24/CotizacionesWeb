-- ============================================
-- Script para poblar permisos iniciales
-- Generado desde estructura consolidada
-- Base de datos: CotizacionesWeb
-- ============================================

USE CotizacionesWeb;
GO

-- Verificar si ya existen permisos
IF EXISTS (SELECT 1 FROM Permisos)
BEGIN
    PRINT '==============================================';
    PRINT 'ADVERTENCIA: Ya existen permisos en la base de datos.';
    PRINT '==============================================';
    PRINT '';
    PRINT 'Para recrear los permisos desde cero, ejecute primero:';
    PRINT '';
    PRINT '  DELETE FROM PermisosRoles;';
    PRINT '  DELETE FROM Permisos;';
    PRINT '';
    PRINT 'Luego vuelva a ejecutar este script.';
    PRINT '==============================================';
    RETURN;
END

PRINT '==============================================';
PRINT 'INICIANDO CREACIÓN DE PERMISOS';
PRINT '==============================================';
PRINT '';

-- ============================================
-- PERMISOS DE CONFIGURACIÓN
-- ============================================
PRINT 'Insertando permisos de Configuración...';

INSERT INTO Permisos (Codigo, Categoria, Descripcion) VALUES
('CFG_PARAMS_VIEW', 'Configuracion', 'Ver parámetros del sistema'),
('CFG_PARAMS_EDIT', 'Configuracion', 'Editar parámetros del sistema'),
('CFG_PARAMS_RESET', 'Configuracion', 'Resetear parámetros a valores por defecto'),
('CFG_PARAMS_VIEW_SECRET','Configuracion', 'Ver valores reales de parámetros sensitivos (contraseñas, tokens, API keys)'),
('CFG_LOGS', 'Configuracion', 'Ver logs del sistema');

PRINT '? 5 permisos de Configuración insertados';
PRINT '';

-- ============================================
-- PERMISOS DE COTIZACIONES
-- ============================================
PRINT 'Insertando permisos de Cotizaciones...';

INSERT INTO Permisos (Codigo, Categoria, Descripcion) VALUES
('COT_VIEW', 'Cotizaciones', 'Ver cotizaciones (solo lectura)'),
('COT_VIEW_DETAIL', 'Cotizaciones', 'Ver detalle de cotizacion (solo lectura)'),
('COT_VIEW_HISTORY', 'Cotizaciones', 'Ver historial de cotizacion'),
('COT_VIEW_VERSIONS', 'Cotizaciones', 'Ver versiones de cotizacion'),
('COT_CREATE', 'Cotizaciones', 'Crear nuevas cotizaciones'),
('COT_EDIT', 'Cotizaciones', 'Editar cotizaciones'),
('COT_DELETE', 'Cotizaciones', 'Eliminar cotizaciones'),
('COT_APPROVE', 'Cotizaciones', 'Aprobar cotizaciones'),
('COT_REJECT', 'Cotizaciones', 'Rechazar cotizaciones'),
('COT_ACCEPT', 'Cotizaciones', 'Aceptar cotizaciones por parte del cliente'),
('COT_SEND_CLIENT', 'Cotizaciones', 'Enviar cotizacion al cliente luego de aprobada'),
('COT_EXPORT', 'Cotizaciones', 'Exportar cotizaciones'),
('COT_COPY', 'Cotizaciones', 'Copiar versión de cotización'),
('COT_DUPLICATE', 'Cotizaciones', 'Duplicar cotizaciones'),
('COT_ARCHIVE', 'Cotizaciones', 'Archivar cotizaciones'),
('COT_ARCHIVE_VIEW', 'Cotizaciones', 'Ver cotizaciones archivadas'),
('COT_ARCHIVE_VIEW_DETAIL', 'Cotizaciones', 'Ver detalle cotizaciones archivadas'),
('COT_ARCHIVE_REACTIVATE', 'Cotizaciones', 'Reactivar cotizaciones archivadas'),
('COT_SEND_ERP', 'Cotizaciones', 'Enviar cotizaciones al ERP');

PRINT '? 19 permisos de Cotizaciones insertados';
PRINT '';

-- ============================================
-- PERMISOS DE REPORTES
-- ============================================
PRINT 'Insertando permisos de Reportes...';

INSERT INTO Permisos (Codigo, Categoria, Descripcion) VALUES
('RPT_VIEW', 'Reportes', 'Ver reportes'),
('RPT_EXPORT', 'Reportes', 'Exportar reportes'),
('RPT_DASHBOARD', 'Reportes', 'Acceso al dashboard ejecutivo'),
('RPT_HISTORIAL', 'Reportes', 'Ver historial de cotizaciones');

PRINT '? 4 permisos de Reportes insertados';
PRINT '';

-- ============================================
-- PERMISOS DE ROLES
-- ============================================
PRINT 'Insertando permisos de Roles...';

INSERT INTO Permisos (Codigo, Categoria, Descripcion) VALUES
('ROL_VIEW', 'Roles', 'Ver roles del sistema'),
('ROL_CREATE', 'Roles', 'Crear nuevos roles'),
('ROL_EDIT', 'Roles', 'Editar roles existentes'),
('ROL_DELETE', 'Roles', 'Eliminar roles'),
('ROL_PERMISOS', 'Roles', 'Gestionar permisos de roles');

PRINT '? 5 permisos de Roles insertados';
PRINT '';

-- ============================================
-- PERMISOS DE USUARIOS
-- ============================================
PRINT 'Insertando permisos de Usuarios...';

INSERT INTO Permisos (Codigo, Categoria, Descripcion) VALUES
('USR_VIEW', 'Usuarios', 'Ver usuarios del sistema'),
('USR_CREATE', 'Usuarios', 'Crear nuevos usuarios'),
('USR_EDIT', 'Usuarios', 'Editar usuarios existentes'),
('USR_DELETE', 'Usuarios', 'Eliminar usuarios'),
('USR_ROLES', 'Usuarios', 'Gestionar roles de usuarios'),
('USR_RESET_PWD', 'Usuarios', 'Resetear contraseñas');

PRINT '? 6 permisos de Usuarios insertados';
PRINT '';

GO

-- ============================================
-- VERIFICACIÓN DE PERMISOS CREADOS
-- ============================================

PRINT '==============================================';
PRINT 'PERMISOS CREADOS EXITOSAMENTE';
PRINT '==============================================';
PRINT '';

-- Mostrar todos los permisos ordenados por categoría y código
SELECT 
    PermisoId,
    Codigo, 
    Categoria, 
    Descripcion 
FROM Permisos 
ORDER BY Categoria, Codigo;

PRINT '';
PRINT '==============================================';
PRINT 'RESUMEN POR CATEGORÍA';
PRINT '==============================================';

SELECT 
    Categoria,
    COUNT(*) as TotalPermisos,
    STRING_AGG(Codigo, ', ') WITHIN GROUP (ORDER BY Codigo) AS Permisos
FROM Permisos 
GROUP BY Categoria
ORDER BY Categoria;

PRINT '';
PRINT '==============================================';
PRINT 'DETALLE DE CATEGORÍAS';
PRINT '==============================================';
PRINT '  • Configuracion.......: 5 permisos';
PRINT '  • Cotizaciones........: 19 permisos';
PRINT '  • Reportes............: 4 permisos';
PRINT '  • Roles...............: 5 permisos';
PRINT '  • Usuarios............: 6 permisos';
PRINT '';
PRINT '  TOTAL.................: 39 permisos';
PRINT '==============================================';
PRINT '';
PRINT 'SIGUIENTE PASO:';
PRINT 'Ejecute el script CrearUsuarioAdmin.sql para crear el';
PRINT 'usuario administrador y asignar todos estos permisos.';
PRINT '==============================================';

GO