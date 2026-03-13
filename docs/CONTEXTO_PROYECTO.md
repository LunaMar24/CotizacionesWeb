# CONTEXTO DEL PROYECTO - CotizacionesWeb

**Para nuevo chat de GitHub Copilot**: Este documento contiene toda la informacion importante sobre el proyecto, lineamientos y patrones establecidos.

---

## INFORMACION GENERAL

### Stack Tecnologico
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

## BASE DE DATOS

### Dos Contextos Separados
1. **DbContextCotizaciones**: Datos propios (Usuarios, Roles, Permisos, Cotizaciones)
2. **DbContextErp**: Datos del ERP externo (SOLO LECTURA - NO transacciones distribuidas)

### Auditoria Automatica
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
- Interceptor de EF Core que llena automaticamente los campos de auditoria
- **CreatedBy / ModifiedBy**: Email del usuario autenticado desde HttpContext
- Si no hay usuario autenticado: usa "system"
- Configurado en Program.cs al registrar DbContext

### Contrasenas
- **BCrypt** con 11 rounds (clase PasswordHasher en Infrastructure/Security)
- **Metodos**: Hash(password) y Verify(password, hash)
- **Endpoint temporal** (solo desarrollo): `/Account/GenerarHash?password=xxx`
- **NO hay bypass**: Todas las contrasenas deben estar hasheadas

### Claims del Usuario
Configurados en AccountController al hacer login:
- **NameIdentifier**: ID del usuario (int) de la tabla Usuarios
- **Name**: Nombre completo del usuario
- **Email**: Email del usuario
- **Role**: Roles asignados (multiples claims)

---

## ENTIDADES PRINCIPALES

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
    public string Codigo { get; set; }        // varchar(30), UNIQUE INDEX
    public string Categoria { get; set; }     // varchar(50), NOT NULL
    public string Descripcion { get; set; }   // varchar(200), NOT NULL
    
    public ICollection<PermisoRol> PermisosRoles { get; set; }
}
```

**IMPORTANTE**: El indice unico esta en `Codigo`, NO en `Descripcion`.

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

## DISENO Y ESTILOS

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
2. `~/css/modals.css` - Estilos de modales (CRITICO - NO MODIFICAR)
3. `~/css/components.css` - Componentes reutilizables
4. `~/css/site.css` - Estilos generales

### Archivos CSS por Modulo
- `~/css/[modulo]/index.css` - Estilos para listados
- `~/css/[modulo]/create.css` - Estilos para formularios y modales

### Archivos JS por Modulo
- `~/js/[modulo]/index.js` - Funcionalidad del listado
- `~/js/site.js` - Funciones globales (showNotification)
- `~/js/modals.js` - Comportamiento de modales

---

## MODALES (MUY IMPORTANTE)

### Reglas Criticas
```css
/* CORRECTO */
.modal-dialog {
    margin-top: 10vh;
    margin-bottom: 10vh;
}

/* NUNCA HACER ESTO */
.modal {
    display: flex !important;  /* Rompe Bootstrap */
}
```

**Por que**: Bootstrap controla el `display` del `.modal` con JavaScript. Si lo sobreescribes con `!important`, los modales no se abren/cierran correctamente.

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

### Modal de Confirmacion Generico
Usar la funcion `mostrarModalConfirmacion()` en lugar de `confirm()` o `alert()`:

```javascript
mostrarModalConfirmacion(
    'Titulo',
    'Mensaje HTML permitido',
    'tipo',  // info, warning, danger, success
    function() {
        // Callback si confirma
    }
);
```

**Ventajas**:
- Modal Bootstrap nativo (mejor UX)
- Colores dinamicos segun tipo
- Soporta HTML en el mensaje
- Sin problemas de encoding

### CSS para Modales Cargados con AJAX
Si un modal se carga dinamicamente y usa estilos de `create.css`, **DEBES cargar ese CSS en el Index**:
```razor
@section Scripts {
    <link rel="stylesheet" href="~/css/roles/index.css">
    <link rel="stylesheet" href="~/css/roles/create.css"> <!-- NECESARIO -->
    <script src="~/js/roles/index.js"></script>
}
```

---

## SISTEMA DE PERMISOS

### Estructura
- Usuario tiene N Roles
- Cada Rol tiene N Permisos
- Permisos identificados por `Codigo` (ej: "USR_CREATE")
- Permisos agrupados por `Categoria` (ej: "Usuarios")

### Codigos de Permisos Actuales (28 total)
```
Usuarios:      USR_VIEW, USR_CREATE, USR_EDIT, USR_DELETE, USR_ROLES, USR_RESET_PWD
Roles:         ROL_VIEW, ROL_CREATE, ROL_EDIT, ROL_DELETE, ROL_PERMISOS
Cotizaciones:  COT_VIEW, COT_CREATE, COT_EDIT, COT_DELETE, COT_APPROVE, COT_REJECT, COT_EXPORT
Clientes:      CLI_VIEW, CLI_CREATE, CLI_EDIT, CLI_DELETE
Reportes:      RPT_VIEW, RPT_EXPORT, RPT_DASHBOARD
Configuracion: CFG_VIEW, CFG_EDIT, CFG_LOGS
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
<!-- Deshabilitar boton -->
<button requiere-permiso="USR_DELETE" class="btn btn-danger">
    Eliminar
</button>

<!-- Ocultar elemento completamente -->
<div requiere-permiso="USR_EDIT" tipo-restriccion="ocultar">
    <button>Editar</button>
</div>
```

### Uso en Layout (Menus Dinamicos)
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

**Regla**: Si el usuario no tiene ningun permiso en un menu padre, el menu completo se oculta.

---

## JAVASCRIPT GLOBAL

### Funcion de Notificaciones
```javascript
// Definida en ~/js/site.js
window.showNotification = function(type, message) {
    // type: 'success' o 'error'
    // Muestra alerta Bootstrap con auto-close en 5 segundos
};

// Uso en vistas:
showNotification('success', 'Operacion exitosa');
```

### Filtros de Tablas
```javascript
function filterTable() {
    const filter = $('#filterName').val().toLowerCase();
    
    $('#table tbody tr').each(function() {
        const row = $(this);
        const nombre = row.attr('data-nombre') || ''; // USAR attr(), NO data()
        
        if (nombre.includes(filter)) {
            row.show();
        } else {
            row.hide();
        }
    });
}

$('#filterName').on('keyup change', filterTable);
```

**IMPORTANTE**: Usar `row.attr('data-campo')` NO `row.data('campo')` (jQuery cachea data() y causa problemas).

### Animacion de Números
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

## COMPONENTES REUTILIZABLES

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

### Card de Filtros Colapsable
Todos los modulos (Usuarios, Roles, Cotizaciones) usan cards colapsables para filtros:
```html
<div class="card card-filtros-custom mb-4">
    <div class="card-header">
        <h3 class="card-title">
            <i class="fas fa-filter"></i> Filtros de Busqueda
        </h3>
        <div class="card-tools">
            <button type="button" class="btn btn-tool" data-card-widget="collapse">
                <i class="fas fa-minus"></i>
            </button>
        </div>
    </div>
    <div class="card-body">
        <!-- Filtros aqui -->
    </div>
</div>
```

---

## ERRORES COMUNES A EVITAR

### 1. NO modificar `.modal` en CSS
```css
/* MAL - Rompe Bootstrap */
.modal {
    display: flex !important;
}

/* BIEN - Solo ajustar margenes */
.modal-dialog {
    margin-top: 10vh;
}
```

### 2. NO usar row.data() para filtros
```javascript
/* MAL - Cachea valores */
const nombre = row.data('nombre');

/* BIEN - Lee del DOM */
const nombre = row.attr('data-nombre');
```

### 3. NO usar confirm() o alert()
```javascript
/* MAL - Alerta del navegador */
if (!confirm('Seguro?')) return;

/* BIEN - Modal Bootstrap */
mostrarModalConfirmacion('Titulo', 'Mensaje', 'warning', callback);
```

### 4. NO duplicar estilos de modales
```css
/* MAL - En cada modulo */
.modal-header { ... }

/* BIEN - Usar modals.css global */
.modal-header-custom { ... }
```

### 5. NO usar tildes en JavaScript
```javascript
/* MAL - Problemas de encoding */
showNotification('error', 'Contraseña incorrecta');

/* BIEN - Sin tildes */
showNotification('error', 'Contrasena incorrecta');
```

---

## MODULOS IMPLEMENTADOS

### Dashboard/Home
- Estadisticas generales
- Actividad reciente
- Accesos rapidos segun rol

### Usuarios
- CRUD completo
- Gestion de roles (modal con checkboxes)
- Resetear contrasena (modal con validacion)
- Ver roles asignados (modal de solo lectura)
- Filtros colapsables: nombre, email, estado
- Modal de confirmacion para eliminar

### Roles
- CRUD completo
- Gestion de permisos agrupados por categoria
- Header de categoria clickeable (selecciona todos)
- Filtros colapsables: nombre, estado
- Validacion: no eliminar rol con usuarios asignados

### Cotizaciones
- Listado con filtros colapsables
- Filtros: busqueda, fechas, estados (dropdown multiple)
- Modal de historial con timeline
- Modal de versiones
- Copiar version (con modal de confirmacion)
- Duplicar cotizacion (con modal de confirmacion)
- Botones de accion coloreados (amarillo, azul, morado, verde, rojo)

---

## SERVICIOS REGISTRADOS (Program.cs)

```csharp
// Seguridad
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Usuarios, Roles y Permisos
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IPermisoService, PermisoService>();

// Cotizaciones
builder.Services.AddScoped<ICotizacionService, CotizacionService>();

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

## MIGRACIONES EF CORE

### Crear Migracion
```bash
dotnet ef migrations add NombreMigracion \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output-dir Data/Migrations
```

### Aplicar Migracion
```bash
dotnet ef database update \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

### Revertir Ultima Migracion
```bash
dotnet ef migrations remove \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

---

## ARCHIVOS IMPORTANTES

### Configuracion
- `Program.cs` - Registro de servicios + AuditInterceptor
- `appsettings.json` - Configuracion general
- `_ViewImports.cshtml` - Usings y Tag Helpers globales

### CSS Global
- `variables.css` - Variables de colores
- `components.css` - Componentes reutilizables
- `modals.css` - Modales (NO MODIFICAR)
- `site.css` - Estilos generales

### JavaScript Global
- `site.js` - showNotification() y funciones globales
- `modals.js` - Comportamiento de modales

---

## DECISIONES DE DISENO CLAVE

1. **Permisos agrupados por categoria** - Facilita asignacion masiva
2. **Header clickeable** - Selecciona/deselecciona toda la categoria
3. **Tag Helper para permisos** - Deshabilita elementos sin JavaScript
4. **Menus dinamicos** - Se ocultan si no hay permisos
5. **Admin bypass** - Roles Admin/Administrador tienen acceso total
6. **Indice unico en Permiso.Codigo** - No en Descripcion
7. **Modales centrados con CSS** - Sin JavaScript que modifique margenes
8. **AuditInterceptor automatico** - No requiere codigo en servicios
9. **Modales de confirmacion** - En lugar de confirm() del navegador
10. **Sin tildes en JavaScript** - Evita problemas de encoding
11. **Filtros colapsables** - Mas espacio para datos en pantalla
12. **IDs no duplicados** - BaseEntity.Id se usa como PK en todas las entidades

---

## INFORMACION ADICIONAL

### Repositorio
- **GitHub**: https://github.com/LunaMar24/CotizacionesWeb
- **Branch**: `Marcela/TrabajoPrueba`

### Autenticacion
- **Basada en Cookies** (NO JWT)
- **Timeout**: 60 minutos con sliding expiration
- **Login**: `/Account/Login`
- **Access Denied**: `/Account/Denied`

### Roles de Sistema
- **Admin / Administrador**: Acceso total (bypass de permisos)
- **Roles personalizados**: Verifican permisos especificos

---

**Version**: 2.0  
**Ultima actualizacion**: 12 de marzo de 2026, 11:15 PM  
**Autor**: Marcela Jimenez (con GitHub Copilot)
