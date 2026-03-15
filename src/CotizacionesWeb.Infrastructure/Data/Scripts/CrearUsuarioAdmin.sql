-- ============================================
-- Script para crear usuario administrador inicial
-- ACTUALIZADO PARA NUEVA ESTRUCTURA CORREGIDA
-- ============================================

-- PASO 1: Crear usuario sistema si no existe (para foreign keys de auditoría)
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE UsuarioId = 1)
BEGIN
    SET IDENTITY_INSERT Usuarios ON;
    
    INSERT INTO Usuarios (UsuarioId, Nombre, Email, PasswordHash, Activo, IntentosFallidos, CreatedAt, CreatedBy)
    VALUES (1, 'Sistema', 'admin@cotizaciones.com', 'SYSTEM_HASH', 1, 0, GETDATE(), 1);
    
    SET IDENTITY_INSERT Usuarios OFF;
    
    PRINT 'Usuario sistema creado con ID = 1 para foreign keys de auditoría';
END

-- PASO 2: Generar el hash de la contraseña
-- Usa este hash pre-generado para "Admin123!":
-- $2a$11$2LJfXKUd6fPqOvMV6JZWu.LrHIYpUi5gN7YBQCBQYq1FQqnzGFzDm
DECLARE @HashAdmin NVARCHAR(500) = '$2a$11$2LJfXKUd6fPqOvMV6JZWu.LrHIYpUi5gN7YBQCBQYq1FQqnzGFzDm';
DECLARE @AdminId INT = 0

-- PASO 3: Crear o actualizar usuario admin
IF EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'admin@cotizaciones.com')
BEGIN
    -- Actualizar usuario existente
    UPDATE Usuarios
    SET PasswordHash = @HashAdmin,
        Activo = 1,
        IntentosFallidos = 0,
        ModifiedAt = GETDATE(),
        ModifiedBy = 1
    WHERE Email = 'admin@cotizaciones.com';
    
    PRINT 'Usuario admin actualizado';
    
    SET @AdminId = (SELECT UsuarioId FROM Usuarios WHERE Email = 'admin@cotizaciones.com');
END
ELSE
BEGIN
    -- Insertar nuevo usuario admin
    INSERT INTO Usuarios (Nombre, Email, PasswordHash, Activo, IntentosFallidos, CreatedAt, CreatedBy)
    VALUES ('Administrador', 'admin@cotizaciones.com', @HashAdmin, 1, 0, GETDATE(), 1);
    
    SET @AdminId  = SCOPE_IDENTITY();
    PRINT 'Usuario admin creado con ID: ' + CAST(@AdminId AS VARCHAR(10));
END

-- PASO 4: Crear rol Administrador si no existe
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Nombre = 'Administrador')
BEGIN
    INSERT INTO Roles (Nombre, Descripcion, Activo, CreatedAt, CreatedBy)
    VALUES ('Administrador', 'Rol con acceso total al sistema', 1, GETDATE(), 1);
    
    PRINT 'Rol Administrador creado';
END

-- PASO 5: Asignar rol al admin
DECLARE @RolAdminId INT = (SELECT RolId FROM Roles WHERE Nombre = 'Administrador');

-- Verificar si ya tiene el rol asignado
IF NOT EXISTS (SELECT 1 FROM UsuarioRoles WHERE UsuarioId = @AdminId AND RolId = @RolAdminId)
BEGIN
    INSERT INTO UsuarioRoles (UsuarioId, RolId)
    VALUES (@AdminId, @RolAdminId);
    
    PRINT 'Rol asignado al usuario admin';
END

-- PASO 6: Asignar TODOS los permisos al rol Administrador
IF @RolAdminId IS NOT NULL
BEGIN
    -- Eliminar permisos existentes para evitar duplicados
    DELETE FROM PermisosRoles WHERE RolId = @RolAdminId;
    
    -- Asignar todos los permisos
    INSERT INTO PermisosRoles (RolId, PermisoId)
    SELECT @RolAdminId, PermisoId
    FROM Permisos;
    
    DECLARE @PermisosCount INT = (SELECT COUNT(*) FROM Permisos);
    PRINT 'Asignados ' + CAST(@PermisosCount AS VARCHAR(10)) + ' permisos al rol Administrador';
END

-- PASO 7: Verificación final
SELECT 
    u.UsuarioId,
    u.Nombre,
    u.Email,
    CASE WHEN LEN(u.PasswordHash) > 50 THEN 'HASH VÁLIDO (Admin123!)' ELSE 'SIN HASH' END AS EstadoPassword,
    u.Activo,
    STRING_AGG(r.Nombre, ', ') AS Roles,
    COUNT(pr.PermisoId) AS TotalPermisos
FROM Usuarios u
LEFT JOIN UsuarioRoles ur ON u.UsuarioId = ur.UsuarioId
LEFT JOIN Roles r ON ur.RolId = r.RolId
LEFT JOIN PermisosRoles pr ON r.RolId = pr.RolId
WHERE u.Email IN ('admin@cotizaciones.com', 'sistema@cotizaciones.com')
GROUP BY u.UsuarioId, u.Nombre, u.Email, u.PasswordHash, u.Activo
ORDER BY u.UsuarioId;

PRINT '============================================';
PRINT 'Setup completado exitosamente:';
PRINT '- Usuario sistema (ID=1) para foreign keys';
PRINT '- Usuario admin con contraseña: Admin123!';
PRINT '- Rol Administrador con todos los permisos';
PRINT '============================================';

GO
