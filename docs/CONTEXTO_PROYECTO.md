# CONTEXTO DEL PROYECTO - CotizacionesWeb

**Para nuevo chat de GitHub Copilot**: Este documento contiene toda la información importante sobre el proyecto, lineamientos, reglas de negocio y patrones establecidos.

### 🎨 **5. Mejoras de UX y Estilos en Vista de Detalle**

#### **Cards Colapsables Implementadas:**
Se implementó funcionalidad completa de colapso para optimizar el espacio:

```javascript
// JavaScript para cards colapsables
$(document).ready(function() {
    // Inicializar funcionalidad de colapso AdminLTE
    $('[data-card-widget="collapse"]').CardWidget();
    
    // Click en header completo (no solo botón)
    $('.card-header[data-card-widget="collapse"]').on('click', function(e) {
        if (!$(e.target).closest('.btn').length) {
            $(this).find('.btn[data-card-widget="collapse"]').click();
        }
    });
    
    // Cambiar iconos dinámicamente
    $('[data-card-widget="collapse"]').on('expanded.lte.cardwidget', function() {
        $(this).closest('.card').find('.btn i')
            .removeClass('fa-plus').addClass('fa-minus');
    });
});
```

#### **Etiquetas Alineadas Correctamente:**
Se corrigió la alineación de etiquetas para mejor presentación visual:

```css
.info-label {
    display: block;
    min-width: 180px;
    width: 180px;
    color: #495057;
    text-align: right;        /* ← Alineación a la derecha */
    margin-right: 15px;
    flex-shrink: 0;
    font-weight: 600;
}

.info-item .d-flex {
    align-items: flex-start;  /* ← Alineación superior para badges */
    min-height: 30px;
}
```

#### **Tabla Scrolleable con Header Fijo:**
```css
.table-responsive {
    max-height: 500px;
    overflow-y: auto;
    border: 1px solid #dee2e6;
    border-radius: 0.375rem;
}

.sticky-top {
    position: sticky;
    top: 0;
    z-index: 10;
    background-color: #f8f9fa !important;
}

/* Scrollbar personalizado */
.table-responsive::-webkit-scrollbar {
    width: 8px;
    height: 8px;
}

.table-responsive::-webkit-scrollbar-thumb {
    background: #c1c1c1;
    border-radius: 4px;
}
```

### 🚀 **6. Integración con Sistema Existente**

#### **Navegación Mejorada:**
- ✅ **Desde Index**: Botón "Ver Detalle" (amarillo) navega a vista completa
- ✅ **Desde Versiones**: Modal incluye botón para ver detalle de versión específica
- ✅ **Breadcrumb**: Botón "Volver al Listado" desde la vista de detalle

#### **JavaScript Actualizado:**
```javascript
// Botón detalle desde listado principal
$('.btn-detalle').on('click', function () {
    const cotizacionId = $(this).attr('data-id');
    window.location.href = '/Cotizaciones/Detalle/' + cotizacionId;
});

// Botón detalle desde modal de versiones
$('.btn-detalle-version').on('click', function () {
    const versionId = $(this).attr('data-version-id');
    const cotizacionId = $(this).attr('data-cotizacion-id');
    window.location.href = '/Cotizaciones/Detalle/' + cotizacionId + '?versionId=' + versionId;
});
```

### 🔐 **7. Permisos y Seguridad**

#### **Nuevo Permiso Agregado:**
```csharp
// Controlador con permiso específico
[HttpGet("Cotizaciones/Detalle/{cotizacionId}")]
[RequierePermiso("COT_VIEW_DETAIL")]  // ← Permiso específico
public async Task<IActionResult> Detalle(string cotizacionId, int? versionId = null)
```

#### **Tag Helper en Vistas:**
```html
<!-- Botón habilitado solo con permiso -->
<button requiere-permiso="COT_VIEW_DETAIL" class="btn btn-warning">
    <i class="fas fa-edit"></i>
</button>
```

---

## 📊 **MÓDULO COTIZACIONES ACTUALIZADO**

### 🆕 **Funcionalidades Agregadas**

#### **Vista de Detalle Completa:**
- **Propósito**: Visualización de solo lectura de cotizaciones
- **Alcance**: Información completa (cliente, productos, financiera, historial)
- **Estados**: Compatible con todas las versiones y estados
- **Responsive**: Optimizada para escritorio, tablet y móvil

#### **Formateo Dinámico de Monedas:**
- **Problema resuelto**: Símbolos de moneda fijos
- **Solución**: Helper inteligente con diccionario de símbolos
- **Impacto**: Todas las vistas (Index, Detalle, Versiones, Modales)

#### **Configuración de Zona Horaria:**
- **Problema resuelto**: Fechas en UTC incorrectas
- **Solución**: Configuración para Costa Rica (UTC-6)
- **Impacto**: Auditoría, creación y modificación de registros

### 📱 **Archivos CSS Específicos**

#### **`detalle.css` (Nuevo):**
```css
/* Estilos específicos para vista de detalle */
~/css/cotizaciones/detalle.css

Incluye:
- Estilos para info-items alineados
- Cards colapsables
- Tabla scrolleable con header fijo
- Badges personalizados por estado
- Optimización para impresión
```

#### **Helper Global:**
```csharp
// Disponible en todas las vistas via _ViewImports.cshtml
@using CotizacionesWeb.UI.Helpers

// Uso directo sin prefijo
@FormatHelper.FormatCurrency(valor, moneda)
@FormatHelper.FormatVersion(version)
```

---

## ⚡ **MEJORAS DE PERFORMANCE Y CÓDIGO**

### 🔧 **Optimizaciones Implementadas**

#### **Consultas de Base de Datos:**
- ✅ **Join Eficiente**: Versión actual obtenida con JOIN directo
- ✅ **Carga Selectiva**: Solo datos necesarios para la vista
- ✅ **Fallback Inteligente**: Manejo de versiones no encontradas

#### **Renderizado de Vistas:**
- ✅ **CSS Externo**: Estilos separados para mejor performance
- ✅ **Loading Diferido**: Cards colapsables cargan bajo demanda
- ✅ **Scrolling Virtual**: Tabla optimizada para muchas líneas

#### **Formateo Optimizado:**
```csharp
// Cultura invariante para máxima performance
value.ToString("N2", CultureInfo.InvariantCulture)

// Diccionario estático (no consulta BD)
private static readonly Dictionary<string, string> CurrencySymbols
```

### 🐛 **Problemas Resueltos**

#### **1. Separador Decimal Incorrecto:**
- **Problema**: `v2,0` y `₡1.234,56`
- **Causa**: Cultura es-CR usa coma decimal
- **Solución**: `CultureInfo.InvariantCulture` en helper
- **Estado**: ✅ **Resuelto**

#### **2. Símbolo de Moneda Fijo:**
- **Problema**: Solo símbolo ₡ para todas las monedas
- **Causa**: Símbolo hardcodeado en vistas
- **Solución**: Helper dinámico con diccionario
- **Estado**: ✅ **Resuelto**

#### **3. Fechas en UTC:**
- **Problema**: Diferencia horaria de 6 horas
- **Causa**: `DateTime.UtcNow` en AuditInterceptor
- **Solución**: `DateTime.Now` + configuración zona horaria
- **Estado**: ✅ **Resuelto**

#### **4. Etiquetas Desalineadas:**
- **Problema**: Campos mal alineados visualmente
- **Causa**: CSS sin anchura fija
- **Solución**: `.info-label` con anchura y alineación
- **Estado**: ✅ **Resuelto**

---

## 🎯 **PRÓXIMOS PASOS SUGERIDOS**

### 🔄 **Funcionalidades Futuras**
1. **📝 Edición en Modal**: Editar cotizaciones desde la vista de detalle
2. **📧 Envío por Email**: Exportar PDF y enviar al cliente
3. **📊 Dashboard**: Métricas de cotizaciones por estado/moneda
4. **🔍 Búsqueda Avanzada**: Filtros por productos, montos, fechas
5. **📱 Optimización Móvil**: Mejoras específicas para dispositivos móviles

### ⚙️ **Mejoras Técnicas**
1. **🏃 Performance**: Lazy loading para líneas de detalle
2. **🔒 Seguridad**: Permisos más granulares por campo
3. **🧪 Testing**: Tests unitarios para FormatHelper
4. **📈 Monitoreo**: Logs de performance para vistas complejas
5. **🌐 Internacionalización**: Soporte para más idiomas/culturas

---

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
- **NO hay bypass**: Todas las contraseñas deben estar hasheadas

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
    public string Codigo { get; set; }        // varchar(30), UNIQUE INDEX
    public string Categoria { get; set; }     // varchar(50), NOT NULL
    public string Descripcion { get; set; }   // varchar(200), NOT NULL
    
    public ICollection<PermisoRol> PermisosRoles { get; set; }
}
```

**IMPORTANTE**: El índice único está en `Codigo`, NO en `Descripcion`.

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

## ?? MÓDULO COTIZACIONES - REGLAS DE NEGOCIO

### ?? Estados Válidos de Cotización

| Código | Estado | Descripción |
|--------|--------|-------------|
| **B** | Borrador | Cotización en construcción, puede modificarse |
| **P** | PendienteAprobacion | Enviada para revisión y aprobación interna |
| **A** | Aprobada | Aprobada internamente, lista para envío al cliente |
| **E** | Enviada | Enviada al cliente, esperando respuesta |
| **T** | Aceptada | Cliente aceptó la cotización |
| **R** | Rechazada | Cliente rechazó la cotización |
| **X** | Archivada | Estado terminal administrativo |

### ?? Flujos Válidos

#### Flujo Principal - Cotización Aceptada
```
Borrador ? PendienteAprobacion ? Aprobada ? Enviada ? Aceptada
```

#### Flujo Principal - Cotización Rechazada
```
Borrador ? PendienteAprobacion ? Aprobada ? Enviada ? Rechazada
```

#### Flujos Adicionales Permitidos
```
Borrador ? Archivada
Aceptada ? Archivada
Rechazada ? Archivada
PendienteAprobacion ? Borrador (requiere nota obligatoria)
```

### ?? Reglas de Transición por Estado

| Estado | Puede Pasar A | Observaciones |
|--------|---------------|---------------|
| **B (Borrador)** | P, X | Flujo normal o archivo directo |
| **P (PendienteAprobacion)** | A, B | Si vuelve a B: nota obligatoria |
| **A (Aprobada)** | E | Solo puede enviarse al cliente |
| **E (Enviada)** | T, R | Cliente acepta o rechaza |
| **T (Aceptada)** | X | Solo puede archivarse |
| **R (Rechazada)** | X | Solo puede archivarse |
| **X (Archivada)** | - | Estado terminal |

### ?? Acciones por Estado

| Acción | Estados Permitidos | Observaciones |
|--------|--------------------|---------------|
| Acción | Estados Permitidos | Observaciones |
|--------|--------------------|---------------|
| **Copiar** | **A, R** solamente | Genera nueva versión en estado B (NO disponible en B,P,E,T,X) |
| **Duplicar** | Cualquiera | Nueva cotización en Borrador |
| **Enviar a aprobación** | **B** solamente | B ? P |
| **Aprobar** | **P** solamente | P ? A |
| **Devolver a borrador** | **P** solamente | P ? B, requiere nota |
| **Enviar al cliente** | **A** solamente | A ? E |
| **Marcar como aceptada** | **E** solamente | E ? T |
| **Marcar como rechazada** | **E** solamente | E ? R |
| **Archivar** | **B, T, R** | ? X, requiere confirmación |
| **?? Enviar al ERP** | **T** solamente | **REGLA ESTRICTA** |
| **Duplicar** | Cualquiera | Nueva cotización en Borrador |
| **Enviar a aprobación** | **B** solamente | B ? P |
| **Aprobar** | **P** solamente | P ? A |
| **Devolver a borrador** | **P** solamente | P ? B, requiere nota |
| **Enviar al cliente** | **A** solamente | A ? E |
| **Marcar como aceptada** | **E** solamente | E ? T |
| **Marcar como rechazada** | **E** solamente | E ? R |
| **Archivar** | **B, T, R** | ? X, requiere confirmación |
| **?? Enviar al ERP** | **T** solamente | **REGLA ESTRICTA** |

### ?? Regla Especial: Envío al ERP

**? REGLA ACTUAL (Correcta)**:
- **SOLO** cotizaciones en estado **T (Aceptada)** pueden enviarse al ERP
- **Justificación**: Solo las cotizaciones confirmadas por el cliente tienen valor comercial
- **Evita**: Especulación, duplicados en ERP, confusión operativa

**? REGLA ANTERIOR (Incorrecta)**:
- Estados A, E, T podían enviarse al ERP
- **Problema**: Cotizaciones especulativas en el ERP

### ??? Campos de Estado en Cotización

| Campo | Tipo | Regla |
|-------|------|-------|
| **FechaAceptacion** | `datetime null` | Solo cuando estado = T |
| **FechaRechazo** | `datetime null` | Solo cuando estado = R |
| **EnviadoERP** | `char(1)` | S/N, default: N |
| **FechaEnvioERP** | `datetime null` | Solo cuando EnviadoERP = S |

### ?? Historial de Cotizaciones

#### Eventos Funcionales Requeridos

| Evento | Código | Cuándo Registrar |
|--------|--------|------------------|
| **Creada** | "Creada" | Al crear nueva cotización |
| **VersionGenerada** | "VersionGenerada" | Al copiar/crear nueva versión |
| **EnviadaAProbacion** | "EnviadaAProbacion" | B ? P |
| **DevueltaABorrador** | "DevueltaABorrador" | P ? B |
| **Aprobada** | "Aprobada" | P ? A |
| **EnviadaCliente** | "EnviadaCliente" | A ? E |
| **AceptadaCliente** | "AceptadaCliente" | E ? T |
| **RechazadaCliente** | "RechazadaCliente" | E ? R |
| **Archivada** | "Archivada" | Cualquier ? X |
| **EnviadaERP** | "EnviadaERP" | Al marcar EnviadoERP = S |

---

## ?? SISTEMA DE VERSIONADO

### ?? Regla de Presentación Principal
**La pantalla principal debe mostrar únicamente la versión vigente de cada cotización**

### ?? Estructura de Versionado

#### Tabla Cotizacion (Puntero)
```sql
CotizacionId  | VersionActual  -- Puntero a la versión vigente
COT-0001     | 5              -- Apunta a VersionActual = 5
COT-0002     | 2              -- Apunta a VersionActual = 2  
COT-0003     | 7              -- Apunta a VersionActual = 7
```

#### Tabla CotizacionVersion (Datos)
```sql
CotizacionId | VersionActual | NumeroVersion  -- Datos de versiones
COT-0001    | 1             | 1.0           -- Versión histórica
COT-0001    | 2             | 2.0           -- Versión histórica  
COT-0001    | 5             | 3.0           -- Versión VIGENTE
COT-0002    | 1             | 1.0           -- Versión histórica
COT-0002    | 2             | 2.0           -- Versión VIGENTE
COT-0003    | 4             | 1.0           -- Versión histórica
COT-0003    | 7             | 2.0           -- Versión VIGENTE
```

### ?? Campos de Versionado

#### **NumeroVersion (Visible al Usuario)**
- ?? **Propósito**: Numeración visible para el usuario  
- ?? **Editable**: Puede ser modificado por el usuario
- ?? **Ejemplos**: 1.0, 2.0, 3.5, etc.
- ? **NO usar para**: Determinar versión vigente

#### **VersionActual (Sistema Interno)**
- ?? **Propósito**: Identificador único interno del sistema
- ?? **Controlado**: Solo el sistema lo modifica  
- ?? **Ejemplos**: 1, 2, 3, 5, 7, 12, etc. (únicos y crecientes)
- ? **USAR para**: Determinar versión vigente, uniones de tablas

### ?? JOIN Correcto para Versión Vigente
```sql
SELECT c.*, v.*
FROM Cotizacion c
INNER JOIN CotizacionVersion v 
    ON c.CotizacionId = v.CotizacionId 
    AND c.VersionActual = v.VersionActual
```

### ? Implementación en EF Core
```csharp
// ? PROBLEMÁTICO - EF Core no puede traducir
.Include(c => c.Versiones.Where(v => v.VersionActual == c.VersionActual))

// ? CORRECTO - JOIN explícito
var query = from cot in _context.Cotizaciones
            join ver in _context.CotizacionesVersiones
                on new { cot.CotizacionId, VersionId = cot.VersionActual }
                equals new { ver.CotizacionId, VersionId = ver.VersionActual }
            select new { Cotizacion = cot, Version = ver };
```

---

## ?? PROCESO DE COPIA DE VERSIONES

### ?? Reglas de Negocio para Copia

#### ?? **¿Cuándo se puede copiar?**
- ? **Aprobada (A)**: Se puede copiar sin restricciones
- ? **Rechazada (R)**: Se puede copiar para crear nueva propuesta
- ? **Enviada (E)**: NO se puede copiar (esperando respuesta del cliente)
- ? **Aceptada (T)**: NO se puede copiar (cotización en firme)
- ? **Borrador (B)**: NO tiene sentido (se pueden hacer cambios directos)
- ? **PendienteAprobacion (P)**: NO tiene sentido (se pueden hacer cambios directos)
- ? **Archivada (X)**: NO se puede copiar (estado terminal)

#### ?? **Estado Post-Copia (REGLA CRÍTICA)**
**?? IMPORTANTE**: Cuando se crea una nueva versión mediante copia, **la cotización SIEMPRE vuelve al estado Borrador (B)**, independientemente de su estado original.

**?? Justificación**:
- Una nueva versión es un **nuevo ciclo de vida**
- Debe pasar por **todas las aprobaciones** nuevamente
- **Evita**: Versiones no revisadas con estado "Aprobada"
- **Garantiza**: Control de calidad y revisión obligatoria

**?? Flujo de Estado en Copia**:
```
Estado Original ? Nueva Versión Creada ? Estado Final
A (Aprobada)    ? Copiar Versión      ? B (Borrador)
R (Rechazada)   ? Copiar Versión      ? B (Borrador)
```

#### ?? **Lógica de Numeración (NumeroVersion)**

**?? Incremento por versión mayor:**
- Versión actual: `1.0` ? Nueva versión: `2.0`
- Versión actual: `2.0` ? Nueva versión: `3.0`
- Versión actual: `5.0` ? Nueva versión: `6.0`

**?? Incremento por versión menor:**
- Versión actual: `1.1` ? Nueva versión: `1.2`
- Versión actual: `2.5` ? Nueva versión: `2.6`
- Versión actual: `3.9` ? Nueva versión: `3.10`

**?? Algoritmo de incremento:**
```csharp
decimal nuevaVersion;
if (versionActual % 1 == 0) // Es versión mayor (ej: 1.0, 2.0)
{
    nuevaVersion = versionActual + 1.0m; // 1.0 ? 2.0
}
else // Es versión menor (ej: 1.1, 2.5)
{
    nuevaVersion = versionActual + 0.1m; // 1.1 ? 1.2
}
```

### ?? **Proceso de Copia Técnico**

#### **1. ?? Generar Nuevo VersionActual**
```sql
-- Obtener próximo VersionActual único
DECLARE @NuevoVersionActual INT = (SELECT MAX(VersionActual) + 1 FROM CotizacionVersion)
```

#### **2. ?? Calcular Nuevo NumeroVersion**
```csharp
// Lógica descrita arriba
var versionActual = versionOrigen.NumeroVersion;
var nuevaVersion = (versionActual % 1 == 0) ? versionActual + 1.0m : versionActual + 0.1m;
```

#### **3. ?? Copiar Versión Completa**
```sql
-- Insertar nueva versión
INSERT INTO CotizacionVersion (
    CotizacionId, VersionActual, NumeroVersion, 
    FechaVersion, NombreInteresado, EmailInteresado, /* ... todos los campos ... */
) 
SELECT 
    CotizacionId, @NuevoVersionActual, @NuevaVersion,
    GETDATE(), NombreInteresado, EmailInteresado, /* ... todos los campos menos fecha/version ... */
FROM CotizacionVersion 
WHERE VersionId = @VersionOrigenId;
```

#### **4. ?? Copiar Detalles de Versión**
```sql
-- Copiar todos los detalles de la versión origen
INSERT INTO DetalleCotizacionVersion (
    VersionId, ProductoId, Cantidad, PrecioUnitario, Descuento, TotalLinea, /* auditoría */
)
SELECT 
    @NuevaVersionId, ProductoId, Cantidad, PrecioUnitario, Descuento, TotalLinea, /* campos auditoria nuevos */
FROM DetalleCotizacionVersion
WHERE VersionId = @VersionOrigenId;
```

#### **5. ?? Actualizar Puntero y Estado de Cotización**
```sql
-- Hacer que la cotización apunte a la nueva versión Y volver a estado Borrador
UPDATE Cotizacion 
SET VersionActual = @NuevoVersionActual,
    EstadoActual = 'B',  -- ?? CRÍTICO: Siempre vuelve a Borrador
    ModifiedAt = GETDATE(),
    ModifiedBy = @UsuarioActual
WHERE CotizacionId = @CotizacionId;
```

#### **6. ?? Registrar en Historial**
```sql
-- Crear evento en historial (NO copiar historial anterior)
INSERT INTO HistorialCotizacion (VersionId, TipoEvento, FechaEvento, UsuarioEvento, Comentario)
VALUES (@NuevaVersionId, 'VersionGenerada', GETDATE(), @UsuarioActual, 
        'Nueva versión ' + CAST(@NuevaVersion AS VARCHAR(10)) + ' generada desde versión ' + CAST(@VersionOrigen AS VARCHAR(10)));
```

### ?? **Campos que NO se copian**
- ? **HistorialCotizacion**: Cada versión inicia su historial con "VersionGenerada"
- ? **FechaVersion**: Se usa fecha actual
- ? **VersionActual**: Se genera nuevo consecutivo
- ? **NumeroVersion**: Se calcula según regla de incremento
- ? **Campos de auditoría**: CreatedAt, CreatedBy, ModifiedAt, ModifiedBy (nuevos)
- ? **Estado de cotización**: Siempre vuelve a Borrador

### ? **Campos que SÍ se copian**
- ? **Todos los datos del cliente**: Nombre, Email, Empresa
- ? **Todos los montos**: SubTotal, Impuesto, Descuento, Total
- ? **Configuración**: Moneda, TipoCambio
- ? **Notas de la versión anterior**
- ? **Todos los detalles de productos**: ProductoId, Cantidad, Precio, etc.

### ?? **Estado Final Post-Copia**
- ?? **Cotización**: ?? **SIEMPRE en estado Borrador (B)** 
- ?? **Nueva versión**: Se convierte en la versión vigente
- ?? **Versión anterior**: Se vuelve histórica
- ?? **Historial**: Solo tiene evento "VersionGenerada"
- ?? **El usuario debe**: Revisar, aprobar y enviar la nueva versión por el ciclo completo

### ?? **Flujo Completo Post-Copia**
```
1. Usuario copia versión de cotización A (Aprobada)
   ?
2. Se crea nueva versión (ej: 2.0) 
   ?
3. Cotización cambia automáticamente a B (Borrador)
   ?
4. Usuario debe seguir el flujo normal:
   B ? P ? A ? E ? T/R
```

### ?? **Acciones Habilitadas Post-Copia**
- ? **Editar**: Modificar datos de la nueva versión
- ? **Enviar a Aprobación**: B ? P
- ? **Enviar al Cliente**: Debe estar aprobada primero
- ? **Copiar nuevamente**: Hasta que no esté en A o R

---

## ?? SISTEMA DE PERMISOS

### Estructura
- Usuario tiene N Roles
- Cada Rol tiene N Permisos
- Permisos identificados por `Codigo` (ej: "USR_CREATE")
- Permisos agrupados por `Categoria` (ej: "Usuarios")

### Códigos de Permisos (28 total)

#### Usuarios (6 permisos)
- `USR_VIEW` - Ver usuarios del sistema
- `USR_CREATE` - Crear nuevos usuarios
- `USR_EDIT` - Editar usuarios existentes
- `USR_DELETE` - Eliminar usuarios
- `USR_ROLES` - Gestionar roles de usuarios
- `USR_RESET_PWD` - Resetear contraseñas

#### Roles (5 permisos)
- `ROL_VIEW` - Ver roles del sistema
- `ROL_CREATE` - Crear nuevos roles
- `ROL_EDIT` - Editar roles existentes
- `ROL_DELETE` - Eliminar roles
- `ROL_PERMISOS` - Gestionar permisos de roles

#### Cotizaciones (17 permisos)
- `COT_VIEW` - Ver cotizaciones
- `COT_VIEW_DETAIL` - Ver detalle de cotización (solo lectura)
- `COT_VIEW_HISTORY` - Ver historial de cotización
- `COT_VIEW_VERSIONS` - Ver versiones de cotización
- `COT_CREATE` - Crear nuevas cotizaciones
- `COT_EDIT` - Editar cotizaciones
- `COT_DELETE` - Eliminar cotizaciones
- `COT_APPROVE` - Aprobar cotizaciones (Pendiente → Aprobada)
- `COT_REJECT` - Rechazar cotizaciones (Enviada → Rechazada)
- `COT_ACCEPT` - Aceptar cotizaciones (Enviada → Aceptada)
- `COT_SEND_CLIENT` - Enviar cotización al cliente (Aprobada → Enviada)
- `COT_EXPORT` - Exportar cotizaciones
- `COT_COPY` - Copiar versión de cotización
- `COT_DUPLICATE` - Duplicar cotizaciones (crear nueva cotización independiente)
- `COT_ARCHIVE` - Archivar cotizaciones
- `COT_SEND_ERP` - Enviar cotizaciones al ERP

#### Clientes (4 permisos)
- `CLI_VIEW` - Ver clientes
- `CLI_CREATE` - Crear nuevos clientes
- `CLI_EDIT` - Editar clientes
- `CLI_DELETE` - Eliminar clientes

#### Reportes (3 permisos)
- `RPT_VIEW` - Ver reportes
- `RPT_EXPORT` - Exportar reportes
- `RPT_DASHBOARD` - Acceso al dashboard ejecutivo

#### Configuración (3 permisos)
- `CFG_VIEW` - Ver configuración del sistema
- `CFG_EDIT` - Editar configuración del sistema
- `CFG_LOGS` - Ver logs del sistema

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

### Jerarquía de Autorización
```
Usuario con rol Admin/Administrador
  ? Bypass completo (siempre tiene acceso)
  
Usuario Normal
  ?? Verifica permiso específico
      ? Tiene permiso ? Acceso permitido
      ? No tiene permiso ? Elemento deshabilitado/oculto o 403
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
2. `~/css/modals.css` - Estilos de modales (CRITICO - NO MODIFICAR)
3. `~/css/components.css` - Componentes reutilizables
4. `~/css/site.css` - Estilos generales

### ?? Estructura de Botones de Cotizaciones

#### ?? Columna "ESTADOS" - Transiciones de Estado
```html
<div class="btn-group-states">
    <!-- Botones compactos para cambios de estado -->
    <button class="btn-state btn-state-warning">
        <i class="fas fa-paper-plane"></i>
    </button>
</div>
```

#### ?? Columna "ACCIONES" - Acciones Generales
```html
<div class="btn-group-actions">
    <!-- Botones normales para consultas/operaciones -->
    <button class="btn-action btn-action-edit">
        <i class="fas fa-edit"></i>
    </button>
</div>
```

### ?? Paleta de Colores por Estado

#### Estados (Compactos)
- ?? **Naranja** (`#f59e0b`) - Envíos y advertencias
- ?? **Verde** (`#10b981`) - Aprobaciones
- ?? **Azul claro** (`#17a2b8`) - Envío a cliente
- ?? **Azul oscuro** (`#1e40af`) - Acciones principales
- ?? **Rojo** (`#dc3545`) - Rechazos
- ? **Negro** (`#343a40`) - Archivar

#### Acciones (Normales)
- ?? **Amarillo** (`#f59e0b`) - Ver detalle
- ?? **Azul** (`#3b82f6`) - Información
- ?? **Morado** (`#6366f1`) - Versiones
- ?? **Verde** (`#10b981`) - Copiar
- ?? **Rojo** (`#ef4444`) - Duplicar

---

## ?? MODALES (MUY IMPORTANTE)

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

### Modal de Confirmación Genérico
Usar la función `mostrarModalConfirmacion()` en lugar de `confirm()` o `alert()`:

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

---

## ?? ERRORES COMUNES A EVITAR

### 1. NO modificar `.modal` en CSS
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

### 2. NO usar row.data() para filtros
```javascript
/* ? MAL - Cachea valores */
const nombre = row.data('nombre');

/* ? BIEN - Lee del DOM */
const nombre = row.attr('data-nombre');
```

### 3. NO usar confirm() o alert()
```javascript
/* ? MAL - Alerta del navegador */
if (!confirm('Seguro?')) return;

/* ? BIEN - Modal Bootstrap */
mostrarModalConfirmacion('Titulo', 'Mensaje', 'warning', callback);
```

### 4. NO usar Include complejos en EF Core
```csharp
/* ? MAL - EF Core no puede traducir */
.Include(c => c.Versiones.Where(v => v.VersionActual == c.VersionActual))

/* ? BIEN - JOIN explícito */
from cot in context.Cotizaciones
join ver in context.CotizacionesVersiones 
    on new { cot.CotizacionId, VersionId = cot.VersionActual }
    equals new { ver.CotizacionId, VersionId = ver.VersionActual }
```

### 5. NO usar tildes en JavaScript
```javascript
/* ? MAL - Problemas de encoding */
showNotification('error', 'Contraseña incorrecta');

/* ? BIEN - Sin tildes */
showNotification('error', 'Contrasena incorrecta');
```

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

### Migración Aplicada: VersionActual de CHAR a INT
- **Cambio**: `CotizacionVersion.VersionActual` de `char` a `int`
- **Beneficio**: Consultas más eficientes, mejor semántica
- **Conversión**: Automática ('0' ? 0, '1' ? 1)
- **Estado**: ? Aplicada exitosamente

---

## ?? RESOLUCIÓN DE PROBLEMAS TÉCNICOS

### ? Error: System.InvalidOperationException en Cotizaciones
**Causa**: EF Core no puede traducir expresiones Include complejas
**Solución**: Usar JOINs explícitos en lugar de Include con Where
**Estado**: ? Resuelto con JOIN explícito

### ? Performance de Consultas de Versiones
**Problema**: Consultas lentas con múltiples versiones
**Solución**: JOIN directo Cotizacion.VersionActual = CotizacionVersion.VersionActual
**Beneficio**: Una consulta en lugar de múltiples, SQL optimizado

---

## ?? MÓDULOS IMPLEMENTADOS

### Dashboard/Home
- Estadísticas generales
- Actividad reciente
- Accesos rápidos según rol

### Usuarios
- CRUD completo
- Gestión de roles (modal con checkboxes)
- Resetear contraseña (modal con validación)
- Ver roles asignados (modal de solo lectura)
- Filtros colapsables: nombre, email, estado
- Modal de confirmación para eliminar

### Roles
- CRUD completo
- Gestión de permisos agrupados por categoría
- Header de categoría clickeable (selecciona todos)
- Filtros colapsables: nombre, estado
- Validación: no eliminar rol con usuarios asignados

### Cotizaciones
- ? **Listado con versiones vigentes**: Solo muestra versión actual por cotización
- ?? **Filtros colapsables**: búsqueda, fechas, estados (dropdown múltiple)
- ?? **Modal de historial**: Timeline de la versión vigente
- ?? **Modal de versiones**: Lista todas las versiones históricas
- ?? **Operaciones**: Copiar versión, duplicar cotización
- ?? **Estados contextuales**: Botones según estado actual
- ?? **Flujo de estados**: B?P?A?E?T/R?X
- ?? **ERP**: Solo disponible para estado T (Aceptada)

---

## ?? SERVICIOS REGISTRADOS (Program.cs)

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

## ?? DECISIONES DE DISEÑO CLAVE

1. **? Versionado con punteros**: Cotizacion.VersionActual apunta a versión vigente
2. **? Estados estrictos**: Flujo B?P?A?E?T/R?X con validaciones
3. **? ERP solo para aceptadas**: Solo estado T puede ir al ERP
4. **? JOINs explícitos**: Evitar Include complejos en EF Core
5. **? Permisos agrupados**: Por categoría para fácil asignación
6. **? Tag Helper para permisos**: Deshabilita elementos sin JavaScript
7. **? Admin bypass**: Roles Admin/Administrador acceso total
8. **? Modales centrados**: Solo CSS, sin JavaScript para márgenes
9. **? AuditInterceptor automático**: No requiere código en servicios
10. **? Columnas separadas**: Estados vs Acciones en listados

---

**Versión**: 4.1
**Última actualización**: 17 de marzo de 2026  
**Estado**: ✅ Sistema completo con vista de detalle y formateo de números implementados  
**Cambios recientes**: Vista de detalle, helper de formateo, configuración de cultura  
**Autor**: Marcela Jiménez (con GitHub Copilot)

---

## 📋 **CAMBIOS IMPLEMENTADOS HOY (17/03/2026)**

### 🎯 **1. Vista de Detalle de Cotización (NUEVA FUNCIONALIDAD)**

Se implementó una vista completa de solo lectura para mostrar toda la información de las cotizaciones:

#### **Características Principales:**
- ✅ **Modo Solo Lectura**: Vista completa sin capacidad de edición
- ✅ **Responsive Design**: Se adapta a diferentes tamaños de pantalla
- ✅ **Cards Colapsables**: Información General e Información del Cliente son colapsables
- ✅ **Tabla Scrolleable**: Las líneas de detalle tienen scroll cuando hay muchos productos
- ✅ **Versiones Específicas**: Puede mostrar versión actual o versiones históricas
- ✅ **Optimizada para Impresión**: Estilos específicos para impresión

#### **Estructura de la Vista:**
```
┌─ Información General (Colapsable)     ┌─ Resumen Financiero
│  • ID, Estado, Versión, Fechas       │  • Subtotal, Impuesto, Total
├─ Información del Cliente (Colapsable) ├─ Fechas Importantes  
│  • Nombre, Email, Empresa            │  • Envío, Aceptación, ERP
├─ Líneas de Detalle (Scrolleable)     ├─ Notas
│  • Productos, Cantidades, Precios    └─ (Si existen)
└─ Tabla con scroll vertical (500px max)
```

#### **Rutas de Acceso:**
- **Versión Actual**: `/Cotizaciones/Detalle/{cotizacionId}`
- **Versión Específica**: `/Cotizaciones/Detalle/{cotizacionId}?versionId={id}`
- **Desde Listado**: Botón amarillo con icono "edit" 
- **Desde Versiones**: Modal de versiones, botón "Ver Detalle"

#### **ViewModels Creados:**
```csharp
// Vista principal de detalle
public class CotizacionDetalleViewModel
{
    // Información básica de cotización
    public string CotizacionId { get; set; }
    public char EstadoActual { get; set; }
    public DateTime FechaCreacion { get; set; }
    
    // Información de versión
    public decimal NumeroVersion { get; set; }
    public DateTime FechaVersion { get; set; }
    
    // Información del cliente
    public string NombreInteresado { get; set; }
    public string EmailInteresado { get; set; }
    public string EmpresaInteresado { get; set; }
    
    // Información financiera
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; }
    
    // Líneas de detalle
    public List<DetalleCotizacionViewModel> Detalles { get; set; }
}

// Líneas de productos
public class DetalleCotizacionViewModel
{
    public string ProductoId { get; set; }
    public string ProductoNombre { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal TotalLinea { get; set; }
}
```

### 💰 **2. Helper de Formateo Inteligente (NUEVA FUNCIONALIDAD)**

Se creó un helper centralizado para formateo consistente de números, versiones y monedas:

#### **Problema Solucionado:**
- ❌ **Antes**: Separador decimal era coma (`v2,0`, `₡1.234,56`)
- ✅ **Ahora**: Separador decimal es punto (`v2.0`, `₡1,234.56`)
- ❌ **Antes**: Símbolo de moneda fijo (₡)
- ✅ **Ahora**: Símbolo dinámico según la moneda (₡, $, €)

#### **FormatHelper Implementado:**
```csharp
public static class FormatHelper
{
    // Diccionario de símbolos de moneda
    private static readonly Dictionary<string, string> CurrencySymbols = new()
    {
        { "CRC", "₡" },     // Colón costarricense
        { "USD", "$" },     // Dólar estadounidense 
        { "DOL", "$" },     // Dólar (alias)
        { "EUR", "€" },     // Euro
        { "MXN", "$" },     // Peso mexicano
        { "GBP", "£" },     // Libra esterlina
        { "JPY", "¥" },     // Yen japonés
        // ... más monedas según necesidad
    };

    // Formatear versiones con punto decimal
    public static string FormatVersion(decimal version)
    {
        return $"v{version.ToString("0.0", CultureInfo.InvariantCulture)}";
    }

    // Formatear moneda con símbolo dinámico
    public static string FormatCurrency(decimal value, string currency = "CRC")
    {
        var formattedNumber = value.ToString("N2", CultureInfo.InvariantCulture);
        var symbol = GetCurrencySymbol(currency);
        return $"{symbol}{formattedNumber}";
    }

    // Obtener símbolo según código de moneda
    public static string GetCurrencySymbol(string currency)
    {
        if (string.IsNullOrEmpty(currency)) return "₡";
        
        var upperCurrency = currency.ToUpper().Trim();
        return CurrencySymbols.TryGetValue(upperCurrency, out string? symbol) 
            ? symbol 
            : upperCurrency; // Si no conoce la moneda, muestra el código
    }
}
```

#### **Ejemplos de Uso:**
```razor
<!-- Versiones -->
@FormatHelper.FormatVersion(Model.NumeroVersion)  → v2.0

<!-- Monedas dinámicas -->
@FormatHelper.FormatCurrency(1250.50m, "CRC")     → ₡1,250.50
@FormatHelper.FormatCurrency(1250.50m, "USD")     → $1,250.50
@FormatHelper.FormatCurrency(1250.50m, "EUR")     → €1,250.50
@FormatHelper.FormatCurrency(1250.50m, "XYZ")     → XYZ1,250.50
```

#### **Monedas Soportadas:**
- **CRC** (₡) - Colón Costarricense
- **USD/DOL** ($) - Dólar Estadounidense
- **EUR** (€) - Euro
- **MXN** ($) - Peso Mexicano  
- **CAD** ($) - Dólar Canadiense
- **GBP** (£) - Libra Esterlina
- **JPY/CNY** (¥) - Yen/Yuan

### 🛠️ **3. Mejoras en DTOs y Servicios**

#### **CotizacionListDto Actualizado:**
Se agregó el campo `Moneda` para soportar símbolos dinámicos:

```csharp
public record CotizacionListDto(
    int Id,
    string CotizacionId,
    // ... campos existentes ...
    string Moneda, // ← NUEVO CAMPO
    DateTime? FechaAceptacion,
    // ... resto de campos ...
);
```

#### **CotizacionService Actualizado:**
```csharp
// Incluir moneda de la versión vigente
return new CotizacionListDto(
    // ... campos existentes ...
    versionVigente?.Moneda ?? "CRC", // Moneda con fallback
    // ... resto de campos ...
);
```

#### **Controller y ViewModels Actualizados:**
- `CotizacionViewModel` incluye propiedad `Moneda`
- Mapeo actualizado en `CotizacionesController`
- Todas las vistas usan el helper dinámico

### 🌍 **4. Configuración de Cultura (Zona Horaria Costa Rica)**

#### **Problema Solucionado:**
- ❌ **Antes**: Fechas en UTC (diferencia horaria incorrecta)
- ✅ **Ahora**: Fechas en hora local de Costa Rica

#### **Cambios Implementados:**

##### **AuditInterceptor Corregido:**
```csharp
// Antes (problemático)
var now = DateTime.UtcNow;

// Ahora (correcto)
var now = DateTime.Now; // Hora local del servidor
```

##### **Configuración en Program.cs:**
```csharp
// Configuración de zona horaria de Costa Rica
try
{
    var costaRicaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
    TimeZoneInfo.Local.GetHashCode(); // Fuerza inicialización
}
catch (Exception ex)
{
    Log.Warning(ex, "No se pudo configurar zona horaria, usando sistema");
}
```

#### **Beneficios:**
- ✅ Fechas de creación/modificación en hora local
- ✅ Consistencia en todo el sistema
- ✅ Configuración para UTC-6 (Costa Rica)

---

## 🎛️ SISTEMA DE PARÁMETROS Y CONSECUTIVOS

### 🔧 **Parámetros del Sistema**

El sistema maneja parámetros de configuración centralizados en la tabla `ParametrosSistema`:

#### **Estructura de Parámetros:**
```sql
ParametroId      INT IDENTITY(1,1) PRIMARY KEY
Codigo           VARCHAR(100) UNIQUE -- Clave única del parámetro
Descripcion      VARCHAR(500)        -- Descripción legible  
Valor            VARCHAR(1000)       -- Valor actual
TipoValor        CHAR(1)             -- S=String, N=Numeric, B=Boolean, D=Date
Categoria        VARCHAR(100)        -- Agrupación (Consecutivos, Financiero, etc.)
EsModificable    BIT                 -- Si puede ser modificado por usuarios
ValorPorDefecto  VARCHAR(1000)       -- Valor por defecto
Notas            VARCHAR(MAX)        -- Notas adicionales
```

#### **Categorías de Parámetros:**
- **🔢 Consecutivos**: Configuración de generación de IDs
- **💰 Financiero**: Tasas, monedas, tipos de cambio
- **📧 Notificaciones**: Configuración de emails y alertas
- **🔗 Integración**: APIs externas (ERP, HubSpot)
- **🔒 Seguridad**: Timeouts, intentos de login
- **📄 Reportes**: Logos, datos empresa
- **⚙️ Workflow**: Reglas de aprobación

### 🎯 **Sistema de Consecutivos Inteligentes**

#### **Parámetros Críticos:**
- **`MASCARA_CONSECUTIVO_COTIZACION`**: Patrón para generar IDs (ej: `AAA-9999`)
- **`CONSECUTIVO_COTIZACION`**: Consecutivo actual a usar (ej: `COT-0005`)

#### **Caracteres de Máscara:**
- **`A`**: Posición alfabética (A-Z)
- **`9`**: Posición numérica (0-9)  
- **`-`**: Separador fijo (no pueden ser consecutivos)

#### **Ejemplos de Máscaras Válidas:**
```
COT-9999        → COT-0001, COT-0002, ..., COT-9999
AAA-999         → AAA-000, AAA-001, ..., ZZZ-999
A9A-999-AAA     → A0A-000-AAA, A0A-001-AAA, ..., Z9Z-999-ZZZ
COT-9999-9999   → COT-0000-0000, ..., COT-9999-9999
```

#### **Algoritmo de Incremento:**
1. **Incremento de derecha a izquierda**
2. **Overflow numérico**: `9 → 0` (carry al siguiente)
3. **Overflow alfabético**: `Z → A` (carry al siguiente)
4. **Separadores**: Se mantienen fijos
5. **Overflow total**: Lanza `OverflowException`

#### **Ejemplos de Secuencias:**
```
COT-0001 → COT-0002 → ... → COT-0009 → COT-0010
COT-9998 → COT-9999 → [OverflowException]
ABC-999  → ABD-000 → ABD-001
AZZ-999  → BAA-000 → BAA-001
```

### 🏗️ **Arquitectura del Sistema**

#### **Servicios Principales:**

##### **`IParametroSistemaService`**:
```csharp
// Obtener valores de parámetros
Task<string?> ObtenerValorParametroAsync(string codigo);
Task<T?> ObtenerValorParametroAsync<T>(string codigo);

// Actualizar parámetros modificables
Task<bool> ActualizarParametroAsync(string codigo, string nuevoValor);

// Consecutivos
Task<string> ObtenerSiguienteConsecutivoCotizacionAsync();
Task<bool> ValidarConfiguracionConsecutivosAsync();
```

##### **`ConsecutivoGenerator`**:
```csharp
// Validaciones
bool ValidarMascara(string mascara);
bool ValidarConsecutivo(string consecutivo, string mascara);

// Generación
string GenerarSiguienteConsecutivo(string actual, string mascara);
string GenerarPrimerConsecutivo(string mascara);
```

### ⚙️ **Integración con CotizacionService**

#### **Generación de IDs Actualizada:**
```csharp
// 🆕 NUEVO MÉTODO (Recomendado)
private async Task<string> GenerarNuevoCotizacionIdAsync()
{
    try 
    {
        // Usar sistema de parámetros
        return await _parametroService.ObtenerSiguienteConsecutivoCotizacionAsync();
    }
    catch 
    {
        // Fallback al método anterior por seguridad
        return GenerarIdTradicional();
    }
}
```

#### **Beneficios del Nuevo Sistema:**
- ✅ **Configurabilidad**: Máscaras personalizables sin código
- ✅ **Atomicidad**: Transacciones para evitar duplicados
- ✅ **Flexibilidad**: Soporte para patrones complejos
- ✅ **Fallback**: Método anterior como respaldo
- ✅ **Logging**: Trazabilidad completa del proceso
- ✅ **Validación**: Verificación de configuración en runtime

### 🚀 **Implementación y Configuración**

#### **1. Script de Inicialización:**
```bash
# Ejecutar script de parámetros iniciales
sqlcmd -S "localhost" -d "CotizacionesWeb" -i "SeedParametrosSistema.sql"
```

#### **2. Configuración en Program.cs:**
```csharp
// Registrar servicios
builder.Services.AddScoped<ConsecutivoGenerator>();
builder.Services.AddScoped<IParametroSistemaService, ParametroSistemaService>();
```

#### **3. Configuración Inicial Recomendada:**
```sql
-- Configuración conservadora (compatible con sistema actual)
MASCARA_CONSECUTIVO_COTIZACION = 'COT-9999'
CONSECUTIVO_COTIZACION = 'COT-0005'  -- Siguiente al último existente
```

### 🔍 **Validación y Troubleshooting**

#### **Verificar Configuración:**
```csharp
var esValida = await _parametroService.ValidarConfiguracionConsecutivosAsync();
if (!esValida) 
{
    // Configuración incorrecta - revisar parámetros
}
```

#### **Problemas Comunes:**
- ❌ **Máscara inválida**: Caracteres no permitidos o separadores consecutivos
- ❌ **Consecutivo no coincide**: El valor actual no cumple la máscara
- ❌ **Parámetros faltantes**: No están configurados los parámetros críticos
- ❌ **Overflow**: Se alcanzó el máximo posible para la máscara

#### **Logs de Diagnóstico:**
```
[INFO] Consecutivo generado: COT-0004 → COT-0005
[WARN] Error con parámetros, usando fallback
[ERROR] Máscara inválida: COT--999 (separadores consecutivos)
```