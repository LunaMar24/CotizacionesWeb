-- ============================================
-- Script de verificación de estructura completa
-- Verifica que todas las llaves primarias y foreign keys estén funcionando
-- ============================================

-- 1. Verificar usuarios y roles
SELECT 
    '=== USUARIOS Y ROLES ===' as Seccion,
    NULL as UsuarioId, NULL as Nombre, NULL as Email, NULL as Roles;

SELECT 
    u.UsuarioId,
    u.Nombre,
    u.Email,
    STRING_AGG(r.Nombre, ', ') AS Roles
FROM Usuarios u
LEFT JOIN UsuarioRoles ur ON u.UsuarioId = ur.UsuarioId
LEFT JOIN Roles r ON ur.RolId = r.RolId
GROUP BY u.UsuarioId, u.Nombre, u.Email
ORDER BY u.UsuarioId;

-- 2. Verificar permisos por rol
SELECT 
    '=== PERMISOS POR ROL ===' as Info,
    NULL as RolId, NULL as RolNombre, NULL as CantidadPermisos;

SELECT 
    r.RolId,
    r.Nombre as RolNombre,
    COUNT(pr.PermisoId) as CantidadPermisos
FROM Roles r
LEFT JOIN PermisosRoles pr ON r.RolId = pr.RolId
GROUP BY r.RolId, r.Nombre
ORDER BY r.RolId;

-- 3. Verificar foreign keys están funcionando
SELECT 
    '=== VERIFICACIÓN FOREIGN KEYS ===' as Info,
    NULL as Tabla, NULL as FK_Campo, NULL as FK_Destino, NULL as Estado;

-- Verificar FK en tabla Usuarios (CreatedBy, ModifiedBy)
SELECT 
    'Usuarios' as Tabla,
    'CreatedBy/ModifiedBy' as FK_Campo,
    'Usuarios.UsuarioId' as FK_Destino,
    CASE WHEN COUNT(*) = 0 THEN 'ERROR: Sin registros' 
         ELSE 'OK: ' + CAST(COUNT(*) AS VARCHAR(10)) + ' registros'
    END as Estado
FROM Usuarios u1
INNER JOIN Usuarios u2 ON u1.CreatedBy = u2.UsuarioId;

-- Verificar FK en tabla Roles (CreatedBy, ModifiedBy)  
SELECT 
    'Roles' as Tabla,
    'CreatedBy/ModifiedBy' as FK_Campo,
    'Usuarios.UsuarioId' as FK_Destino,
    CASE WHEN COUNT(*) = 0 THEN 'ERROR: Sin registros'
         ELSE 'OK: ' + CAST(COUNT(*) AS VARCHAR(10)) + ' registros'
    END as Estado
FROM Roles r
INNER JOIN Usuarios u ON r.CreatedBy = u.UsuarioId;

-- Verificar FK en UsuarioRoles
SELECT 
    'UsuarioRoles' as Tabla,
    'UsuarioId+RolId' as FK_Campo,
    'Usuarios+Roles' as FK_Destino,
    CASE WHEN COUNT(*) = 0 THEN 'ERROR: Sin registros'
         ELSE 'OK: ' + CAST(COUNT(*) AS VARCHAR(10)) + ' registros'
    END as Estado
FROM UsuarioRoles ur
INNER JOIN Usuarios u ON ur.UsuarioId = u.UsuarioId
INNER JOIN Roles r ON ur.RolId = r.RolId;

-- Verificar FK en PermisosRoles
SELECT 
    'PermisosRoles' as Tabla,
    'PermisoId+RolId' as FK_Campo,
    'Permisos+Roles' as FK_Destino,
    CASE WHEN COUNT(*) = 0 THEN 'ERROR: Sin registros'
         ELSE 'OK: ' + CAST(COUNT(*) AS VARCHAR(10)) + ' registros'
    END as Estado
FROM PermisosRoles pr
INNER JOIN Permisos p ON pr.PermisoId = p.PermisoId
INNER JOIN Roles r ON pr.RolId = r.RolId;

-- 4. Verificar estructura de llaves primarias
SELECT 
    '=== LLAVES PRIMARIAS ===' as Info,
    NULL as Tabla, NULL as Columna_PK, NULL as Tipo, NULL as Identity_Actual;

SELECT 
    t.name AS Tabla,
    c.name AS Columna_PK,
    ty.name AS Tipo,
    CASE WHEN c.is_identity = 1 
         THEN 'IDENTITY(' + CAST(ic.seed_value AS VARCHAR(10)) + ',' + CAST(ic.increment_value AS VARCHAR(10)) + ')'
         ELSE 'NO IDENTITY'
    END AS Identity_Actual
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id AND i.is_primary_key = 1
INNER JOIN sys.index_columns ix ON i.object_id = ix.object_id AND i.index_id = ix.index_id
INNER JOIN sys.columns c ON ix.object_id = c.object_id AND ix.column_id = c.column_id
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
LEFT JOIN sys.identity_columns ic ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE t.name IN ('Usuarios', 'Roles', 'Permisos', 'UsuarioRoles', 'PermisosRoles', 
                 'Cotizacion', 'CotizacionVersion', 'DetalleCotizacionVersion', 
                 'Interesado', 'HistorialCotizacion', 'ArchivoCotizacion', 'Parametros')
ORDER BY t.name, ix.key_ordinal;

-- 5. Resumen final
SELECT 
    '=== RESUMEN FINAL ===' as Info,
    NULL as Descripcion, NULL as Valor;

SELECT 'Total Usuarios' as Descripcion, CAST(COUNT(*) AS VARCHAR(10)) as Valor FROM Usuarios
UNION ALL
SELECT 'Total Roles', CAST(COUNT(*) AS VARCHAR(10)) FROM Roles  
UNION ALL
SELECT 'Total Permisos', CAST(COUNT(*) AS VARCHAR(10)) FROM Permisos
UNION ALL
SELECT 'Total UsuarioRoles', CAST(COUNT(*) AS VARCHAR(10)) FROM UsuarioRoles
UNION ALL
SELECT 'Total PermisosRoles', CAST(COUNT(*) AS VARCHAR(10)) FROM PermisosRoles
UNION ALL
SELECT 'Total Cotizaciones', CAST(COUNT(*) AS VARCHAR(10)) FROM Cotizacion
UNION ALL
SELECT 'Total Interesados', CAST(COUNT(*) AS VARCHAR(10)) FROM Interesado
UNION ALL
SELECT 'Total Parametros', CAST(COUNT(*) AS VARCHAR(10)) FROM Parametros;

PRINT 'Verificación de estructura completada';
PRINT 'Revisa los resultados para confirmar que todo está funcionando correctamente';