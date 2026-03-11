# SISTEMA DE PERMISOS - Documentación Completa

---

## ?? COMPONENTES DEL SISTEMA

### 1. Application Layer

**IPermisoService** (`Application/Permisos/IPermisoService.cs`):
```csharp
public interface IPermisoService
{
    Task<List<PermisoDto>> GetAllAsync();
    Task<List<string>> GetUsuarioPermisosCodigosAsync(int usuarioId);
    Task<bool> UsuarioTienePermisoAsync(int usuarioId, string codigoPermiso);
}

public record PermisoDto(int Id, string Codigo, string Categoria, string Descripcion);
```

### 2. Infrastructure Layer

**PermisoService** (`Infrastructure/Services/PermisoService.cs`):
- Consulta permisos a través de: Usuario ? UsuarioRol ? Rol ? PermisoRol ? Permiso
- Usa LINQ para obtener códigos únicos de todos los roles del usuario
- Ordena por Categoría y luego por Código

### 3. UI Layer - Services

**IPermisoChecker** (`UI/Services/PermisoChecker.cs`):
```csharp
public interface IPermisoChecker
{
    Task<bool> TienePermisoAsync(string codigoPermiso);
    Task<List<string>> GetPermisosUsuarioActualAsync();
}
```
- Obtiene usuario actual desde HttpContext
- Usa IPermisoService para verificar permisos

### 4. UI Layer - Helpers

**UserHelper** (`UI/Helpers/UserHelper.cs`):
```csharp
public static int GetUsuarioId(this ClaimsPrincipal user)
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    return int.TryParse(userIdClaim?.Value, out int userId) ? userId : 0;
}
```

### 5. UI Layer - Tag Helpers

**RequierePermisoTagHelper** (`UI/TagHelpers/RequierePermisoTagHelper.cs`):
- Atributo: `requiere-permiso="CODIGO_PERMISO"`
- Atributo: `tipo-restriccion="deshabilitar"` o `"ocultar"`
- Por defecto: deshabilita el elemento (disabled, opacity 0.5)

**Uso**:
```html
<button requiere-permiso="USR_DELETE" class="btn btn-danger">Eliminar</button>
<div requiere-permiso="USR_EDIT" tipo-restriccion="ocultar">...</div>
```

### 6. UI Layer - Filters

**RequierePermisoAttribute** (`UI/Filters/RequierePermisoAttribute.cs`):
- Atributo para proteger acciones del controller
- Uso: `[RequierePermiso("CODIGO_PERMISO")]`
- Retorna 401 (Unauthorized) si no está autenticado
- Retorna 403 (Forbidden) si no tiene el permiso

**Uso**:
```csharp
[RequierePermiso("USR_CREATE")]
public async Task<IActionResult> Create() { }
```

---

## ?? PERMISOS DEFINIDOS

### Usuarios (6 permisos)
- `USR_VIEW`: Ver usuarios del sistema
- `USR_CREATE`: Crear nuevos usuarios
- `USR_EDIT`: Editar usuarios existentes
- `USR_DELETE`: Eliminar usuarios
- `USR_ROLES`: Gestionar roles de usuarios
- `USR_RESET_PWD`: Resetear contraseñas

### Roles (5 permisos)
- `ROL_VIEW`: Ver roles del sistema
- `ROL_CREATE`: Crear nuevos roles
- `ROL_EDIT`: Editar roles existentes
- `ROL_DELETE`: Eliminar roles
- `ROL_PERMISOS`: Gestionar permisos de roles

### Cotizaciones (7 permisos)
- `COT_VIEW`: Ver cotizaciones
- `COT_CREATE`: Crear nuevas cotizaciones
- `COT_EDIT`: Editar cotizaciones
- `COT_DELETE`: Eliminar cotizaciones
- `COT_APPROVE`: Aprobar cotizaciones
- `COT_REJECT`: Rechazar cotizaciones
- `COT_EXPORT`: Exportar cotizaciones

### Clientes (4 permisos)
- `CLI_VIEW`: Ver clientes
- `CLI_CREATE`: Crear nuevos clientes
- `CLI_EDIT`: Editar clientes
- `CLI_DELETE`: Eliminar clientes

### Reportes (3 permisos)
- `RPT_VIEW`: Ver reportes
- `RPT_EXPORT`: Exportar reportes
- `RPT_DASHBOARD`: Acceso al dashboard ejecutivo

### Configuración (3 permisos)
- `CFG_VIEW`: Ver configuración del sistema
- `CFG_EDIT`: Editar configuración del sistema
- `CFG_LOGS`: Ver logs del sistema

---

## ?? FLUJO DE VERIFICACIÓN

### En Controllers
```
1. Usuario hace request a una acción
2. [RequierePermiso("CODIGO")] intercepta
3. Extrae UsuarioId de ClaimTypes.NameIdentifier
4. Consulta IPermisoService.UsuarioTienePermisoAsync()
5. Si tiene permiso: ejecuta acción
6. Si NO tiene: retorna 403 Forbidden
```

### En Vistas (Tag Helper)
```
1. Tag Helper detecta requiere-permiso="CODIGO"
2. Llama a PermisoChecker.TienePermisoAsync()
3. PermisoChecker obtiene usuario del HttpContext
4. Consulta permisos en base de datos
5. Si NO tiene permiso:
   - tipo-restriccion="deshabilitar": Agrega disabled, opacity 0.5
   - tipo-restriccion="ocultar": SuppressOutput() (no renderiza)
```

### En Layout (Menús)
```
1. @inject IPermisoChecker en Layout
2. Verifica permisos antes del @if
3. Calcula variables: tienePermiso = await PermisoChecker.TienePermisoAsync()
4. Si NO tiene ningún permiso en sección: oculta menú completo
5. Si tiene algún permiso: muestra menú, oculta items sin permiso
```

---

## ?? DISEÑO VISUAL DE PERMISOS

### Agrupación por Categoría
```razor
@{
    var permisosPorCategoria = Model.PermisosDisponibles
        .GroupBy(p => p.Categoria)
        .OrderBy(g => g.Key);
}

@foreach (var grupo in permisosPorCategoria)
{
    <div class="categoria-group">
        <div class="categoria-header" title="Click para seleccionar todos">
            <i class="fas fa-folder"></i>
            <strong>@grupo.Key</strong>
            <span class="badge">@grupo.Count() permisos</span>
        </div>
        
        @foreach (var p in grupo.OrderBy(x => x.Codigo))
        {
            <div class="permiso-item">
                <input type="checkbox" name="permisosIds" value="@p.Id">
                <label>
                    <div class="permiso-codigo">
                        <i class="fas fa-lock"></i>
                        <strong>@p.Codigo</strong>
                    </div>
                    <div class="permiso-descripcion">
                        @p.Descripcion
                    </div>
                </label>
            </div>
        }
    </div>
}
```

### JavaScript - Seleccionar Categoría Completa
```javascript
$(document).on('click', '.categoria-header', function(e) {
    if (!$(e.target).hasClass('badge')) {
        const categoriaGroup = $(this).closest('.categoria-group');
        const checkboxes = categoriaGroup.find('.permiso-checkbox');
        const allChecked = checkboxes.filter(':checked').length === checkboxes.length;
        
        checkboxes.prop('checked', !allChecked);
        
        $(this).css('opacity', '0.8');
        setTimeout(() => $(this).css('opacity', '1'), 200);
    }
});
```

---

## ?? COMPORTAMIENTO POR TIPO DE USUARIO

### Administrador (Roles: "Admin" o "Administrador")
- ? **Bypass completo**: Siempre tiene acceso a todo
- ? Ve todos los menús
- ? Todos los botones habilitados
- ? No necesita permisos específicos

### Usuario con Permisos Específicos
- ? Solo ve menús donde tiene al menos un permiso
- ? Botones se deshabilitan si no tiene el permiso
- ? Acciones del controller protegidas
- ? Puede tener múltiples roles (permisos se acumulan)

### Usuario sin Permisos
- ? No ve el menú si no tiene ningún permiso en esa sección
- ? Botones deshabilitados con tooltip "No tiene permisos"
- ? Acceso denegado en controllers (403 Forbidden)

---

## ?? ARCHIVOS CREADOS

### Nuevos Archivos
1. `Application/Permisos/IPermisoService.cs`
2. `Infrastructure/Services/PermisoService.cs`
3. `Infrastructure/Data/Interceptors/AuditInterceptor.cs`
4. `UI/Helpers/UserHelper.cs`
5. `UI/Services/PermisoChecker.cs`
6. `UI/TagHelpers/RequierePermisoTagHelper.cs`
7. `UI/Filters/RequierePermisoAttribute.cs`

### Scripts SQL
1. `Infrastructure/Data/Scripts/SeedPermisos.sql`
2. `Infrastructure/Data/Scripts/CrearUsuarioAdmin.sql`
3. `Infrastructure/Data/Scripts/ActualizarPasswordsHash.sql`

### Archivos Modificados
1. `Application/Authentication/LoginModels.cs` - Agregado UsuarioId
2. `Infrastructure/Security/AuthService.cs` - Retorna UsuarioId, quita bypass
3. `UI/Controllers/AccountController.cs` - Agrega claim NameIdentifier
4. `UI/Program.cs` - Registra servicios y AuditInterceptor
5. `UI/Views/_ViewImports.cshtml` - Registra Tag Helpers
6. `UI/Views/Shared/_Layout.cshtml` - Menús dinámicos con permisos
7. `UI/Controllers/UsuariosController.cs` - 9 atributos [RequierePermiso]
8. `UI/Controllers/RolesController.cs` - 8 atributos [RequierePermiso]
9. `UI/Views/Usuarios/Index.cshtml` - Botones con requiere-permiso
10. `UI/Views/Roles/Index.cshtml` - Botones con requiere-permiso

---

## ?? TROUBLESHOOTING

### Si los botones no se deshabilitan:
1. Verificar que Tag Helper esté registrado en `_ViewImports.cshtml`
2. Verificar que `PermisoChecker` esté registrado en `Program.cs`
3. Verificar que el usuario tenga roles asignados en BD
4. Verificar que los roles tengan permisos asignados
5. Hacer logout/login para renovar claims

### Si devuelve "UsuarioId = 0":
1. Hacer logout
2. Login nuevamente
3. Verificar que `LoginResult` incluye `UsuarioId`
4. Verificar que `AccountController` crea claim `NameIdentifier`

### Si los menús no se ocultan:
1. Verificar que `@inject IPermisoChecker` esté en Layout
2. Verificar que las variables se calculen FUERA de los bloques `@if`
3. Verificar sintaxis Razor (no usar `@{ }` dentro de otro `@if`)
4. Verificar que `HttpContextAccessor` esté registrado

### Si devuelve 403 Forbidden:
1. Verificar que el usuario tenga el permiso en su rol
2. Ejecutar `SeedPermisos.sql` si no hay permisos en BD
3. Asignar permisos al rol desde la pantalla de Roles
4. Verificar que el atributo `[RequierePermiso]` tenga el código correcto

### Si auditoría sigue usando "system":
1. Verificar que `AuditInterceptor` esté configurado en Program.cs
2. Verificar que usuario esté autenticado
3. Verificar que claim `Email` exista
4. Reiniciar la aplicación (no Hot Reload para interceptores)

---

## ?? EJEMPLOS COMPLETOS

### Ejemplo 1: Proteger Controller Completo
```csharp
[Authorize(Roles = "Admin,Administrador")]
public class UsuariosController : Controller
{
    [RequierePermiso("USR_VIEW")]
    public async Task<IActionResult> Index() { }
    
    [RequierePermiso("USR_CREATE")]
    public IActionResult Create() => View();
    
    [HttpPost]
    [RequierePermiso("USR_CREATE")]
    public async Task<IActionResult> Create(CreateViewModel model) { }
    
    [RequierePermiso("USR_EDIT")]
    public async Task<IActionResult> Edit(int id) { }
    
    [HttpPost]
    [RequierePermiso("USR_EDIT")]
    public async Task<IActionResult> Edit(EditViewModel model) { }
    
    [HttpPost]
    [RequierePermiso("USR_DELETE")]
    public async Task<IActionResult> Delete(int id) { }
}
```

### Ejemplo 2: Vista con Permisos
```razor
<!-- Botón crear (deshabilitado sin permiso) -->
<a asp-action="Create" 
   class="btn btn-primary"
   requiere-permiso="USR_CREATE">
    <i class="fas fa-plus"></i> CREAR USUARIO
</a>

<!-- Botones de acción en tabla -->
<div class="btn-group">
    <button class="btn btn-sm btn-warning"
            requiere-permiso="USR_EDIT">
        <i class="fas fa-edit"></i>
    </button>
    
    <button class="btn btn-sm btn-danger"
            requiere-permiso="USR_DELETE">
        <i class="fas fa-trash"></i>
    </button>
</div>

<!-- Sección completa oculta sin permiso -->
<div requiere-permiso="USR_ROLES" tipo-restriccion="ocultar">
    <h3>Gestión de Roles</h3>
    <button class="btn btn-primary">Asignar Roles</button>
</div>
```

### Ejemplo 3: Layout con Menús Dinámicos
```razor
@using CotizacionesWeb.UI.Services
@inject IPermisoChecker PermisoChecker

@{
    var isAuthenticated = User.Identity?.IsAuthenticated == true;
    var tieneUsuariosView = isAuthenticated && await PermisoChecker.TienePermisoAsync("USR_VIEW");
    var tieneRolesView = isAuthenticated && await PermisoChecker.TienePermisoAsync("ROL_VIEW");
    var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
    var tieneAdminMenu = tieneUsuariosView || tieneRolesView || esAdmin;
}

@if (tieneAdminMenu)
{
    <li class="nav-item">
        <a href="#" class="nav-link">
            <i class="nav-icon fas fa-users-cog"></i>
            <p>Administración <i class="right fas fa-angle-left"></i></p>
        </a>
        <ul class="nav nav-treeview">
            @if (tieneUsuariosView || esAdmin)
            {
                <li class="nav-item">
                    <a asp-controller="Usuarios" asp-action="Index" class="nav-link">
                        <i class="far fa-circle nav-icon"></i>
                        <p>Usuarios</p>
                    </a>
                </li>
            }
            @if (tieneRolesView || esAdmin)
            {
                <li class="nav-item">
                    <a asp-controller="Roles" asp-action="Index" class="nav-link">
                        <i class="far fa-circle nav-icon"></i>
                        <p>Roles</p>
                    </a>
                </li>
            }
        </ul>
    </li>
}
```

---

## ??? SCRIPTS SQL

### SeedPermisos.sql
Crea los 28 permisos iniciales en 6 categorías.

**Ejecutar primero** para tener permisos en el sistema.

### CrearUsuarioAdmin.sql
Crea:
1. Usuario `admin@cotizaciones.com` con contraseña hasheada
2. Rol `Administrador` si no existe
3. Asigna TODOS los permisos al rol Administrador
4. Asigna el rol al usuario admin

**?? Requiere**: Reemplazar `REEMPLAZAR_CON_HASH_GENERADO` con hash real.

### ActualizarPasswordsHash.sql
Script de ayuda para:
- Identificar usuarios con contraseñas sin hash
- Actualizar contraseñas a formato hash
- Resetear intentos fallidos

---

## ?? GUÍA DE CONFIGURACIÓN INICIAL

### PASO 1: Generar Hash
```
1. Iniciar aplicación
2. Visitar: https://localhost:5001/Account/GenerarHash?password=Admin123!
3. Copiar el hash generado (60+ caracteres)
```

### PASO 2: Poblar Permisos
```sql
-- Ejecutar en SQL Server:
-- src/CotizacionesWeb.Infrastructure/Data/Scripts/SeedPermisos.sql
```

### PASO 3: Crear Usuario Admin
```sql
-- 1. Abrir: CrearUsuarioAdmin.sql
-- 2. Buscar: REEMPLAZAR_CON_HASH_GENERADO
-- 3. Reemplazar con el hash del PASO 1
-- 4. Ejecutar el script completo
```

### PASO 4: Verificar
```
1. Login: admin@cotizaciones.com / Admin123!
2. Ir a: Administración ? Roles
3. Ver permisos del rol "Administrador"
4. Debe tener los 28 permisos asignados
```

### PASO 5: Probar Sistema
```
1. Crear rol "Operador" con permisos limitados (solo VIEW)
2. Crear usuario de prueba con rol Operador
3. Logout
4. Login con usuario de prueba
5. Verificar:
   - Botones de crear/editar/eliminar deshabilitados
   - Menús sin permisos ocultos
   - Si intenta URL directa: 403 Forbidden
```

---

## ??? JERARQUÍA DE AUTORIZACIÓN

```
Usuario con rol Admin/Administrador
  ?? Bypass completo (siempre tiene acceso)
  
Usuario Normal
  ?? Verifica permiso específico
      ?? Tiene permiso ? Acceso permitido
      ?? No tiene permiso ? Elemento deshabilitado/oculto o 403
```

---

## ?? NOTAS IMPORTANTES

1. **Claims persistentes**: Los usuarios deben hacer logout/login después de cambios en estructura de claims
2. **Admin bypass**: Roles Admin/Administrador NO necesitan permisos asignados
3. **Permisos acumulativos**: Si usuario tiene múltiples roles, obtiene todos los permisos
4. **Email en auditoría**: CreatedBy/ModifiedBy almacenan el EMAIL, no el nombre
5. **System fallback**: Operaciones sin usuario autenticado usan "system"
6. **Interceptor automático**: No necesitas código manual en servicios para auditoría

---

## ?? REFERENCIAS

### Componentes Clave
- `IPermisoService`: Consultas de permisos en BD
- `IPermisoChecker`: Verificación de permisos en UI
- `RequierePermisoAttribute`: Protección de controllers
- `RequierePermisoTagHelper`: Control de elementos en vistas
- `AuditInterceptor`: Auditoría automática
- `UserHelper`: Extensiones para ClaimsPrincipal

### Archivos de Configuración
- `Program.cs`: Registros y configuración del interceptor
- `_ViewImports.cshtml`: Tag Helpers y usings globales
- `_Layout.cshtml`: Menús dinámicos con lógica de permisos

---

**Versión**: 1.1  
**Última actualización**: 10 de marzo de 2026, 7:50 PM  
**Autor**: Marcela Jiménez (con GitHub Copilot)
