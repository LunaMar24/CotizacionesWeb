# Lineamientos Técnicos - Migraciones EF Core

## Estrategia
- EF Core Code First
- Migraciones controladas por código

## Ubicación
Infrastructure/Data/Migrations

## Comandos
Agregar migración:
dotnet ef migrations add NombreMigracion

Actualizar DB:
dotnet ef database update

Script producción:
dotnet ef migrations script --idempotent

## Reglas
- No modificar estructura directamente en producción.
- No editar migraciones ya aplicadas.
- Crear migraciones correctivas si es necesario.