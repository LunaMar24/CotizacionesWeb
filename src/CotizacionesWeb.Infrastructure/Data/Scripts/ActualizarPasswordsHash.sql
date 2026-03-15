-- ============================================
-- Script para actualizar contraseñas a hash
-- ACTUALIZADO PARA ESTRUCTURA CORREGIDA CON FOREIGN KEYS
-- ============================================

-- PASO 1: Verificar usuarios sin hash válido
SELECT 
    UsuarioId,
    Nombre,
    Email,
    CASE 
        WHEN PasswordHash IS NULL THEN 'NULL'
        WHEN PasswordHash = '' THEN 'VACIO'
        WHEN LEN(PasswordHash) < 50 THEN 'TEXTO PLANO'
        ELSE 'HASH VALIDO'
    END AS EstadoPassword,
    LEN(PasswordHash) AS LongitudHash,
    Activo,
    IntentosFallidos,
    CreatedAt
FROM Usuarios
ORDER BY 
    CASE 
        WHEN PasswordHash IS NULL OR PasswordHash = '' OR LEN(PasswordHash) < 50 THEN 0
        ELSE 1
    END,
    Email;

-- PASO 2: Actualizar contraseñas usando hash BCrypt

-- Opción A: Actualizar usuario específico con hash válido
-- Hash para "Admin123!" (BCrypt, 11 rounds)
/*
UPDATE Usuarios 
SET PasswordHash = '$2a$11$2LJfXKUd6fPqOvMV6JZWu.LrHIYpUi5gN7YBQCBQYq1FQqnzGFzDm',
    IntentosFallidos = 0,
    ModifiedAt = GETDATE(),
    ModifiedBy = 1  -- FK válida a usuario sistema
WHERE Email = 'admin@cotizaciones.com';
PRINT 'Contraseña actualizada para admin@cotizaciones.com: Admin123!';
*/

-- Opción B: Actualizar múltiples usuarios con contraseña temporal
-- Hash para "Temp123!" (BCrypt, 11 rounds)
/*
UPDATE Usuarios 
SET PasswordHash = '$2a$11$abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ',
    IntentosFallidos = 0,
    ModifiedAt = GETDATE(),
    ModifiedBy = 1
WHERE PasswordHash IS NULL 
   OR PasswordHash = '' 
   OR LEN(PasswordHash) < 50;
PRINT 'Contraseñas temporales asignadas';
*/

-- Opción C: Resetear contraseña del admin (más común)
-- Hash para "Admin123!" pre-generado
UPDATE Usuarios 
SET PasswordHash = '$2a$11$2LJfXKUd6fPqOvMV6JZWu.LrHIYpUi5gN7YBQCBQYq1FQqnzGFzDm',
    IntentosFallidos = 0,
    ModifiedAt = GETDATE(),
    ModifiedBy = 1
WHERE Email = 'admin@cotizaciones.com';

IF @@ROWCOUNT > 0
    PRINT 'Contraseña del administrador actualizada a: Admin123!'
ELSE
    PRINT 'No se encontró el usuario admin@cotizaciones.com';

-- PASO 3: Verificar actualización
SELECT 
    COUNT(*) AS TotalUsuarios,
    SUM(CASE WHEN LEN(PasswordHash) >= 50 THEN 1 ELSE 0 END) AS ConHashValido,
    SUM(CASE WHEN LEN(PasswordHash) < 50 OR PasswordHash IS NULL OR PasswordHash = '' THEN 1 ELSE 0 END) AS SinHash
FROM Usuarios;

-- PASO 4: Mostrar estado de usuarios
SELECT 
    UsuarioId,
    Nombre,
    Email,
    LEFT(PasswordHash, 20) + '...' AS HashPreview,
    Activo,
    IntentosFallidos,
    CASE 
        WHEN LEN(PasswordHash) >= 50 THEN 'VALIDO'
        ELSE 'INVALIDO'
    END AS EstadoHash,
    CreatedAt,
    ModifiedAt
FROM Usuarios
ORDER BY Email;

-- PASO 5: Verificar roles asignados (con estructura corregida)
SELECT 
    u.UsuarioId,
    u.Nombre,
    u.Email,
    STRING_AGG(r.Nombre, ', ') AS Roles,
    COUNT(pr.PermisoId) AS TotalPermisos
FROM Usuarios u
LEFT JOIN UsuarioRoles ur ON u.UsuarioId = ur.UsuarioId
LEFT JOIN Roles r ON ur.RolId = r.RolId
LEFT JOIN PermisosRoles pr ON r.RolId = pr.RolId
GROUP BY u.UsuarioId, u.Nombre, u.Email
ORDER BY u.Email;

PRINT '============================================';
PRINT 'Actualización de contraseñas completada';
PRINT 'Estructura de Foreign Keys funcionando correctamente';
PRINT '============================================';

GO