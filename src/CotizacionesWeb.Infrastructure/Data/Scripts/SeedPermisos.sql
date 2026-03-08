-- ============================================
-- Script para poblar permisos iniciales
-- ============================================

-- Permisos de Usuarios
INSERT INTO Permisos (Codigo, Categoria, Descripcion, CreatedAt, CreatedBy) VALUES
('USR_VIEW', 'Usuarios', 'Ver usuarios del sistema', GETUTCDATE(), 'system'),
('USR_CREATE', 'Usuarios', 'Crear nuevos usuarios', GETUTCDATE(), 'system'),
('USR_EDIT', 'Usuarios', 'Editar usuarios existentes', GETUTCDATE(), 'system'),
('USR_DELETE', 'Usuarios', 'Eliminar usuarios', GETUTCDATE(), 'system'),
('USR_ROLES', 'Usuarios', 'Gestionar roles de usuarios', GETUTCDATE(), 'system'),
('USR_RESET_PWD', 'Usuarios', 'Resetear contraseñas', GETUTCDATE(), 'system');

-- Permisos de Roles
INSERT INTO Permisos (Codigo, Categoria, Descripcion, CreatedAt, CreatedBy) VALUES
('ROL_VIEW', 'Roles', 'Ver roles del sistema', GETUTCDATE(), 'system'),
('ROL_CREATE', 'Roles', 'Crear nuevos roles', GETUTCDATE(), 'system'),
('ROL_EDIT', 'Roles', 'Editar roles existentes', GETUTCDATE(), 'system'),
('ROL_DELETE', 'Roles', 'Eliminar roles', GETUTCDATE(), 'system'),
('ROL_PERMISOS', 'Roles', 'Gestionar permisos de roles', GETUTCDATE(), 'system');

-- Permisos de Cotizaciones
INSERT INTO Permisos (Codigo, Categoria, Descripcion, CreatedAt, CreatedBy) VALUES
('COT_VIEW', 'Cotizaciones', 'Ver cotizaciones', GETUTCDATE(), 'system'),
('COT_CREATE', 'Cotizaciones', 'Crear nuevas cotizaciones', GETUTCDATE(), 'system'),
('COT_EDIT', 'Cotizaciones', 'Editar cotizaciones', GETUTCDATE(), 'system'),
('COT_DELETE', 'Cotizaciones', 'Eliminar cotizaciones', GETUTCDATE(), 'system'),
('COT_APPROVE', 'Cotizaciones', 'Aprobar cotizaciones', GETUTCDATE(), 'system'),
('COT_REJECT', 'Cotizaciones', 'Rechazar cotizaciones', GETUTCDATE(), 'system'),
('COT_EXPORT', 'Cotizaciones', 'Exportar cotizaciones', GETUTCDATE(), 'system');

-- Permisos de Clientes
INSERT INTO Permisos (Codigo, Categoria, Descripcion, CreatedAt, CreatedBy) VALUES
('CLI_VIEW', 'Clientes', 'Ver clientes', GETUTCDATE(), 'system'),
('CLI_CREATE', 'Clientes', 'Crear nuevos clientes', GETUTCDATE(), 'system'),
('CLI_EDIT', 'Clientes', 'Editar clientes', GETUTCDATE(), 'system'),
('CLI_DELETE', 'Clientes', 'Eliminar clientes', GETUTCDATE(), 'system');

-- Permisos de Reportes
INSERT INTO Permisos (Codigo, Categoria, Descripcion, CreatedAt, CreatedBy) VALUES
('RPT_VIEW', 'Reportes', 'Ver reportes', GETUTCDATE(), 'system'),
('RPT_EXPORT', 'Reportes', 'Exportar reportes', GETUTCDATE(), 'system'),
('RPT_DASHBOARD', 'Reportes', 'Acceso al dashboard ejecutivo', GETUTCDATE(), 'system');

-- Permisos de Configuración
INSERT INTO Permisos (Codigo, Categoria, Descripcion, CreatedAt, CreatedBy) VALUES
('CFG_VIEW', 'Configuración', 'Ver configuración del sistema', GETUTCDATE(), 'system'),
('CFG_EDIT', 'Configuración', 'Editar configuración del sistema', GETUTCDATE(), 'system'),
('CFG_LOGS', 'Configuración', 'Ver logs del sistema', GETUTCDATE(), 'system');

GO

-- Verificar los permisos creados
SELECT Codigo, Categoria, Descripcion 
FROM Permisos 
ORDER BY Categoria, Codigo;
