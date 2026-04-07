# ?? Entity Framework Core - Comandos Code First

Guía completa de comandos para trabajar con **EF Core Code First** en el proyecto CotizacionesWeb.

---

## ?? Instalación y Verificación

### Verificar versión de EF Core Tools

```sh
dotnet ef --version
```

### Instalar EF Core Tools globalmente

```sh
dotnet tool install --global dotnet-ef --version 8.0.14
```

### Actualizar a la última versión

```sh
dotnet tool update --global dotnet-ef
```

---

## ?? Crear Migraciones

### Crear nueva migración

```sh
dotnet ef migrations add <NombreDeLaMigracion> \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output-dir Data/Migrations
```

**Ejemplos prácticos:**

```sh
# Agregar campo a entidad existente
dotnet ef migrations add AgregarTelefonoAUsuario \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output-dir Data/Migrations

# Crear nueva entidad
dotnet ef migrations add AgregarEntidadProducto \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output-dir Data/Migrations

# Modificar relaciones
dotnet ef migrations add ActualizarRelacionUsuarioRol \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output-dir Data/Migrations

# Actualizar configuraciones (ejemplo real del proyecto)
dotnet ef migrations add CorregirCheckConstraintsObsoletos \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output-dir Data/Migrations
```

**¿Cuándo crear una migración?**
- ? Agregar/quitar propiedades de entidades
- ? Crear nuevas entidades
- ? Cambiar tipos de datos
- ? Modificar relaciones entre entidades
- ? Agregar/quitar índices

---

## ?? Aplicar Migraciones

### Aplicar todas las migraciones pendientes

```sh
dotnet ef database update \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

### Aplicar hasta una migración específica

```sh
dotnet ef database update <NombreMigracion> \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

### Revertir a una migración anterior

```sh
dotnet ef database update <MigracionAnterior> \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

**Ejemplo:**
```sh
# Volver a la migración "InitialCreate"
dotnet ef database update InitialCreate \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

---

## ?? Consultar Migraciones

### Ver lista de migraciones

```sh
dotnet ef migrations list \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

### Ver lista detallada

```sh
dotnet ef migrations list --verbose \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

**Salida ejemplo:**
```
20240115120000_InitialCreate (Applied)
20240120140000_AgregarCampoTelefono (Applied)
20240125150000_AgregarEntidadProducto (Applied)
20260322213707_MoverSoloMonedaACotizacion (Applied)
20260405235622_AgregarNotificacionCotizacion (Applied)
20260406120000_CorregirCheckConstraintsObsoletos (Pending)
```

---

## ? Eliminar Migraciones

### Eliminar la última migración (NO aplicada)

```sh
dotnet ef migrations remove \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

### Forzar eliminación (usar con precaución)

```sh
dotnet ef migrations remove --force \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

?? **IMPORTANTE:** 
- Solo puedes eliminar migraciones que **NO** se hayan aplicado a la base de datos
- Si ya aplicaste la migración, primero debes revertirla con `database update <MigracionAnterior>`

---

## ?? Generar Scripts SQL

### Generar SQL de todas las migraciones

```sh
dotnet ef migrations script \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output migration.sql
```

### Generar SQL incremental (entre dos migraciones)

```sh
dotnet ef migrations script <MigracionOrigen> <MigracionDestino> \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output incremental.sql
```

### Generar SQL idempotente (puede ejecutarse múltiples veces)

```sh
dotnet ef migrations script --idempotent \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output idempotent.sql
```

**¿Cuándo usar scripts SQL?**
- ?? Para revisión de DBAs
- ?? Para aplicar manualmente en producción
- ?? Para documentación de cambios
- ?? Cuando no tienes acceso directo a la BD de producción

---

## ??? Gestión de Base de Datos

### Eliminar base de datos completa

```sh
dotnet ef database drop \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

### Eliminar sin confirmación (forzar)

```sh
dotnet ef database drop --force \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

?? **PELIGRO:** Este comando elimina **TODA** la base de datos. Úsalo solo en desarrollo.

### Revertir todas las migraciones

```sh
dotnet ef database update 0 \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

---

## ?? Información del DbContext

### Ver información del DbContext

```sh
dotnet ef dbcontext info \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

### Listar todos los DbContext

```sh
dotnet ef dbcontext list \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI
```

---

## ?? Migraciones con Datos Iniciales (Seed)

### Crear migración vacía para seed data

```sh
dotnet ef migrations add SeedInitialData \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output-dir Data/Migrations
```

### Editar la migración generada

```csharp
public partial class SeedInitialData : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Roles",
            columns: new[] { "Nombre", "CreatedAt", "CreatedBy" },
            values: new object[,]
            {
                { "Admin", DateTime.UtcNow, "system" },
                { "Usuario", DateTime.UtcNow, "system" }
            }
        );
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Roles",
            keyColumn: "Nombre",
            keyValues: new object[] { "Admin", "Usuario" }
        );
    }
}
```

---

## ?? Flujos de Trabajo Comunes

### Flujo 1: Agregar nueva propiedad a entidad

```sh
# 1. Editar la entidad (ej: Usuario.cs)
# Agregar: public string? Telefono { get; set; }

# 2. Crear migración
dotnet ef migrations add AgregarTelefonoAUsuario \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output-dir Data/Migrations

# 3. Revisar archivo generado en Data/Migrations/ (opcional)

# 4. Aplicar a base de datos
dotnet ef database update \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

### Flujo 2: Crear nueva entidad

```sh
# 1. Crear clase en Domain/Entities/Producto.cs

# 2. Agregar DbSet en DbContextCotizaciones.cs
# public DbSet<Producto> Productos => Set<Producto>();

# 3. Crear migración
dotnet ef migrations add AgregarEntidadProducto \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output-dir Data/Migrations

# 4. Aplicar
dotnet ef database update \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

### Flujo 3: Corregir migración aplicada incorrectamente

```sh
# 1. Revertir a migración anterior
dotnet ef database update <MigracionAnterior> \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones

# 2. Eliminar migración incorrecta
dotnet ef migrations remove \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones

# 3. Corregir código

# 4. Crear nueva migración
dotnet ef migrations add MigracionCorregida \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output-dir Data/Migrations

# 5. Aplicar
dotnet ef database update \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

### Flujo 5: Corregir warnings de Entity Framework Core

```sh
# Ejemplo real: Corregir HasCheckConstraint obsoleto
# 1. Identificar warnings en build
# ?? Warning: HasCheckConstraint is obsolete, use ToTable(t => t.HasCheckConstraint()) instead

# 2. Corregir archivos de configuración (ej: ParametrosConfiguration.cs)
# Cambiar: builder.HasCheckConstraint("nombre", "expresion");
# Por:     builder.ToTable("tabla", t => t.HasCheckConstraint("nombre", "expresion"));

# 3. Verificar que compila
dotnet build src/CotizacionesWeb.Infrastructure

# 4. Crear migración para aplicar cambios (si es necesario)
dotnet ef migrations add CorregirCheckConstraintsObsoletos \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output-dir Data/Migrations

# 5. Aplicar si hay cambios en schema
dotnet ef database update \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

### Flujo 6: Resetear base de datos completamente

```sh
# Opción A: Eliminar BD y recrear
dotnet ef database drop --force \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones

dotnet ef database update \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones

# Opción B: Revertir todas las migraciones y reaplicar
dotnet ef database update 0 \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones

dotnet ef database update \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

---

## ?? Solución de Problemas Comunes

### Error: "Build failed"

**Problema:** El proyecto no compila.

**Solución:**
```sh
# Compilar manualmente primero
dotnet build src/CotizacionesWeb.Infrastructure
dotnet build src/CotizacionesWeb.UI

# Si hay errores de warnings como HasCheckConstraint obsoleto
# Corregir las configuraciones de Entity Framework primero
# Luego ejecutar el comando EF Core
```

### Error: "No DbContext was found"

**Problema:** EF no encuentra el DbContext.

**Solución:**
- Verifica que `--context DbContextCotizaciones` esté correcto
- Asegúrate de estar en el directorio raíz del proyecto
- Verifica que el proyecto Infrastructure tenga el DbContext

### Error: "Unable to create an object of type 'DbContextCotizaciones'"

**Problema:** Falta la cadena de conexión.

**Solución:**
- Verifica que `appsettings.json` tenga la ConnectionString
- Asegúrate de especificar `--startup-project src/CotizacionesWeb.UI`

### Error: "HasCheckConstraint is obsolete" Warning

**Problema:** Entity Framework Core 8 marca `HasCheckConstraint` como obsoleto.

**Solución:**
```sh
# 1. Identificar archivos de configuración con el warning
# Buscar en: src/CotizacionesWeb.Infrastructure/Data/Configurations/*.cs

# 2. Cambiar el patrón obsoleto:
# ANTES: 
# builder.HasCheckConstraint("CK_Nombre", "[Campo] IN ('A', 'B')");

# DESPUÉS:
# builder.ToTable("NombreTabla", t => 
#     t.HasCheckConstraint("CK_Nombre", "[Campo] IN ('A', 'B')"));

# 3. Compilar para verificar
dotnet build src/CotizacionesWeb.Infrastructure

# 4. Los check constraints seguirán funcionando igual
```

**Problema:** Intentas eliminar una migración ya aplicada.

**Solución:**
```sh
# Primero revertir la migración
dotnet ef database update <MigracionAnterior> \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones

# Luego eliminar
dotnet ef migrations remove \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

---

## ?? Alias de PowerShell (Opcional)

Crea un archivo `ef-aliases.ps1` en la raíz del proyecto:

```powershell
# Alias para comandos EF Core
function ef-migrate {
    param([string]$name)
    dotnet ef migrations add $name `
        --project src/CotizacionesWeb.Infrastructure `
        --startup-project src/CotizacionesWeb.UI `
        --context DbContextCotizaciones `
        --output-dir Data/Migrations
}

function ef-update {
    dotnet ef database update `
        --project src/CotizacionesWeb.Infrastructure `
        --startup-project src/CotizacionesWeb.UI `
        --context DbContextCotizaciones
}

function ef-list {
    dotnet ef migrations list `
        --project src/CotizacionesWeb.Infrastructure `
        --startup-project src/CotizacionesWeb.UI `
        --context DbContextCotizaciones
}

function ef-remove {
    dotnet ef migrations remove `
        --project src/CotizacionesWeb.Infrastructure `
        --startup-project src/CotizacionesWeb.UI `
        --context DbContextCotizaciones
}

function ef-drop {
    dotnet ef database drop --force `
        --project src/CotizacionesWeb.Infrastructure `
        --startup-project src/CotizacionesWeb.UI `
        --context DbContextCotizaciones
}

function ef-info {
    dotnet ef dbcontext info `
        --project src/CotizacionesWeb.Infrastructure `
        --startup-project src/CotizacionesWeb.UI `
        --context DbContextCotizaciones
}
```

**Uso:**
```powershell
# Cargar aliases (ejecutar una vez por sesión)
. .\ef-aliases.ps1

# Usar comandos simplificados
ef-migrate "AgregarTelefono"
ef-update
ef-list
ef-remove
ef-drop
ef-info
```

---

## ? Checklist Pre-Migración

Antes de crear y aplicar una migración, verifica:

- [ ] **Compilación exitosa:** El proyecto compila sin errores
- [ ] **ConnectionString correcta:** `appsettings.json` tiene la cadena de conexión válida
- [ ] **Backup (producción):** Si es producción, haz backup de la BD
- [ ] **Revisar código generado:** Verifica el archivo de migración antes de aplicar
- [ ] **Probar en desarrollo:** Aplica primero en ambiente de desarrollo
- [ ] **Commit de código:** Asegúrate de versionar la migración en Git
- [ ] **Documentar cambios:** Agrega comentarios descriptivos si es necesario

---

## ?? Recursos Adicionales

- [Documentación oficial de EF Core](https://docs.microsoft.com/ef/core/)
- [Migraciones en EF Core](https://docs.microsoft.com/ef/core/managing-schemas/migrations/)
- [Code First con EF Core](https://docs.microsoft.com/ef/core/modeling/)

---

## ?? Notas del Proyecto

### Configuración actual:
- **Provider:** SQL Server
- **DbContext:** `DbContextCotizaciones`
- **Proyecto Infrastructure:** `src/CotizacionesWeb.Infrastructure`
- **Proyecto Startup:** `src/CotizacionesWeb.UI`
- **Directorio Migraciones:** `Data/Migrations`

### Entidades actuales:
- `Usuario` - Usuarios del sistema con autenticación
- `Rol` - Roles para autorización
- `UsuarioRol` - Relación many-to-many entre usuarios y roles
- `Permiso` - Permisos específicos del sistema
- `PermisoRol` - Relación entre roles y permisos
- `Interesado` - Clientes/prospectos desde HubSpot
- `Cotizacion` - Cotizaciones principales
- `CotizacionVersion` - Versiones de cotizaciones
- `DetalleCotizacionVersion` - Líneas de productos por versión
- `HistorialCotizacion` - Auditoría de cambios de estado
- `ArchivoCotizacion` - Archivos adjuntos a cotizaciones
- `NotificacionCotizacion` - Sistema de notificaciones por email
- `Parametros` - Parámetros de configuración del sistema

---

**Última actualización:** Abril 2026  
**Versión .NET:** 8.0  
**Versión EF Core:** 8.0.14
