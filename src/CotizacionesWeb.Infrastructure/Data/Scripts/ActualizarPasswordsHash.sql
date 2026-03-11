-- ============================================
-- Script para actualizar contraseñas a hash
-- ============================================

-- IMPORTANTE: Este script es para actualizar usuarios existentes
-- que tengan contraseñas en texto plano o sin hash

-- ============================================
-- PASO 1: Generar el hash usando el endpoint
-- ============================================
-- Visita (con la app corriendo):
-- https://localhost:5001/Account/GenerarHash?password=Admin123!
--
-- O usa: https://bcrypt-generator.com/ (11 rounds)

-- ============================================
-- PASO 2: Verificar usuarios sin hash válido
-- ============================================
SELECT 
    Id,
    Nombre,
    Email,
    CASE 
        WHEN PasswordHash IS NULL THEN '? NULL'
        WHEN PasswordHash = '' THEN '? VACÍO'
        WHEN LEN(PasswordHash) < 50 THEN '? TEXTO PLANO'
        ELSE '? HASH VÁLIDO'
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

-- ============================================
-- PASO 3: Actualizar contraseñas
-- ============================================

-- Opción A: Actualizar usuario específico
/*
UPDATE Usuarios 
SET PasswordHash = '$2a$11$TU_HASH_GENERADO_AQUI',
    IntentosFallidos = 0,
    ModifiedAt = GETUTCDATE(),
    ModifiedBy = 'system'
WHERE Email = 'usuario@ejemplo.com';
*/

-- Opción B: Establecer misma contraseña temporal para todos
/*
UPDATE Usuarios 
SET PasswordHash = '$2a$11$HASH_PASSWORD_TEMPORAL',
    IntentosFallidos = 0,
    ModifiedAt = GETUTCDATE(),
    ModifiedBy = 'system'
WHERE PasswordHash IS NULL 
   OR PasswordHash = '' 
   OR LEN(PasswordHash) < 50;
*/

-- ============================================
-- PASO 4: Verificar actualización
-- ============================================
SELECT 
    COUNT(*) AS TotalUsuarios,
    SUM(CASE WHEN LEN(PasswordHash) >= 50 THEN 1 ELSE 0 END) AS ConHashValido,
    SUM(CASE WHEN LEN(PasswordHash) < 50 OR PasswordHash IS NULL OR PasswordHash = '' THEN 1 ELSE 0 END) AS SinHash
FROM Usuarios;

SELECT 
    Id,
    Nombre,
    Email,
    LEFT(PasswordHash, 20) + '...' AS HashPreview,
    Activo,
    IntentosFallidos
FROM Usuarios
ORDER BY Email;

GO

