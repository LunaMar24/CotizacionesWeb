# Lineamientos Técnicos - Autenticación

Sistema usa autenticación propia (sin ASP.NET Identity).

## Estrategia
- Cookie Authentication
- Autorización basada en Roles

## Tabla Usuario
Campos mínimos:
- UsuarioId
- Nombre
- Email
- PasswordHash
- Activo
- IntentosFallidos
- BloqueadoHasta

## Seguridad
Hashing PBKDF2 con Rfc2898DeriveBytes.
Iteraciones mínimas: 100000.

## Claims
- NameIdentifier → UsuarioId
- Name → Nombre
- Role → Rol

## Reglas
- Nunca almacenar contraseñas en texto plano.
- No exponer errores técnicos al usuario.