-- ============================================
-- Script para crear usuario administrador inicial
-- con contraseña hasheada
-- ============================================

-- PASO 1: Generar el hash de la contraseña
-- Visita en tu navegador (con la app corriendo):
-- https://localhost:5001/Account/GenerarHash?password=Admin123!
-- 
-- O usa este hash pre-generado para "Admin123!":
-- (Nota: Este hash es un ejemplo, genera uno nuevo usando el endpoint)

-- PASO 2: Insertar o actualizar usuario admin
DECLARE @HashAdmin NVARCHAR(500) = 'REEMPLAZAR_CON_HASH_GENERADO';

-- Verificar si existe el usuario admin
IF EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'admin@cotizaciones.com')
BEGIN
    -- Actualizar usuario existente
    UPDATE Usuarios
    SET PasswordHash = @HashAdmin,
        Activo = 1,
        IntentosFallidos = 0,
        ModifiedAt = GETUTCDATE(),
        ModifiedBy = 'system'
    WHERE Email = 'admin@cotizaciones.com';
    
    PRINT 'Usuario admin actualizado';
END
ELSE
BEGIN
    -- Insertar nuevo usuario admin
    INSERT INTO Usuarios (Nombre, Email, PasswordHash, Activo, IntentosFallidos, CreatedAt, CreatedBy)
    VALUES ('Administrador', 'admin@cotizaciones.com', @HashAdmin, 1, 0, GETUTCDATE(), 'system');
    
    PRINT 'Usuario admin creado';
    
    -- Obtener el ID del usuario recién creado
    DECLARE @UsuarioId INT = SCOPE_IDENTITY();
    
    -- Asignar rol de Admin si existe
    IF EXISTS (SELECT 1 FROM Roles WHERE Nombre = 'Admin' OR Nombre = 'Administrador')
    BEGIN
        DECLARE @RolId INT = (SELECT TOP 1 Id FROM Roles WHERE Nombre IN ('Admin', 'Administrador'));
        
        INSERT INTO UsuarioRoles (UsuarioId, RolId, CreatedAt, CreatedBy)
        VALUES (@UsuarioId, @RolId, GETUTCDATE(), 'system');
        
        PRINT 'Rol asignado al usuario admin';
    END
    ELSE
    BEGIN
        PRINT 'ADVERTENCIA: No existe el rol Admin/Administrador. Créalo primero.';
    END
END

-- PASO 3: Verificar
SELECT 
    u.Id,
    u.Nombre,
    u.Email,
    CASE WHEN LEN(u.PasswordHash) > 50 THEN 'HASH VÁLIDO' ELSE 'SIN HASH' END AS EstadoPassword,
    u.Activo,
    STRING_AGG(r.Nombre, ', ') AS Roles
FROM Usuarios u
LEFT JOIN UsuarioRoles ur ON u.Id = ur.UsuarioId
LEFT JOIN Roles r ON ur.RolId = r.Id
WHERE u.Email = 'admin@cotizaciones.com'
GROUP BY u.Id, u.Nombre, u.Email, u.PasswordHash, u.Activo;

GO

-- ============================================
-- CREAR ROL ADMINISTRADOR SI NO EXISTE
-- ============================================
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Nombre = 'Administrador')
BEGIN
    INSERT INTO Roles (Nombre, Descripcion, Activo, CreatedAt, CreatedBy)
    VALUES ('Administrador', 'Rol con acceso total al sistema', 1, GETUTCDATE(), 'system');
    
    PRINT 'Rol Administrador creado';
END

-- Asignar TODOS los permisos al rol Administrador
DECLARE @RolAdminId INT = (SELECT Id FROM Roles WHERE Nombre = 'Administrador');

IF @RolAdminId IS NOT NULL
BEGIN
    -- Eliminar permisos existentes
    DELETE FROM PermisosRoles WHERE RolId = @RolAdminId;
    
    -- Asignar todos los permisos
    INSERT INTO PermisosRoles (RolId, PermisoId, CreatedAt, CreatedBy)
    SELECT @RolAdminId, Id, GETUTCDATE(), 'system'
    FROM Permisos;
    
    PRINT 'Todos los permisos asignados al rol Administrador';
END

GO
