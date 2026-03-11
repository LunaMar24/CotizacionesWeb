# CONTEXTO DEL PROYECTO - CotizacionesWeb

**Para nuevo chat de GitHub Copilot**: Este documento contiene toda la información importante sobre el proyecto, lineamientos y patrones establecidos.

---

## ?? INFORMACIÓN GENERAL

### Stack Tecnológico
- **.NET 8** - ASP.NET Core MVC (NO es Blazor ni Razor Pages puro)
- **Entity Framework Core 8** - Code First
- **SQL Server** - LocalDB (dev) / Azure SQL (prod)
- **AdminLTE 3.2** + **Bootstrap 4.6** - UI Framework
- **jQuery 3.x** - JavaScript principal
- **Font Awesome 6.4.0** - Iconos
- **Serilog** - Logging a archivos JSON

### Estructura de Carpetas
```
src/
??? CotizacionesWeb.Domain/          # Entidades, enums
??? CotizacionesWeb.Application/     # Interfaces, DTOs, Requests
??? CotizacionesWeb.Infrastructure/  # EF Core, servicios
??? CotizacionesWeb.UI/              # MVC: Controllers, Views, wwwroot
```

### Reglas de Dependencia (Estrictas)
```
UI ? Application
Infrastructure ? Application + Domain
Application ? Domain
Domain ? NADA (independiente)
```

---

## ??? BASE DE DATOS

### Dos Contextos Separados
1. **DbContextCotizaciones**: Datos propios (Usuarios, Roles, Permisos, Cotizaciones)
2. **DbContextErp**: Datos del ERP externo (SOLO LECTURA - NO transacciones distribuidas)

### Auditoría Automática
```csharp
public class BaseEntity {
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
```

**AuditInterceptor** (Infrastructure/Data/Interceptors/):
- Interceptor de EF Core que llena automáticamente los campos de auditoría
- **CreatedBy / ModifiedBy**: Email del usuario autenticado desde HttpContext
- Si no hay usuario autenticado: usa "system"
- Configurado en Program.cs al registrar DbContext

### Contraseñas
- **BCrypt** con 11 rounds (clase PasswordHasher en Infrastructure/Security)
- **Métodos**: Hash(password) y Verify(password, hash)
- **Endpoint temporal** (solo desarrollo): `/Account/GenerarHash?password=xxx`
- **?? NO hay bypass**: Todas las contraseñas deben estar hasheadas

### Claims del Usuario
Configurados en AccountController al hacer login:
- **NameIdentifier**: ID del usuario (int) de la tabla Usuarios
- **Name**: Nombre completo del usuario
- **Email**: Email del usuario
- **Role**: Roles asignados (múltiples claims)

---

## ?? ENTIDADES PRINCIPALES

### Rol
```csharp
public class Rol : BaseEntity
{
    public string Nombre { get; set; }        // varchar(100), UNIQUE INDEX
    public string Descripcion { get; set; }   // varchar(500)
    public bool Activo { get; set; }          // default: true
    
    public ICollection<UsuarioRol> UsuarioRoles { get; set; }
    public ICollection<PermisoRol> PermisosRoles { get; set; }
}
```

### Permiso
```csharp
public class Permiso : BaseEntity
{
    public string Codigo { get; set; }        // varchar(30), UNIQUE INDEX ??
    public string Categoria { get; set; }     // varchar(50), NOT NULL
    public string Descripcion { get; set; }   // varchar(200), NOT NULL
    
    public ICollection<PermisoRol> PermisosRoles { get; set; }
}
```

**?? IMPORTANTE**: El índice único está en `Codigo`, NO en `Descripcion`.

### Usuario
```csharp
public class Usuario : BaseEntity
{
    public string Nombre { get; set; }
    public string Email { get; set; }         // UNIQUE INDEX
    public string PasswordHash { get; set; }  // BCrypt hash
    public bool Activo { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public int IntentosFallidos { get; set; } // Max: 5
    
    public ICollection<UsuarioRol> UsuarioRoles { get; set; }
}
```

---

## ?? DISEÑO Y ESTILOS

### Paleta de Colores
```css
--color-primary: #1e3a8a;          /* Azul oscuro */
--color-secondary: #3b82f6;        /* Azul claro */
--color-primary-light: #dbeafe;    /* Hover */
--color-white: #ffffff;
--color-dark: #1f2937;
```

### Archivos CSS Globales (cargados en _Layout.cshtml)
1. `~/css/variables.css` - Variables de colores
2. `~/css/modals.css` - Estilos de modales (?? CRÍTICO - NO MODIFICAR)
3. `~/css/components.css` - Componentes reutilizables
4. `~/css/site.css` - Estilos generales

### Archivos CSS por Módulo
- `~/css/[modulo]/index.css` - Estilos para listados
- `~/css/[modulo]/create.css` - Estilos para formularios y modales

### Archivos JS por Módulo
- `~/js/[modulo]/index.js` - Funcionalidad del listado
- `~/js/site.js` - Funciones globales (showNotification)
- `~/js/modals.js` - Comportamiento de modales

---

## ?? MODALES (?? MUY IMPORTANTE)

### Reglas Críticas
```css
/* ? CORRECTO */
.modal-dialog {
    margin-top: 10vh;
    margin-bottom: 10vh;
}

/* ? NUNCA HACER ESTO */
.modal {
    display: flex !important;  /* Rompe Bootstrap */
}
```

**Por qué**: Bootstrap controla el `display` del `.modal` con JavaScript. Si lo sobreescribes con `!important`, los modales no se abren/cierran correctamente.

### Cargar Modal con AJAX
```javascript
$('.btn-edit').on('click', function() {
    const id = $(this).data('id');
    
    $('#modalContent').html('<div class="text-center p-5"><div class="spinner-border"></div></div>');
    $('#modal').modal('show');
    
    $.get('/Controller/Edit/' + id, function(data) {
        $('#modalContent').html(data);
    });
});
```

### ?? CSS para Modales Cargados con AJAX
Si un modal se carga dinámicamente y usa estilos de `create.css`, **DEBES cargar ese CSS en el Index**:
```razor
@section Scripts {
    <link rel="stylesheet" href="~/css/roles/index.css">
    <link rel="stylesheet" href="~/css/roles/create.css"> <!-- ?? NECESARIO -->
    <script src="~/js/roles/index.js"></script>
}
```

---

## ?? SISTEMA DE PERMISOS

### Estructura
- Usuario tiene N Roles
- Cada Rol tiene N Permisos
- Permisos identificados por `Codigo` (ej: "USR_CREATE")
- Permisos agrupados por `Categoria` (ej: "Usuarios")

### Códigos de Permisos Actuales (28 total)
```
Usuarios:      USR_VIEW, USR_CREATE, USR_EDIT, USR_DELETE, USR_ROLES, USR_RESET_PWD
Roles:         ROL_VIEW, ROL_CREATE, ROL_EDIT, ROL_DELETE, ROL_PERMISOS
Cotizaciones:  COT_VIEW, COT_CREATE, COT_EDIT, COT_DELETE, COT_APPROVE, COT_REJECT, COT_EXPORT
Clientes:      CLI_VIEW, CLI_CREATE, CLI_EDIT, CLI_DELETE
Reportes:      RPT_VIEW, RPT_EXPORT, RPT_DASHBOARD
Configuración: CFG_VIEW, CFG_EDIT, CFG_LOGS
```

### Uso en Controllers
```csharp
[Authorize(Roles = "Admin,Administrador")]
public class UsuariosController : Controller
{
    [RequierePermiso("USR_CREATE")]
    public async Task<IActionResult> Create() { }
    
    [RequierePermiso("USR_EDIT")]
    public async Task<IActionResult> Edit(int id) { }
}
```

### Uso en Vistas (Tag Helper)
```html
<!-- Deshabilitar botón -->
<button requiere-permiso="USR_DELETE" class="btn btn-danger">
    Eliminar
</button>

<!-- Ocultar elemento completamente -->
<div requiere-permiso="USR_EDIT" tipo-restriccion="ocultar">
    <button>Editar</button>
</div>
```

### Uso en Layout (Menús Dinámicos)
```razor
@inject IPermisoChecker PermisoChecker

@{
    var tienePermiso = await PermisoChecker.TienePermisoAsync("USR_VIEW");
    var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
}

@if (tienePermiso || esAdmin)
{
    <li class="nav-item">
        <a asp-controller="Usuarios">Usuarios</a>
    </li>
}
```

**Regla**: Si el usuario no tiene ningún permiso en un menú padre, el menú completo se oculta.

### Permisos Agrupados por Categoría (UI)
Los permisos se muestran visualmente agrupados:
- **Header con gradiente** (clickeable para seleccionar todos de la categoría)
- Cada permiso muestra: **Código** (en negrita) + **Descripción** (en gris)
- Ordenados por: Categoría ? Código

---

## ?? JAVASCRIPT GLOBAL

### Función de Notificaciones
```javascript
// Definida en ~/js/site.js
window.showNotification = function(type, message) {
    // type: 'success' o 'error'
    // Muestra alerta Bootstrap con auto-close en 5 segundos
};

// Uso en vistas:
showNotification('success', 'Operación exitosa');
```

### Filtros de Tablas
```javascript
function filterTable() {
    const filter = $('#filterName').val().toLowerCase();
    
    $('#table tbody tr').each(function() {
        const row = $(this);
        const nombre = row.attr('data-nombre') || ''; // ?? USAR attr(), NO data()
        
        if (nombre.includes(filter)) {
            row.show();
        } else {
            row.hide();
        }
    });
}

$('#filterName').on('keyup change', filterTable);
```

**?? IMPORTANTE**: Usar `row.attr('data-campo')` NO `row.data('campo')` (jQuery cachea data() y causa problemas).

### Animación de Números
```javascript
function animateValue(element, start, end, duration) {
    let startTimestamp = null;
    const step = (timestamp) => {
        if (!startTimestamp) startTimestamp = timestamp;
        const progress = Math.min((timestamp - startTimestamp) / duration, 1);
        const value = Math.floor(progress * (end - start) + start);
        element.textContent = value;
        if (progress < 1) {
            window.requestAnimationFrame(step);
        }
    };
    window.requestAnimationFrame(step);
}
```

---

## ?? COMPONENTES REUTILIZABLES

### Switch Toggle
```html
<div class="custom-switch-container">
    <label class="switch">
        <input type="checkbox" asp-for="Activo" checked>
        <span class="slider"></span>
    </label>
    <label class="switch-label">
        <i class="fas fa-toggle-on"></i> Activo
    </label>
</div>
```
Dimensiones: 60px × 34px, fondo gris `#f8f9fa`, borde `2px solid #e0e0e0`

### Cards con Hover
```css
.card-custom {
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    transition: transform 0.2s, box-shadow 0.2s;
}

.card-custom:hover {
    transform: translateY(-4px);
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
}
```

### Tablas Estándar
```html
<table class="table table-hover table-custom">
    <thead>
        <tr>
            <th>Columna</th>
        </tr>
    </thead>
    <tbody>
        <!-- data-nombre y data-activo para filtros -->
        <tr data-nombre="valor" data-activo="true">
            <td>Dato</td>
        </tr>
    </tbody>
</table>
```

---

## ?? ERRORES COMUNES A EVITAR

### 1. ? NO modificar `.modal` en CSS
```css
/* ? MAL - Rompe Bootstrap */
.modal {
    display: flex !important;
}

/* ? BIEN - Solo ajustar márgenes */
.modal-dialog {
    margin-top: 10vh;
}
```

### 2. ? NO usar row.data() para filtros
```javascript
/* ? MAL - Cachea valores */
const nombre = row.data('nombre');

/* ? BIEN - Lee del DOM */
const nombre = row.attr('data-nombre');
```

### 3. ? NO duplicar función showNotification
```javascript
/* ? MAL - Crear en cada módulo */
function showNotification() { ... }

/* ? BIEN - Usar la global */
window.showNotification('success', 'mensaje');
```

### 4. ? NO duplicar estilos de modales
```css
/* ? MAL - En cada módulo */
.modal-header { ... }

/* ? BIEN - Usar modals.css global */
.modal-header-custom { ... }
```

### 5. ? NO olvidar cargar CSS de modales en Index
```razor
<!-- ? MAL - Modal sin estilos -->
@section Scripts {
    <link href="~/css/roles/index.css">
}

<!-- ? BIEN - Include create.css para modales -->
@section Scripts {
    <link href="~/css/roles/index.css">
    <link href="~/css/roles/create.css">
}
```

---

## ? CHECKLIST PARA NUEVO MÓDULO

### Domain Layer
- [ ] Crear entidad en `Domain/Entities/NombreEntidad.cs`
- [ ] Heredar de `BaseEntity`
- [ ] Definir propiedades y relaciones

### Infrastructure Layer
- [ ] Crear `Data/Configurations/NombreEntidadConfiguration.cs`
- [ ] Implementar `IEntityTypeConfiguration<T>`
- [ ] Agregar `DbSet<T>` en DbContext
- [ ] Crear migración con EF Core
- [ ] Aplicar migración
- [ ] Crear `Services/NombreEntidadService.cs`

### Application Layer
- [ ] Crear `NombreModulo/INombreModuloService.cs`
- [ ] Definir DTOs con `record`
- [ ] Definir Requests con `record`

### UI Layer - Backend
- [ ] Crear `Models/NombreModuloViewModels.cs`
- [ ] Agregar validaciones: `[Required]`, `[StringLength]`, etc.
- [ ] Crear `Controllers/NombreModuloController.cs`
- [ ] Agregar `[Authorize(Roles = "Admin,Administrador")]`
- [ ] Agregar `[RequierePermiso("CODIGO")]` en acciones

### UI Layer - Frontend
- [ ] Crear `Views/NombreModulo/Index.cshtml`
- [ ] Crear `Views/NombreModulo/Create.cshtml`
- [ ] Crear `Views/NombreModulo/_EditModal.cshtml`
- [ ] Crear `wwwroot/css/nombremodulo/index.css`
- [ ] Crear `wwwroot/css/nombremodulo/create.css`
- [ ] Crear `wwwroot/js/nombremodulo/index.js`
- [ ] Agregar link en `_Layout.cshtml` (con verificación de permisos)

### Validación Final
- [ ] `dotnet build` exitoso
- [ ] Probar CRUD completo
- [ ] Verificar filtros funcionando
- [ ] Verificar modales (abrir/cerrar correctamente)
- [ ] Verificar permisos (botones deshabilitados)
- [ ] Verificar menú (oculto si sin permiso)
- [ ] Responsive design

---

## ?? MIGRACIONES EF CORE

### Crear Migración
```bash
dotnet ef migrations add NombreMigracion \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output-dir Data/Migrations
```

### Aplicar Migración
```bash
dotnet ef database update \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

### Revertir Última Migración
```bash
dotnet ef migrations remove \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

---

## ?? MÓDULOS IMPLEMENTADOS

### ? Dashboard/Home
- Estadísticas generales
- Actividad reciente
- Accesos rápidos según rol

### ? Usuarios
- CRUD completo
- Gestión de roles (modal con checkboxes)
- Resetear contraseña (modal con validación)
- Ver roles asignados (modal de solo lectura)
- Filtros: nombre, email, estado

### ? Roles
- CRUD completo
- Gestión de permisos agrupados por categoría
- Header de categoría clickeable (selecciona todos)
- Filtros: nombre, estado
- Validación: no eliminar rol con usuarios asignados

---

## ??? SERVICIOS REGISTRADOS (Program.cs)

```csharp
// Seguridad
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Usuarios, Roles y Permisos
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IPermisoService, PermisoService>();

// UI Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPermisoChecker, PermisoChecker>();

// DbContext con AuditInterceptor
builder.Services.AddDbContext<DbContextCotizaciones>((serviceProvider, options) =>
{
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    
    var auditInterceptor = new AuditInterceptor(() =>
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value ?? "system";
        }
        return "system";
    });
    
    options.UseSqlServer(builder.Configuration.GetConnectionString("CotizacionesDb"))
           .AddInterceptors(auditInterceptor);
});
```

---

## ?? PASOS INICIALES DEL SISTEMA

### 1. Generar Hash de Contraseña
Visita (con la app corriendo):
```
https://localhost:5001/Account/GenerarHash?password=Admin123!
```
Copia el hash generado (60+ caracteres).

### 2. Ejecutar Scripts SQL (en orden)
**A. Poblar permisos:**
```sql
-- Ejecutar: src/CotizacionesWeb.Infrastructure/Data/Scripts/SeedPermisos.sql
```

**B. Crear usuario admin:**
```sql
-- 1. Abrir: src/CotizacionesWeb.Infrastructure/Data/Scripts/CrearUsuarioAdmin.sql
-- 2. Reemplazar: REEMPLAZAR_CON_HASH_GENERADO con el hash del paso 1
-- 3. Ejecutar el script
```

### 3. Login y Verificar
- Login con: `admin@cotizaciones.com` / `Admin123!`
- Ir a: Roles > Administrador > Ver permisos
- Verificar: Debe tener los 28 permisos asignados

### 4. Probar Sistema de Permisos
1. Crear rol de prueba con permisos limitados
2. Crear usuario de prueba con ese rol
3. Logout y login con usuario de prueba
4. Verificar botones deshabilitados y menús ocultos

---

## ?? ARCHIVOS IMPORTANTES

### Configuración
- `Program.cs` - Registro de servicios + AuditInterceptor
- `appsettings.json` - Configuración general
- `_ViewImports.cshtml` - Usings y Tag Helpers globales

### CSS Global
- `variables.css` - Variables de colores
- `components.css` - Componentes reutilizables
- `modals.css` - Modales (?? NO MODIFICAR)
- `site.css` - Estilos generales

### JavaScript Global
- `site.js` - showNotification() y funciones globales
- `modals.js` - Comportamiento de modales

---

## ?? DECISIONES DE DISEÑO CLAVE

1. **Permisos agrupados por categoría** - Facilita asignación masiva
2. **Header clickeable** - Selecciona/deselecciona toda la categoría
3. **Tag Helper para permisos** - Deshabilita elementos sin JavaScript
4. **Menús dinámicos** - Se ocultan si no hay permisos
5. **Admin bypass** - Roles Admin/Administrador tienen acceso total
6. **Índice único en Permiso.Codigo** - No en Descripcion
7. **Modales centrados con CSS** - Sin JavaScript que modifique márgenes
8. **AuditInterceptor automático** - No requiere código en servicios

---

## ?? INSTRUCCIONES PARA NUEVO CHAT

1. Lee este documento completo
2. Familiarízate con los patrones establecidos
3. Respeta la arquitectura por capas
4. Usa los componentes y estilos existentes
5. NO reinventes soluciones que ya existen
6. Consulta los archivos de ejemplo antes de crear nuevos
7. Siempre verifica con `dotnet build` antes de finalizar
8. Los usuarios deben hacer logout/login después de cambios en claims

---

## ?? INFORMACIÓN ADICIONAL

### Repositorio
- **GitHub**: https://github.com/LunaMar24/CotizacionesWeb
- **Branch**: `Marcela/TrabajoPrueba`

### Autenticación
- **Basada en Cookies** (NO JWT)
- **Timeout**: 60 minutos con sliding expiration
- **Login**: `/Account/Login`
- **Access Denied**: `/Account/Denied`

### Roles de Sistema
- **Admin / Administrador**: Acceso total (bypass de permisos)
- **Roles personalizados**: Verifican permisos específicos

---

**Versión**: 1.1  
**Última actualización**: 10 de marzo de 2026, 7:50 PM  
**Autor**: Marcela Jiménez (con GitHub Copilot)
