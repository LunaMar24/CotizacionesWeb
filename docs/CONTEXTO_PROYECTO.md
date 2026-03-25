# CONTEXTO DEL PROYECTO - CotizacionesWeb

**Para nuevo chat de GitHub Copilot**: Este documento contiene toda la información importante sobre el proyecto, lineamientos, reglas de negocio y patrones establecidos.

---

## 📋 **REGLAS GENERALES DE DESARROLLO**

### ⚠️ **REGLA CRÍTICA: NO CREAR FUNCIONES DE DIAGNÓSTICO AUTOMÁTICAMENTE**

**❌ PROHIBIDO**: Agregar funciones de diagnóstico como `window.diagnosticar*()`, `window.debug*()`, `window.probar*()` sin solicitud explícita del usuario.

**✅ PERMITIDO**: 
- Logging normal con `console.log()` para debugging
- Comentarios explicativos en el código
- Validaciones y manejo de errores estándar

**💡 RAZÓN**: Las funciones de diagnóstico deben ser solicitadas explícitamente por el usuario cuando sean necesarias para troubleshooting específico.

### 🔒 **REGLAS DE EDICIÓN DE MONEDA**

**La moneda de una cotización SOLO se puede cambiar cuando se cumplen TODAS estas condiciones:**

1. ✅ **Estado Borrador**: La cotización debe estar en estado `'B'` (Borrador)
2. ✅ **Sin líneas de detalle**: NO debe tener ninguna línea en la tabla (ni temporales ni persistentes)
3. ✅ **Tabla vacía**: `totalLineas === 0`

**❌ NO se puede cambiar la moneda si:**
- Hay cualquier línea en la tabla (aunque no esté guardada)
- La cotización está en estado diferente a Borrador
- Ya hay productos/servicios agregados

**💡 RAZÓN**: Los precios ya están expresados en la moneda original. Cambiar la moneda con líneas existentes causaría inconsistencias financieras.

---

## 🏛️ **PRINCIPIOS FUNDAMENTALES DE DESARROLLO**

### 📖 **CONTEXTO DEL PROYECTO**

**Tipo de Proyecto**: ASP.NET Core MVC (.NET 8) - **NO es Blazor ni Razor Pages puro**

**Arquitectura**: Clean Architecture con separación en capas:
- **UI**: Controllers, Views, JavaScript, CSS
- **Application**: Interfaces, DTOs, Casos de uso
- **Infrastructure**: EF Core, Servicios, Implementaciones
- **Domain**: Entidades, Enums, Reglas de negocio

**Stack Tecnológico**:
- .NET 8 / ASP.NET Core MVC
- Entity Framework Core 8 (Code First)
- SQL Server (LocalDB dev / Azure prod)
- AdminLTE 3.2 + Bootstrap 4.6
- jQuery 3.x (JavaScript principal)
- Font Awesome 6.4.0

### 🔄 **PRINCIPIO DRY (Don't Repeat Yourself) - CRÍTICO**

**📜 REGLA FUNDAMENTAL**: Antes de crear nuevas implementaciones, SIEMPRE revisar si ya existe funcionalidad similar que pueda ser reutilizada.

#### **✅ IMPLEMENTACIONES EXITOSAS DE DRY:**

##### **💰 FormatHelper - Formateo de Monedas:**
```csharp
// ❌ MAL - Duplicación en controlador
private List<(string codigo, string simbolo, string nombre)> ObtenerMonedasDisponibles()
{
    return new List<(string codigo, string simbolo, string nombre)>
    {
        ("CRC", "₡", "Colón Costarricense"), // Duplicado del FormatHelper
        ("USD", "$", "Dólar Estadounidense"),
        // ...
    };
}

// ✅ BIEN - Reutilización del helper existente
ViewBag.MonedasDisponibles = FormatHelper.GetMonedasDisponibles(); // Usa fuente única
```

##### **🎨 CSS y Estilos:**
```css
/* ❌ MAL - Duplicar estilos de badges en cada archivo CSS */
.badge-custom-moneda { ... }  /* En index.css */
.badge-custom-currency { ... }  /* En detalle.css - DUPLICADO */

/* ✅ BIEN - Definir una vez, reutilizar en todo el sistema */
.badge-moneda { ... }  /* En components.css - ÚNICO */
```

##### **📊 Estados de Cotización:**
```javascript
// ❌ MAL - Duplicar lógica de estados en cada script
const estados = { 'B': 'Borrador', 'P': 'Pendiente' }; // En index.js
const estadosBorrador = { 'B': 'Borrador' }; // En editar.js - DUPLICADO

// ✅ BIEN - Configuración centralizada
FormatUtils.getEstadoTexto('B'); // Usa FormatConfig centralizado
```

#### **🛠️ COMPONENTES REUTILIZABLES EXISTENTES:**

##### **1. FormatHelper (C# - Servidor):**
- **Propósito**: Formateo consistente de monedas, versiones y números
- **Ubicación**: `CotizacionesWeb.UI.Helpers.FormatHelper`
- **Uso**: `@FormatHelper.FormatCurrency(valor, moneda)`
- **Funciones disponibles**:
  ```csharp
  FormatHelper.FormatCurrency(decimal, string)     // Monedas con símbolo
  FormatHelper.GetCurrencySymbol(string)           // Solo símbolo
  FormatHelper.FormatVersion(decimal)              // Versiones (v2.0)
  FormatHelper.GetMonedasDisponibles()             // Lista completa
  FormatHelper.IsSupportedCurrency(string)         // Validación
  ```

##### **2. FormatUtils (JavaScript - Cliente):**
- **Propósito**: Lógica de estados, transiciones y formateo del lado cliente
- **Ubicación**: `~/js/shared/format-config.js`
- **Uso**: `FormatUtils.isEditable(estado)`
- **Funciones disponibles**:
  ```javascript
  FormatUtils.formatCurrency(value, currency)     // Formateo moneda
  FormatUtils.isEditable(estado)                  // Validación edición
  FormatUtils.getEstadoTexto(estado)             // Texto de estado
  FormatUtils.isTransicionPermitida(origen, destino) // Flujo estados
  ```

##### **3. Estilos CSS Centralizados:**
- **Ubicación**: `~/css/components.css` (global), archivos específicos por módulo
- **Componentes reutilizables**:
  ```css
  .badge-moneda          /* Badges de moneda */
  .badge-version         /* Badges de versión */
  .badge-status          /* Estados de cotización */
  .btn-action           /* Botones de acciones */
  .btn-state            /* Botones de transición de estado */
  ```

##### **4. Modal de Confirmación:**
- **Ubicación**: Incluido en `_Layout.cshtml`
- **Uso**: `mostrarModalConfirmacion(titulo, mensaje, tipo, callback)`
- **Ventaja**: Consistencia visual y funcional en toda la aplicación

#### **⚠️ REGLAS CRÍTICAS PARA MANTENER DRY:**

##### **🔍 ANTES DE CREAR, VERIFICAR:**
1. **¿Existe un helper para esto?** Revisar `FormatHelper.cs` y `FormatUtils.js`
2. **¿Hay CSS similar?** Buscar en `components.css` y archivos de módulo
3. **¿Ya se implementó esta lógica?** Revisar servicios y controladores existentes
4. **¿Existe un modal para esto?** Usar modal de confirmación genérico

##### **🚫 NUNCA DUPLICAR:**
1. **Diccionarios de monedas** - Usar `FormatHelper.CurrencySymbols`
2. **Lógica de estados** - Usar `FormatConfig.estados`
3. **Validaciones de permisos** - Usar `RequierePermiso` attribute
4. **Formateo de números** - Usar `FormatHelper` methods
5. **Estilos de badges** - Extender clases existentes

##### **🔄 AL ENCONTRAR DUPLICACIÓN:**
1. **Identificar el original** y el duplicado
2. **Centralizar en el componente principal**
3. **Actualizar todas las referencias** al componente centralizado
4. **Eliminar el código duplicado**
5. **Documentar en este archivo** para futuras referencias

#### **📝 EJEMPLOS DE REFACTORIZACIÓN DRY:**

##### **Caso Real - Monedas (Sesión 3):**
```
ANTES:
├── FormatHelper.cs (CurrencySymbols)     ← Original
└── CotizacionesController.cs (ObtenerMonedasDisponibles) ← Duplicado

PROBLEMA:
- Mantenimiento en dos lugares
- Posible inconsistencia de datos
- Violación DRY

DESPUÉS:
├── FormatHelper.cs (única fuente)
│   ├── CurrencySymbols (privado)
│   └── GetMonedasDisponibles() (público)
└── CotizacionesController.cs → FormatHelper.GetMonedasDisponibles()

RESULTADO:
✅ Mantenimiento en un solo lugar
✅ Consistencia garantizada  
✅ Principio DRY cumplido
```

#### **🎯 CHECKLIST PARA NUEVAS FUNCIONALIDADES:**

```
□ ¿Revise FormatHelper para funciones de formateo?
□ ¿Verifique FormatUtils para lógica de estados?
□ ¿Busque en components.css estilos similares?
□ ¿Existe un servicio que haga algo parecido?
□ ¿Hay un modal genérico que pueda usar?
□ ¿La nueva función puede ser útil para otros módulos?
□ Si es reutilizable, ¿la ubique en el lugar correcto?
□ ¿Actualice esta documentación con la nueva función?
```

#### **💡 BENEFICIOS DE MANTENER DRY:**

1. **🚀 Mantenimiento Simplificado**: Un solo lugar para cambios
2. **🛡️ Consistencia Garantizada**: Comportamiento uniforme
3. **📈 Performance Mejorada**: Menos código duplicado
4. **🐛 Menos Bugs**: Una implementación bien probada
5. **👥 Colaboración Eficiente**: Desarrolladores saben dónde buscar funcionalidad

---

## 🏗️ **PRINCIPIOS DE ARQUITECTURA Y DATOS**

### 📊 **PRINCIPIO: PRESERVACIÓN DE AUDITORÍA EN ACTUALIZACIONES**

**📜 REGLA FUNDAMENTAL**: NUNCA eliminar y recrear registros cuando se pueden actualizar. Esto preserva la integridad de la auditoría (`CreatedAt`, `CreatedBy`, `ModifiedAt`, `ModifiedBy`).

#### **✅ PATRÓN IMPLEMENTADO: UPDATE/INSERT/DELETE SELECTIVO**

##### **🔧 Estrategia para Actualización de Detalles:**
```csharp
// ❌ MAL - Rompe auditoría
_context.DetallesCotizacionVersion.RemoveRange(detallesExistentes); // Pierde CreatedAt
foreach (var detalle in nuevosDetalles) {
    _context.Add(new DetalleCotizacionVersion { ... }); // Nuevos CreatedAt incorrectos
}

// ✅ BIEN - Preserva auditoría
private async Task ActualizarDetallesVersionAsync(int versionId, List<ActualizarDetalleRequest> nuevosDetalles)
{
    // 1. UPDATE: Actualizar registros existentes (preserva CreatedAt/CreatedBy)
    // 2. INSERT: Agregar nuevos registros (DetalleVersionId = 0)  
    // 3. DELETE: Eliminar solo los que realmente se eliminaron
}
```

#### **🛠️ IMPLEMENTACIÓN EN CotizacionService:**

##### **Método ActualizarDetallesVersionAsync:**
```csharp
// 1. OBTENER detalles existentes
var detallesExistentes = await _context.DetallesCotizacionVersion
    .Where(d => d.VersionId == versionId).ToListAsync();

// 2. ACTUALIZAR existentes que siguen en el request
foreach (var detalleRequest in nuevosDetalles.Where(d => d.DetalleVersionId > 0))
{
    if (detallesExistentesDict.TryGetValue(detalleRequest.DetalleVersionId, out var existente))
    {
        // Preserva CreatedAt y CreatedBy originales
        existente.ProductoId = detalleRequest.ProductoId;
        existente.Cantidad = detalleRequest.Cantidad;
        // ModifiedAt y ModifiedBy → AuditInterceptor automático
    }
}

// 3. INSERTAR nuevos (DetalleVersionId = 0)
foreach (var detalleRequest in nuevosDetalles.Where(d => d.DetalleVersionId == 0))
{
    var nuevoDetalle = new DetalleCotizacionVersion { ... };
    _context.Add(nuevoDetalle); // CreatedAt y CreatedBy → AuditInterceptor
}

// 4. ELIMINAR solo los que no están en el request
var detallesAEliminar = detallesExistentes
    .Where(d => !idsEnRequest.Contains(d.DetalleVersionId)).ToList();
_context.RemoveRange(detallesAEliminar); // Solo elimina lo necesario
```

#### **📋 DTO con Estrategia Clara:**
```csharp
public record ActualizarDetalleRequest
{
    public int DetalleVersionId { get; set; } // 0 = nuevo, >0 = actualizar existente
    public string ProductoId { get; set; }
    public decimal Cantidad { get; set; }
    // ... otros campos
}
```

#### **⚡ BENEFICIOS DE ESTA ESTRATEGIA:**

##### **🛡️ Auditoría Preservada:**
- ✅ **CreatedAt original**: Mantiene fecha/hora real de creación
- ✅ **CreatedBy original**: Preserva quién creó realmente el registro
- ✅ **ModifiedAt actualizado**: Refleja cuándo fue la última modificación
- ✅ **ModifiedBy actualizado**: Muestra quién hizo el cambio

##### **📈 Performance Optimizada:**
- ✅ **Menos operaciones de BD**: Solo actualiza lo que cambió
- ✅ **Transacciones más eficientes**: Menos inserts/deletes innecesarios
- ✅ **Índices preservados**: No rompe claves primarias existentes

##### **🔍 Trazabilidad Completa:**
- ✅ **Historial real**: Auditoría muestra evolución real de los datos
- ✅ **Investigación**: Posible determinar cuándo se creó vs cuándo se modificó cada línea
- ✅ **Compliance**: Cumple requisitos de auditoría empresarial

#### **⚠️ REGLAS CRÍTICAS A SEGUIR:**

##### **🔍 IDENTIFICACIÓN DE ESTRATEGIA:**
1. **DetalleVersionId = 0**: Es un registro nuevo → INSERT
2. **DetalleVersionId > 0 y en request**: Existe y se mantiene → UPDATE  
3. **DetalleVersionId > 0 y NO en request**: Existe pero se eliminó → DELETE
4. **Nunca hacer**: DELETE de todos + INSERT de todos

##### **🚫 NUNCA HACER:**
```csharp
// ❌ ANTI-PATRÓN: Eliminar todo y recrear
context.RemoveRange(entity.Children);
foreach(var item in newItems) {
    context.Add(new Child(item)); // ¡Pierde auditoría!
}
```

##### **🔄 APLICAR PATRÓN EN:**
- **✅ Detalles de cotización** (ya implementado)
- **✅ Líneas de facturas** (futuro)
- **✅ Items de órdenes** (futuro)  
- **✅ Elementos de listas dinámicas** (futuro)

#### **📝 EJEMPLO DE AUDITORÍA PRESERVADA:**

##### **Escenario Real:**
```
Usuario crea línea: PROD001 x 10 unidades
├─ CreatedAt: 2026-03-15 10:30:00
├─ CreatedBy: usuario@empresa.com
├─ ModifiedAt: 2026-03-15 10:30:00  
└─ ModifiedBy: usuario@empresa.com

Usuario modifica cantidad: PROD001 x 15 unidades  
├─ CreatedAt: 2026-03-15 10:30:00    ← PRESERVADO
├─ CreatedBy: usuario@empresa.com    ← PRESERVADO
├─ ModifiedAt: 2026-03-15 14:45:00   ← ACTUALIZADO
└─ ModifiedBy: supervisor@empresa.com ← ACTUALIZADO

RESULTADO: ✅ Se puede saber cuándo se creó Y cuándo se modificó
```

##### **Con Anti-Patrón (eliminar/recrear):**
```
Usuario modifica cantidad: PROD001 x 15 unidades
├─ CreatedAt: 2026-03-15 14:45:00    ← ¡INCORRECTO! (debería ser 10:30)
├─ CreatedBy: supervisor@empresa.com ← ¡INCORRECTO! (debería ser usuario@empresa.com)
├─ ModifiedAt: 2026-03-15 14:45:00
└─ ModifiedBy: supervisor@empresa.com

RESULTADO: ❌ Se perdió la información de cuándo/quién creó originalmente
```

#### **💡 IMPLEMENTACIONES FUTURAS:**
Este patrón debe aplicarse a:
- **Edición de facturas** 
- **Modificación de órdenes de compra**
- **Actualización de inventarios**
- **Cualquier entidad con relaciones uno-a-muchos editables**

### 🔄 **REGLAS DE NEGOCIO PARA CAMBIO DE MONEDA**

**📜 REGLA FUNDAMENTAL**: El cambio de moneda está estrictamente controlado para evitar inconsistencias financieras.

#### **✅ CONDICIONES PARA PERMITIR CAMBIO DE MONEDA:**

La moneda de una cotización **SOLO** se puede cambiar cuando se cumplen **TODAS** estas condiciones:

1. **📝 Estado Borrador**: La cotización debe estar en estado `'B'` (Borrador)
2. **📋 Sin líneas**: NO debe tener ninguna línea de detalle (ni guardadas ni temporales)
3. **🔢 Tabla vacía**: `totalLineas === 0` (la tabla de productos debe estar completamente vacía)

#### **❌ ESCENARIOS DONDE NO SE PERMITE:**

##### **🚫 Con Líneas de Detalle:**
```csharp
// ❌ NO PERMITIDO
var tieneLineas = $('#tablaDetalles tbody tr').length > 0;
if (tieneLineas) {
    return Error("No se puede cambiar la moneda cuando hay líneas de detalle");
}
```
**Justificación**: Los precios de productos están expresados en la moneda original; cambiar la moneda haría que los precios no correspondan.

##### **🚫 Estados Diferentes a Borrador:**
```csharp
// ❌ NO PERMITIDO  
if (cotizacion.EstadoActual != 'B') {
    return Error("Solo se puede cambiar la moneda en estado Borrador");
}
```
**Justificación**: Cotizaciones en otros estados han pasado por procesos de aprobación que no deben alterarse.

#### **🔧 IMPLEMENTACIÓN TÉCNICA:**

##### **Backend (Servicio):**
```csharp
// En CotizacionService.cs - Método ActualizarCotizacionAsync
bool puedeActualizarMoneda = false;
if (!string.IsNullOrEmpty(request.Moneda) && request.Moneda != cotizacion.Moneda)
{
    // Verificar estado Borrador
    if (cotizacion.EstadoActual == 'B')
    {
        // Verificar que NO haya líneas (ni una sola)
        var tieneDetalles = await _context.DetallesCotizacionVersion
            .AnyAsync(d => d.VersionId == version.VersionId);

        if (!tieneDetalles)
        {
            puedeActualizarMoneda = true;
            // Cambio autorizado
        }
        else
        {
            return new ActualizarCotizacionResult(false, 
                "No se puede cambiar la moneda cuando hay líneas de detalle");
        }
    }
    else
    {
        return new ActualizarCotizacionResult(false, 
            "Solo se puede cambiar la moneda en estado Borrador");
    }
}
```

##### **Frontend (JavaScript):**
```javascript
// En editar.js - Validación del combo de moneda
$('#MonedaSelect').on('change', function() {
    const totalLineas = $('#tablaDetalles tbody tr').length;
    
    // ❌ Bloquear si hay CUALQUIER línea (temporal o persistente)
    if (totalLineas > 0) {
        showNotification('warning', 
            'No se puede cambiar la moneda cuando hay líneas de detalle.');
        $(this).val(monedaAnterior); // Revertir
        return;
    }
    
    // ✅ Proceder con modal de confirmación si no hay líneas
    mostrarModalConfirmacionMoneda(...);
});
```

#### **🎯 FLUJOS DE USUARIO DOCUMENTADOS:**

##### **✅ Flujo Exitoso:**
1. **Usuario crea cotización** → Estado: Borrador
2. **NO agrega productos** → Sin líneas de detalle
3. **Cambia moneda** → Sistema solicita confirmación
4. **Confirma cambio** → Moneda actualizada exitosamente

##### **❌ Flujo Bloqueado:**
1. **Usuario crea cotización** → Estado: Borrador  
2. **Agrega 1 línea de producto** → Tiene líneas de detalle
3. **Intenta cambiar moneda** → ❌ Sistema bloquea el cambio
4. **Ve mensaje** → "No se puede cambiar cuando hay líneas de detalle"
5. **Solución**: Eliminar todas las líneas primero

#### **💡 DIFERENCIA CON REGLA ANTERIOR:**

**❌ REGLA ANTERIOR (INCORRECTA)**:
- Se permitía cambio si solo había líneas "temporales" (no guardadas)
- Problema: Usuarios podían cambiar moneda con productos en la tabla

**✅ REGLA ACTUAL (CORRECTA)**:
- NO se permite cambio si hay CUALQUIER línea (guardada o temporal)
- Justificación: Los precios ya están ingresados y no se convertirían automáticamente

---

## 💰 **GESTIÓN DE MONEDAS Y FORMATO FINANCIERO**

### 🌍 **MONEDAS SOPORTADAS**

El sistema maneja múltiples monedas con soporte completo para simbología, formato y conversión:

#### **💱 Monedas Disponibles:**

| Código | Símbolo | Nombre Completo | Uso Principal |
|--------|---------|-----------------|---------------|
| **CRC** | **₡** | **Colón Costarricense** | **Moneda predeterminada** |
| **USD** | **$** | **Dólar Estadounidense** | Clientes internacionales |
| **EUR** | **€** | **Euro** | Clientes europeos |
| **MXN** | **$** | Peso Mexicano | Expansión regional |
| **CAD** | **$** | Dólar Canadiense | Mercado norteamericano |
| **GBP** | **£** | Libra Esterlina | Mercado británico |

#### **🔧 Configuración Técnica:**

##### **Símbolos Unicode (Evitar Problemas de Encoding):**
```javascript
const symbols = {
    'CRC': '\u00A2',    // ₡ (Unicode: U+00A2) 
    'USD': '$',         // Dólar estadounidense
    'EUR': '\u20AC',    // € (Unicode: U+20AC)
    'GBP': '\u00A3',    // £ (Unicode: U+00A3)
    'JPY': '\u00A5',    // ¥ (Unicode: U+00A5)
};
```

##### **Formato de Números por Moneda:**
```csharp
// En FormatHelper.cs
public static string FormatCurrency(decimal value, string currency)
{
    var symbol = GetCurrencySymbol(currency);
    var formatted = value.ToString("N2", CultureInfo.InvariantCulture);
    return $"{symbol}{formatted}";
}
```

### 📊 **REGLAS DE FORMATO FINANCIERO**

#### **🎯 Estándares de Presentación:**

##### **✅ Formato Correcto:**
```
Subtotal:           ₡125,000.00
Descuento:         -₡5,000.00  
Subtotal c/Desc:    ₡120,000.00
Impuesto (13%):     ₡15,600.00
─────────────────────────────
Total:              ₡135,600.00
```

##### **❌ Formato Incorrecto:**
```
Subtotal: ¢125000      // Sin separadores ni decimales
Descuento: ¢-5000      // Símbolo mal ubicado  
Total: 135,600 CRC     // Inconsistente
```

#### **🔢 Reglas de Cálculo:**

##### **📐 Fórmulas Estándar:**
```csharp
// 1. Subtotal = Σ(Cantidad × PrecioUnitario)
decimal subtotal = detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

// 2. Subtotal con Descuentos = Subtotal - Σ(Descuentos)
decimal subtotalConDescuentos = subtotal - detalles.Sum(d => d.Descuento);

// 3. Impuesto = SubtotalConDescuentos × 0.13 (13% Costa Rica)
decimal impuesto = subtotalConDescuentos * 0.13m;

// 4. Total = SubtotalConDescuentos + Impuesto
decimal total = subtotalConDescuentos + impuesto;
```

##### **⚖️ Precisión Decimal:**
- **Cálculos internos**: `decimal` (precisión máxima)
- **Base de datos**: `DECIMAL(18,2)` 
- **Presentación**: 2 decimales siempre
- **Redondeo**: Banker's rounding (estándar .NET)

### 🔄 **CONVERSIÓN Y TIPOS DE CAMBIO**

#### **💹 Gestión de Tipos de Cambio:**

##### **🎯 Reglas de Negocio:**
1. **Moneda predeterminada**: Todas las cotizaciones nuevas en CRC
2. **Cambio permitido**: Solo en versión 1.0, estado Borrador, sin detalles
3. **Tipo de cambio**: Se mantiene en la versión para auditoría
4. **Conversión**: Manual (el usuario ingresa precios en la moneda seleccionada)

##### **📊 Estructura de Datos:**
```csharp
// Tabla Cotizaciones
public string Moneda { get; set; } = "CRC"; // Moneda de la cotización

// Tabla CotizacionesVersiones  
public decimal? TipoCambio { get; set; } // Tipo de cambio histórico (futuro)
```

#### **🎯 Futuras Integraciones:**
```csharp
// Posible integración con API de tipos de cambio
public interface ITipoCambioService
{
    Task<decimal> GetTipoCambioAsync(string from, string to, DateTime fecha);
    Task<Dictionary<string, decimal>> GetTiposCambioActualesAsync();
}
```

### 🎨 **EXPERIENCIA DE USUARIO CON MONEDAS**

#### **🔄 Cambio de Moneda en Interfaz:**

##### **✅ Flujo Optimizado:**
1. **Detección automática** de si se puede cambiar
2. **Combo habilitado** solo cuando es posible
3. **Feedback inmediato** si no es posible
4. **Confirmación requerida** para cambios
5. **Actualización visual** instantánea

##### **🎯 Componentes Reactivos:**
```javascript
// Actualización automática de displays al cambiar moneda
function aplicarCambioMoneda(nuevaMoneda) {
    monedaActual = nuevaMoneda;
    
    // Actualizar todos los displays financieros
    actualizarDisplaysMoneda();
    
    // Actualizar configuración global
    if (window.FormatConfig) {
        window.FormatConfig.moneda = nuevaMoneda;
        window.FormatConfig.simboloMoneda = getCurrencySymbol(nuevaMoneda);
    }
}
```

#### **🚨 Validaciones de UX:**

##### **💡 Validaciones Proactivas:**
```javascript
// Prevención de errores antes de envío
$('#MonedaSelect').on('change', function() {
    const totalLineas = $('#tablaDetalles tbody tr').length;
    
    if (totalLineas > 0) {
        // Revertir selección automáticamente
        $(this).val(monedaAnterior);
        showNotification('warning', 
            'No se puede cambiar la moneda cuando hay líneas de detalle.');
        return;
    }
});
```

##### **✅ Feedback Positivo:**
- **Confirmación visual** cuando el cambio es exitoso
- **Actualización inmediata** de todos los totales
- **Persistencia** del cambio en la sesión

### 📋 **MEJORES PRÁCTICAS PARA DESARROLLADORES**

#### **🔧 Implementación de Nuevas Monedas:**

##### **1. Agregar al enum/constants:**
```csharp
public static readonly Dictionary<string, (string Symbol, string Name)> SupportedCurrencies = new()
{
    ["CRC"] = ("₡", "Colón Costarricense"),
    ["USD"] = ("$", "Dólar Estadounidense"),
    ["EUR"] = ("€", "Euro"),
    ["NUEVA"] = ("§", "Nueva Moneda")  // Ejemplo
};
```

##### **2. Actualizar FormatHelper:**
```csharp
public static List<(string codigo, string simbolo, string nombre)> GetMonedasDisponibles()
{
    return SupportedCurrencies.Select(kv => 
        (kv.Key, kv.Value.Symbol, kv.Value.Name)).ToList();
}
```

##### **3. Agregar validaciones:**
```csharp
public static bool IsSupportedCurrency(string currency)
{
    return SupportedCurrencies.ContainsKey(currency?.ToUpper() ?? "");
}
```

#### **⚠️ Validaciones Críticas:**

##### **🚨 Nunca Hacer:**
```csharp
// ❌ MAL - Hardcodear monedas
if (moneda == "CRC" || moneda == "USD") { }

// ❌ MAL - Asumir formato
var precio = "$" + valor.ToString();

// ❌ MAL - Ignorar validaciones
cotizacion.Moneda = request.Moneda; // Sin verificar reglas
```

##### **✅ Siempre Hacer:**
```csharp
// ✅ BIEN - Usar funciones centralizadas
if (FormatHelper.IsSupportedCurrency(moneda)) { }

// ✅ BIEN - Formato consistente  
var precio = FormatHelper.FormatCurrency(valor, moneda);

// ✅ BIEN - Validar reglas de negocio
if (PuedeActualizarMoneda(cotizacion, version)) {
    cotizacion.Moneda = request.Moneda;
}
```

---

## 🎨 **INTERFAZ DE USUARIO Y NOTIFICACIONES**

### 📢 **SISTEMA DE NOTIFICACIONES GLOBAL**

**📜 REGLA FUNDAMENTAL**: NUNCA usar `alert()`, `confirm()` o `prompt()` del navegador. El proyecto tiene un sistema de notificaciones centralizado y elegante implementado en `site.js`.

#### **✅ FUNCIONES GLOBALES DISPONIBLES:**

##### **🔔 `window.showNotification(type, message)`**
**Ubicación**: `src/CotizacionesWeb.UI/wwwroot/js/site.js`

```javascript
// ✅ CORRECTO - Usar función global
window.showNotification('success', 'Cotización guardada exitosamente');
window.showNotification('error', 'Error al procesar los datos');
window.showNotification('warning', 'Advertencia: Datos incompletos');
window.showNotification('info', 'Información actualizada');

// ❌ INCORRECTO - NUNCA usar
alert('Error al guardar'); // ¡MAL!
confirm('¿Está seguro?'); // ¡MAL!
```

**Características:**
- ✅ **Estilos consistentes**: Usa clases Bootstrap del tema
- ✅ **Auto-dismiss**: Se cierra automáticamente después de 5 segundos
- ✅ **Responsive**: Funciona en desktop y móvil
- ✅ **Iconografía**: Incluye iconos Font Awesome apropiados
- ✅ **Posicionamiento**: Se inserta al inicio del área de contenido

##### **🚨 `window.showModalAlert(modalAlertId, message, type)`**
**Para alertas dentro de modales específicos:**

```javascript
// Mostrar error dentro de un modal
window.showModalAlert('modalEditarDetalleAlert', 'Campo requerido', 'danger');
window.showModalAlert('modalConfirmAlert', 'Operación exitosa', 'success');

// Ocultar alerta del modal
window.hideModalAlert('modalEditarDetalleAlert');
```

**Características:**
- ✅ **Scope local**: Solo afecta el modal específico
- ✅ **Auto-scroll**: Hace scroll al inicio del modal al mostrar
- ✅ **Tipos soportados**: 'danger', 'warning', 'info', 'success'

#### **🎯 IMPLEMENTACIÓN EN CÓDIGO JAVASCRIPT:**

##### **✅ Patrón Correcto para Notificaciones:**
```javascript
function showNotification(type, message) {
    // Usar la función global de site.js
    if (typeof window.showNotification === 'function' && 
        window.showNotification !== showNotification) {
        try {
            window.showNotification(type, message);
        } catch (error) {
            // Solo console como fallback (no alert)
            console.log(`[${type.toUpperCase()}] ${message}`);
        }
    } else {
        // Fallback seguro - solo console
        console.log(`[${type.toUpperCase()}] ${message}`);
    }
}
```

##### **❌ Anti-Patrones a Evitar:**
```javascript
// ❌ NUNCA HACER ESTO:
alert('Error al guardar');
confirm('¿Desea continuar?');
prompt('Ingrese valor:');

// ❌ TAMPOCO ESTO:
$('#miModal').modal('show');
$('#miModal .modal-body').html('<div class="alert alert-danger">Error</div>');

// ❌ NI ESTO:
if (error) {
    $('.content').prepend('<div class="alert alert-danger">Error</div>');
}
```

#### **🛠️ CONFIGURACIÓN EN VISTAS RAZOR:**

##### **✅ Estructura HTML para Alertas en Modales:**
```razor
<!-- Dentro de modal-body -->
<div id="modalEditarDetalleAlert" class="alert alert-danger d-none" role="alert">
    <strong>Error:</strong> <span id="modalEditarDetalleAlertMessage"></span>
</div>
```

##### **✅ Scripts de Vista Consistentes:**
```javascript
@section Scripts {
    <script>
        // Configuración específica del servidor
        if (window.FormatConfig) {
            window.FormatConfig.moneda = '@Model.Moneda';
            window.FormatConfig.estado = '@Model.EstadoActual';
        }
        
        // JavaScript específico de la vista
        $(document).ready(function() {
            // Usar showNotification para feedback al usuario
            function procesarFormulario() {
                // ... lógica
                if (success) {
                    window.showNotification('success', 'Operación exitosa');
                } else {
                    window.showNotification('error', 'Error en la operación');
                }
            }
        });
    </script>
    <script src="~/js/miVista.js" asp-append-version="true"></script>
}
```

#### **📋 TIPOS DE NOTIFICACIÓN ESTÁNDAR:**

##### **🎨 Mapeo de Tipos vs Estilos:**
| Tipo | Clase CSS | Icono | Uso |
|------|-----------|--------|-----|
| `success` | `alert-success` | `fa-check-circle` | Operaciones exitosas |
| `error` | `alert-danger` | `fa-exclamation-circle` | Errores y fallos |
| `warning` | `alert-warning` | `fa-exclamation-triangle` | Advertencias |
| `info` | `alert-info` | `fa-info-circle` | Información general |

##### **💡 Ejemplos de Uso por Contexto:**
```javascript
// Guardado exitoso
window.showNotification('success', 'Cotización guardada exitosamente');

// Error de validación
window.showNotification('error', 'El nombre del interesado es obligatorio');

// Advertencia de estado
window.showNotification('warning', 'Solo las cotizaciones en estado Borrador pueden editarse');

// Información de proceso
window.showNotification('info', 'Procesando datos, por favor espere...');
```

#### **🔗 INTEGRACIÓN CON SISTEMA GLOBAL:**

##### **✅ Verificación de Disponibilidad:**
```javascript
// Verificar que el sistema está disponible antes de usar
function notificarUsuario(tipo, mensaje) {
    if (typeof window.showNotification === 'function') {
        window.showNotification(tipo, mensaje);
    } else {
        // Fallback para desarrollo/debug
        console.warn('Sistema de notificaciones no disponible');
        console.log(`[${tipo}] ${mensaje}`);
    }
}
```

##### **🚫 NUNCA Implementar Sistema Propio:**
```javascript
// ❌ MAL - Reimplementar notificaciones
function miShowNotification(mensaje) {
    const div = $('<div class="alert alert-info">').text(mensaje);
    $('body').prepend(div);
    setTimeout(() => div.remove(), 3000);
}

// ✅ BIEN - Usar sistema global
window.showNotification('info', mensaje);
```

#### **🧪 TESTING Y DEBUGGING:**

##### **🔍 Comandos de Consola Útiles:**
```javascript
// Probar notificaciones en consola del navegador
window.showNotification('success', 'Prueba éxito');
window.showNotification('error', 'Prueba error');
window.showNotification('warning', 'Prueba advertencia');
window.showNotification('info', 'Prueba información');

// Verificar disponibilidad
console.log('showNotification disponible:', typeof window.showNotification === 'function');

// Ver configuración actual
console.log('Función showNotification:', window.showNotification);
```

##### **📝 Logging para Desarrollo:**
```javascript
// En archivos JS específicos de vista
function miFuncion() {
    try {
        // ... lógica
        window.showNotification('success', 'Operación completada');
    } catch (error) {
        console.error('Error en miFuncion:', error);
        window.showNotification('error', 'Error inesperado en la operación');
    }
}
```

#### **⚠️ REGLAS CRÍTICAS:**

##### **🚫 PROHIBIDO:**
1. **Usar alert/confirm/prompt** del navegador
2. **Crear sistemas de notificación propios** en vistas individuales
3. **Insertar HTML de alertas manualmente** en el DOM
4. **Usar console.log para notificar al usuario** (solo para debug)

##### **✅ OBLIGATORIO:**
1. **Usar window.showNotification** para feedback al usuario
2. **Incluir fallbacks apropiados** cuando la función no esté disponible
3. **Usar tipos estándar** (success, error, warning, info)
4. **Mensajes descriptivos y en español** para el usuario final

#### **📚 EJEMPLOS REALES DEL PROYECTO:**

##### **Editar Cotización (`editar.js`):**
```javascript
// Validación de formulario
if (!nombreInteresado) {
    window.showNotification('error', 'El nombre del interesado es obligatorio');
    $('#NombreInteresado').focus();
    return;
}

// Guardado exitoso
if (response.success) {
    window.showNotification('success', response.message || 'Cotización guardada exitosamente');
} else {
    window.showNotification('error', response.message || 'Error al guardar la cotización');
}
```

##### **Manejo de Errores AJAX:**
```javascript
$.ajax({
    // ... configuración
    success: function(response) {
        if (response.success) {
            window.showNotification('success', 'Operación exitosa');
        } else {
            window.showNotification('error', response.message);
        }
    },
    error: function(xhr) {
        let errorMessage = 'Error de comunicación con el servidor';
        
        if (xhr.status === 403) {
            errorMessage = 'No tiene permisos para realizar esta operación';
        } else if (xhr.status === 404) {
            errorMessage = 'Recurso no encontrado';
        } else if (xhr.status >= 500) {
            errorMessage = 'Error interno del servidor';
        }
        
        window.showNotification('error', errorMessage);
    }
});
```

### 🎯 **BENEFICIOS DEL SISTEMA ESTANDARIZADO:**

#### **🎨 Consistencia Visual:**
- ✅ **Tema unificado**: Todas las notificaciones siguen el diseño AdminLTE
- ✅ **Branding consistente**: Colores y tipografía del proyecto
- ✅ **Responsive**: Se adapta automáticamente a diferentes pantallas

#### **🔧 Mantenimiento Simplificado:**
- ✅ **Un solo lugar para cambios**: Modificaciones en `site.js` afectan toda la app
- ✅ **Debugging centralizado**: Fácil agregar logging o analytics
- ✅ **Testing consistente**: Un solo conjunto de tests para notificaciones

#### **👥 Experiencia de Usuario:**
- ✅ **Comportamiento predecible**: Los usuarios aprenden el patrón una vez
- ✅ **Accesibilidad**: Soporte para screen readers y navegación por teclado
- ✅ **Performance**: Reutiliza elementos DOM en lugar de crear nuevos

---

## 🏗️ **ARQUITECTURA DEL PROYECTO**

> **📚 Documentación Técnica Completa**: Ver `src/CotizacionesWeb.Infrastructure/Docs/` para lineamientos detallados

### 📖 **Documentación de Referencia**

El proyecto mantiene documentación técnica detallada en la carpeta Infrastructure:

- **🏛️ `architecture.md`**: Lineamientos de arquitectura general, estilo arquitectónico y reglas de dependencias
- **🔐 `authentication.md`**: Sistema de autenticación y autorización
- **🗃️ `migrations.md`**: Gestión de migraciones y esquema de base de datos
- **📊 `logging.md`**: Configuración de logging y diagnóstico
- **⚙️ `EFCommands.md`**: Comandos y configuración de Entity Framework

### 🎯 **Estilo Arquitectónico: Arquitectura Limpia/Hexagonal**

**📜 REGLA FUNDAMENTAL**: Seguir estrictas reglas de dependencia entre capas para mantener el diseño limpio y testeable.

#### **🏗️ Capas del Sistema:**

```
┌─────────────────────────────────────────────┐
│                     UI                      │ ← Presentación (Controllers, Views, Models)
│          CotizacionesWeb.UI                 │
└─────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────┐
│                APPLICATION                  │ ← Casos de Uso (Services, DTOs, Interfaces)
│       CotizacionesWeb.Application           │
└─────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────┐
│                  DOMAIN                     │ ← Entidades, Reglas de Negocio, Enums
│         CotizacionesWeb.Domain              │
└─────────────────────────────────────────────┘
                        ▲
                        │
┌─────────────────────────────────────────────┐
│             INFRASTRUCTURE                  │ ← Datos, Integraciones, Implementaciones
│      CotizacionesWeb.Infrastructure         │
└─────────────────────────────────────────────┘
```

#### **⚠️ REGLAS CRÍTICAS DE DEPENDENCIA:**

##### **✅ PERMITIDO:**
- **UI** → **Application** ✓
- **Application** → **Domain** ✓  
- **Infrastructure** → **Application** + **Domain** ✓

##### **❌ PROHIBIDO:**
- **Controllers** → **Infrastructure** ❌ (Violación de arquitectura limpia)
- **Domain** → **Cualquier otra capa** ❌ (Domain debe ser independiente)
- **Application** → **Infrastructure** ❌ (Usar abstracciones/interfaces)

#### **🔧 IMPLEMENTACIÓN PRÁCTICA:**

##### **✅ Patrón Correcto:**
```csharp
// En UI/Controllers
public class CotizacionesController : Controller
{
    private readonly ICotizacionService _cotizacionService; // ✓ Solo interface de Application
    
    public async Task<IActionResult> Guardar(ViewModel model)
    {
        var result = await _cotizacionService.ActualizarAsync(request); // ✓ Delegación a Application
        return Json(result);
    }
}

// En Application
public interface ICotizacionService // ✓ Abstracción en Application
{
    Task<Result> ActualizarAsync(Request request);
}

// En Infrastructure  
public class CotizacionService : ICotizacionService // ✓ Implementación en Infrastructure
{
    private readonly DbContext _context; // ✓ Acceso a datos aquí
}
```

##### **❌ Patrón Incorrecto:**
```csharp
// ❌ MAL - Controller accediendo directamente a Infrastructure
public class CotizacionesController : Controller
{
    private readonly DbContext _context; // ❌ Violación de arquitectura

    public async Task<IActionResult> Guardar()
    {
        var entity = await _context.Cotizaciones.FindAsync(id); // ❌ Lógica de datos en Controller
    }
}
```

#### **💉 INYECCIÓN DE DEPENDENCIAS:**

##### **✅ Configuración Correcta en Program.cs:**
```csharp
// Application Services (Interfaces)
builder.Services.AddScoped<ICotizacionService, CotizacionService>();

// Infrastructure Services  
builder.Services.AddDbContext<DbContextCotizaciones>(options => ...);

// Controllers solo reciben interfaces de Application
```

#### **🧪 BENEFICIOS DE ARQUITECTURA LIMPIA:**

##### **🔍 Testabilidad:**
- ✅ **Unit Tests** para Domain sin dependencias externas
- ✅ **Integration Tests** para Application usando mocks
- ✅ **Controller Tests** usando servicios simulados

##### **🔄 Flexibilidad:**
- ✅ **Cambiar BD** sin afectar lógica de negocio
- ✅ **Cambiar UI** (MVC → API) sin tocar Application/Domain
- ✅ **Integrar servicios** externos sin modificar núcleo

##### **📈 Mantenibilidad:**
- ✅ **Separación clara** de responsabilidades
- ✅ **Bajo acoplamiento** entre capas
- ✅ **Alta cohesión** dentro de cada capa

#### **📋 CHECKLIST DE VERIFICACIÓN:**

##### **🎯 Para Controllers:**
- [ ] ¿Solo inyecta interfaces de Application?
- [ ] ¿No tiene lógica de negocio compleja?
- [ ] ¿No accede directamente a DbContext?
- [ ] ¿Maneja solo coordinación y serialización?

##### **🎯 Para Services de Application:**
- [ ] ¿Implementa casos de uso específicos?
- [ ] ¿No depende de implementaciones concretas?
- [ ] ¿Usa DTOs para comunicación?
- [ ] ¿Valida reglas de negocio?

##### **🎯 Para Infrastructure:**
- [ ] ¿Implementa interfaces de Application?
- [ ] ¿Contiene toda la lógica de acceso a datos?
- [ ] ¿Maneja integraciones externas?
- [ ] ¿No es referenciada directamente por UI?

---

## 🔧 **DEBUGGING Y RESOLUCIÓN DE PROBLEMAS**

### 🚨 **PROBLEMAS COMUNES Y SOLUCIONES**

#### **❌ PROBLEMA: Estado no detectado correctamente**

**Síntomas:**
- Cotización en estado "Borrador" pero interfaz no editable
- Mensaje: "Solo las cotizaciones en estado Borrador pueden ser editadas"
- Botones deshabilitados incorrectamente

**🔍 Diagnóstico:**
```javascript
// En consola del navegador (F12)
window.diagnosticarEstado();
```

**💡 Soluciones:**

##### **Solución 1: Auto-corrección**
```javascript
// Si DOM muestra "Borrador" pero estado detectado es incorrecto
window.forzarEstadoBorrador();
```

##### **Solución 2: Corrección manual**
```javascript
// Si el estado es B pero no es editable
window.forzarEstadoEditable();
```

##### **Solución 3: Reconfiguración completa**
```javascript
// Reinicializar vista completamente
inicializarVista();
configurarEventos();
```

#### **❌ PROBLEMA: Notificaciones no se muestran**

**Síntomas:**
- Aparece `console.log` en lugar de notificaciones visuales
- Funciones usan `alert()` del navegador
- No hay feedback visual al usuario

**🔍 Diagnóstico:**
```javascript
// Verificar disponibilidad de función global
console.log('showNotification disponible:', typeof window.showNotification === 'function');

// Probar notificación directa
window.showNotification('info', 'Prueba de notificación');
```

**💡 Soluciones:**

##### **Verificar carga de site.js:**
```javascript
// En _Layout.cshtml, verificar que existe esta línea:
<script src="~/js/site.js" asp-append-version="true"></script>

// Y que se carga ANTES de scripts específicos de vista
```

##### **Verificar implementación en vista:**
```javascript
// ✅ CORRECTO
function showNotification(type, message) {
    if (typeof window.showNotification === 'function' && 
        window.showNotification !== showNotification) {
        window.showNotification(type, message);
    } else {
        console.log(`[${type}] ${message}`);
    }
}

// ❌ INCORRECTO
function showNotification(type, message) {
    alert(message); // ¡No!
}
```

#### **❌ PROBLEMA: Errores de JavaScript en consola**

**Síntomas:**
- `TypeError: window.showNotification is not a function`
- `ReferenceError: estadoActual is not defined`
- Funcionalidad de edición no responde

**🔍 Diagnóstico:**
```javascript
// Verificar orden de carga de scripts
console.log('jQuery disponible:', typeof $ !== 'undefined');
console.log('AdminLTE disponible:', typeof $.fn.CardWidget !== 'undefined');
console.log('FormatConfig disponible:', typeof window.FormatConfig !== 'undefined');
console.log('showNotification disponible:', typeof window.showNotification === 'function');
```

**💡 Soluciones:**

##### **Verificar orden de scripts en _Layout.cshtml:**
```razor
<!-- ✅ ORDEN CORRECTO -->
<!-- jQuery PRIMERO -->
<script src="~/lib/jquery/dist/jquery.min.js"></script>
<!-- Bootstrap -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/js/bootstrap.bundle.min.js"></script>
<!-- AdminLTE -->
<script src="https://cdn.jsdelivr.net/npm/admin-lte@3.2/dist/js/adminlte.min.js"></script>
<!-- Format Config ANTES de otros scripts -->
<script src="~/js/shared/format-config.js" asp-append-version="true"></script>
<!-- Site.js con funciones globales -->
<script src="~/js/site.js" asp-append-version="true"></script>
<!-- Scripts específicos de vista AL FINAL -->
@await RenderSectionAsync("Scripts", required: false)
```

### 🛠️ **HERRAMIENTAS DE DEBUGGING DISPONIBLES**

#### **🔍 Funciones de Diagnóstico Global:**

##### **`window.diagnosticarEstado()`**
```javascript
// Diagnóstico completo del estado de la cotización
window.diagnosticarEstado();

/* Salida esperada:
📊 Variables Globales:
  - estadoActual: B
  - monedaActual: CRC
✅ Verificaciones de Estado:
  - Es editable (directo): true
  - Badge texto: "Borrador"
*/
```

##### **`window.diagnosticarSimbolos()`**
```javascript
// Diagnóstico de símbolos de moneda
window.diagnosticarSimbolos();

/* Salida esperada:
💰 DIAGNÓSTICO DE SÍMBOLOS DE MONEDA
🧪 Pruebas de formateo:
  FormatUtils.formatCurrency(125000, 'CRC'): ¢125,000.00
*/
```

##### **`window.diagnosticarContadorNotas()`**
```javascript
// Diagnóstico del contador de caracteres en notas
window.diagnosticarContadorNotas();
```

#### **🔧 Funciones de Corrección Automática:**

##### **`window.forzarEstadoBorrador()`**
```javascript
// Auto-corrección cuando DOM indica Borrador pero estado no detectado
window.forzarEstadoBorrador();

/* Hace automáticamente:
✅ Lee badge del DOM
✅ Fuerza estadoActual = 'B'
✅ Reconfigura eventos
✅ Habilita interfaz
*/
```

##### **`window.corregirSimbolos()`**
```javascript
// Corrige símbolos de moneda malformados en el DOM
window.corregirSimbolos();
```

### 📝 **CHECKLIST DE DEBUGGING**

#### **🎯 Para Problemas de Estado:**
- [ ] ¿El badge del DOM dice "Borrador"?
- [ ] ¿`estadoActual === 'B'`?
- [ ] ¿`window.FormatConfig.estado === 'B'`?
- [ ] ¿Los botones están habilitados?
- [ ] ¿Los campos NO tienen `readonly`?

#### **🎯 Para Problemas de Notificaciones:**
- [ ] ¿Existe `window.showNotification`?
- [ ] ¿Se carga `site.js` antes del script de vista?
- [ ] ¿No hay conflictos de nombres de función?
- [ ] ¿Las notificaciones aparecen en pantalla?

#### **🎯 Para Problemas de JavaScript:**
- [ ] ¿jQuery está cargado?
- [ ] ¿AdminLTE está disponible?
- [ ] ¿No hay errores en consola?
- [ ] ¿El orden de scripts es correcto?

### 🚀 **COMANDOS DE EMERGENCIA**

#### **Reinicialización Completa:**
```javascript
// En consola, ejecutar paso a paso:
window.diagnosticarEstado();                // 1. Diagnosticar problema
window.forzarEstadoBorrador();             // 2. Corregir estado si necesario
window.corregirSimbolos();                 // 3. Corregir símbolos si necesario
location.reload();                         // 4. Recargar página como último recurso
```

#### **Debugging de Producción:**
```javascript
// Información básica para reportes de error
console.log('=== INFO DEBUG ===');
console.log('URL:', window.location.href);
console.log('User Agent:', navigator.userAgent);
console.log('jQuery:', typeof $ !== 'undefined' ? $.fn.jquery : 'NO DISPONIBLE');
console.log('Estado actual:', estadoActual);
console.log('Moneda actual:', monedaActual);
console.log('FormatConfig:', !!window.FormatConfig);
console.log('showNotification:', typeof window.showNotification === 'function');
console.log('==================');
```

---

## 📋 **CAMBIOS ESTRUCTURALES CRÍTICOS (22/03/2026 - SESIÓN 3)**

### 🔄 **MIGRACIÓN DE MONEDA: CotizacionVersion → Cotizacion**

Se realizó un cambio estructural importante en la base de datos para optimizar la gestión de monedas:

#### **🎯 Justificación del Cambio:**
```
PROBLEMA: La moneda estaba almacenada en CotizacionVersion, lo que permitía
          cambios de moneda entre versiones de una misma cotización.
          
SOLUCIÓN: Mover la moneda a la tabla Cotizacion para que sea consistente
          en todas las versiones de una cotización.
          
REGLA DE NEGOCIO: Si se requiere cambio de moneda, se debe duplicar 
                  la cotización o crear una nueva.
```

#### **📊 Cambios en el Esquema:**

##### **Tabla Cotizacion (AGREGADO):**
```sql
ALTER TABLE Cotizacion 
ADD Moneda NVARCHAR(10) NOT NULL DEFAULT 'CRC'
```

##### **Tabla CotizacionVersion (REMOVIDO/MANTENIDO):**
```sql
-- REMOVIDO: Moneda (movida a Cotizacion)
ALTER TABLE CotizacionVersion 
DROP COLUMN Moneda

-- MANTENIDO: TipoCambio (puede cambiar en el tiempo)
-- TipoCambio DECIMAL(18,6) NULL -- Se mantiene aquí
```

#### **🔧 Cambios en el Código:**

##### **Entidades Actualizadas:**
```csharp
// Cotizacion.cs
public class Cotizacion : BaseEntity
{
    // ... campos existentes ...
    public string Moneda { get; set; } = "CRC"; // ← AGREGADO
    // NOTA: TipoCambio NO se agrega aquí (se mantiene en CotizacionVersion)
}

// CotizacionVersion.cs  
public class CotizacionVersion : BaseEntity
{
    // ... campos existentes ...
    // REMOVIDO: public string Moneda { get; set; }
    public decimal? TipoCambio { get; set; } // ← SE MANTIENE (puede cambiar en versiones)
}
```

##### **Servicios Actualizados:**
```csharp
// CotizacionService - GetCotizacionesListAsync()
return new CotizacionListDto(
    // ... otros campos ...
    c.Moneda, // ← Ahora desde Cotizacion en lugar de Version
    // ... resto de campos ...
);

// CotizacionService - Al copiar versiones
var nuevaVersion = new CotizacionVersion
{
    // ... campos de versión ...
    TipoCambio = versionVigente.TipoCambio, // ← Se mantiene y copia desde version anterior
    // Moneda ya NO se copia (está en Cotizacion y es inmutable)
};
```

#### **⚠️ Consideraciones Importantes:**

1. **🔒 Moneda Inmutable**: Una vez establecida la moneda de una cotización, NO puede cambiar entre versiones
2. **💱 TipoCambio Variable**: El tipo de cambio SÍ puede variar entre versiones (fluctuaciones del mercado)
3. **🔄 Migración de Datos**: Los datos existentes mantienen su moneda original
4. **📋 Compatibilidad**: Los DTOs mantienen compatibilidad temporal marcando campos como obsoletos

#### **🛠️ Estado de Implementación:**
- ✅ **Entidades**: Actualizadas
- ✅ **Configuraciones EF**: Actualizadas  
- ✅ **Servicios**: Actualizados
- ✅ **DTOs**: Actualizados con compatibilidad
- ✅ **Migraciones**: Creadas (aplicadas en servidor Azure)
- ✅ **ViewModels**: Compatible (ya usaban moneda desde nivel superior)
- ⚠️ **Base de Datos Local**: Inconsistencias en migraciones (servidor Azure correcto)

#### **📈 Beneficios del Cambio:**
- **🎯 Consistencia**: Moneda uniforme en todas las versiones de una cotización
- **🔒 Integridad**: Evita confusiones por cambios de moneda accidentales
- **⚡ Performance**: Menos JOINs para obtener la moneda (está en tabla principal)
- **📊 Reporting**: Reportes más simples (moneda a nivel de cotización)
- **🔄 Lógica Simplificada**: Reglas de negocio más claras

---

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

**Versión**: 4.2
**Última actualización**: 17 de marzo de 2026  
**Estado**: ✅ Sistema completo con vista de detalle, edición controlada por estados y configuración centralizada de formateo
**Cambios recientes**: Vista de edición, reglas de estados estrictas, configuración JavaScript centralizada
**Autor**: Marcela Jiménez (con GitHub Copilot)

---

## 📋 **CAMBIOS CRÍTICOS IMPLEMENTADOS (17/03/2026 - SESIÓN 2)**

### 🔒 **1. SISTEMA DE EDICIÓN RESTRINGIDO POR ESTADOS**

Se implementó un sistema estricto donde **SOLO las cotizaciones en estado BORRADOR (B) pueden ser editadas**:

#### **🚨 Regla de Negocio Crítica:**
```
REGLA: Solo cotizaciones en estado "Borrador" (B) son editables.
RAZÓN: Los demás estados representan flujos en proceso o estados terminales.
```

#### **Comportamiento por Estado:**
| Estado | Texto | Editable | Comportamiento |
|--------|-------|----------|----------------|
| **B** | Borrador | ✅ **SÍ** | **Completamente editable** |
| **P** | Pendiente Aprobación | ❌ **NO** | Solo lectura + aviso |
| **A** | Aprobada | ❌ **NO** | Solo lectura + aviso |
| **E** | Enviada | ❌ **NO** | Solo lectura + aviso |
| **T** | Aceptada | ❌ **NO** | Solo lectura + aviso |
| **R** | Rechazada | ❌ **NO** | Solo lectura + aviso |
| **X** | Archivada | ❌ **NO** | Solo lectura + aviso |

#### **Implementación Multi-Capa:**

##### **1. Vista Index - Botones Condicionales:**
```razor
<!-- Editar - SOLO para estado Borrador -->
@if (cotizacion.EstadoActual == 'B')
{
    <button class="btn-action btn-action-warning btn-editar" 
            requiere-permiso="COT_EDIT">
        <i class="fas fa-edit"></i>
    </button>
}

<!-- Ver Detalle - Para todos los estados -->
<button class="btn-action btn-action-edit btn-detalle" 
        requiere-permiso="COT_VIEW_DETAIL">
    <i class="fas fa-eye"></i>
</button>
```

##### **2. Controller - Validación Estricta:**
```csharp
[HttpGet("Cotizaciones/Editar/{cotizacionId}")]
public async Task<IActionResult> Editar(string cotizacionId)
{
    // VALIDACIÓN CRÍTICA: Solo se puede editar en estado Borrador
    if (cotizacion.EstadoActual != 'B')
    {
        TempData["Error"] = "Solo las cotizaciones en estado Borrador pueden ser editadas.";
        return RedirectToAction(nameof(Index));
    }
    // ... resto del código
}
```

##### **3. Vista Editar - Campos Dinámicos:**
```razor
<!-- Header con estado dinámico -->
<h1>
    Editar Cotización @Model.CotizacionId
    <span class="badge badge-secondary-custom">@Model.EstadoActualTexto</span>
</h1>

<!-- Alerta si no es editable -->
@if (Model.EstadoActual != 'B')
{
    <div class="alert alert-warning">
        <strong>Atención:</strong> Esta cotización está en estado @Model.EstadoActualTexto 
        y no puede ser editada.
    </div>
}

<!-- Campos deshabilitados si no es Borrador -->
<input type="text" @(Model.EstadoActual != 'B' ? "readonly" : "") />
<button @(Model.EstadoActual == 'B' ? "" : "disabled")>Guardar</button>
```

##### **4. JavaScript - Protección del Cliente:**
```javascript
const esEditable = FormatUtils.isEditable(estadoActual);

if (esEditable) {
    // Configurar todos los eventos de edición
} else {
    // Mostrar avisos si intenta editar
    $('.form-control').on('click', function(e) {
        showNotification('warning', 'Esta cotización no puede ser editada.');
        e.preventDefault();
    });
}
```

### 🧭 **2. CONFIGURACIÓN CENTRALIZADA DE ESTADOS Y FORMATEO**

Se creó un archivo JavaScript centralizado (`format-config.js`) para evitar duplicación de lógica:

#### **Archivo: `~/js/shared/format-config.js`**
```javascript
window.FormatConfig = {
    // Configuración de monedas (sincronizada con FormatHelper.cs)
    currencies: {
        'CRC': '₡',     // Colón costarricense
        'USD': '$',     // Dólar estadounidense 
        'EUR': '€',     // Euro
        'GBP': '£',     // Libra esterlina
        'JPY': '¥',     // Yen japonés
        // ... mismo diccionario que C#
    },
    
    // Estados de cotización con reglas de negocio
    estados: {
        'B': { texto: 'Borrador', editable: true, class: 'badge-secondary-custom' },
        'P': { texto: 'Pendiente Aprobación', editable: false, class: 'badge-warning-custom' },
        'A': { texto: 'Aprobada', editable: false, class: 'badge-success-custom' },
        'E': { texto: 'Enviada', editable: false, class: 'badge-info-custom' },
        'T': { texto: 'Aceptada', editable: false, class: 'badge-primary-custom' },
        'R': { texto: 'Rechazada', editable: false, class: 'badge-danger-custom' },
        'X': { texto: 'Archivada', editable: false, class: 'badge-dark-custom' }
    },
    
    // Reglas de transiciones válidas
    transicionesPermitidas: {
        'B': ['P', 'X'], // Borrador → Pendiente, Archivada
        'P': ['A', 'B'], // Pendiente → Aprobada, Borrador  
        'A': ['E'],      // Aprobada → Enviada
        'E': ['T', 'R'], // Enviada → Aceptada, Rechazada
        'T': ['X'],      // Aceptada → Archivada
        'R': ['X'],      // Rechazada → Archivada
        'X': []          // Archivada → Sin transiciones
    }
};

window.FormatUtils = {
    formatCurrency: function(value, currency) { /* ... */ },
    getCurrencySymbol: function(currency) { /* ... */ },
    isEditable: function(estado) { return FormatConfig.estados[estado]?.editable || false; },
    getEstadoTexto: function(estado) { /* ... */ },
    isTransicionPermitida: function(estadoActual, estadoDestino) { /* ... */ }
};
```

#### **Integración en Layout:**
```html
<!-- Cargar ANTES que otros scripts -->
<script src="~/js/shared/format-config.js" asp-append-version="true"></script>
```

#### **Uso en Scripts Específicos:**
```javascript
// En editar.js
const esEditable = FormatUtils.isEditable(estadoActual); // Usa configuración central
const montoFormateado = FormatUtils.formatCurrency(1250.50, 'USD'); // $1,250.50
```

### 🎨 **3. VISTA DE EDICIÓN COMPLETA CON MODAL AVANZADO**

Se implementó una vista de edición completa con las siguientes características:

#### **Funcionalidades Implementadas:**
- ✅ **Modal con header azul** (matching con otras agrupaciones)
- ✅ **Edición de información del interesado** con búsqueda Select2
- ✅ **Edición de notas** con contador de caracteres dinámico
- ✅ **Modal para editar/agregar líneas de detalle**
- ✅ **Cálculos automáticos** de totales en tiempo real
- ✅ **Recálculo de resumen financiero** al cambiar líneas

#### **Modal de Edición de Líneas:**
```
┌─────────────────────────────────────────────────┐
│ [🔵 Header Azul] Editar Detalle            [X] │
├─────────────────────────────────────────────────┤
│ Producto: [PROD001 - Laptop Dell] [Descripción]│
│ Cantidad: [25.00]                              │  
│ Precio:   [₡5000.00]  Descuento: [₡0.00]      │
│                                                │
│ Total de Línea: ₡125,000.00                   │
│                     [Cancelar] [Guardar]       │
└─────────────────────────────────────────────────┘
```

#### **Características Técnicas:**
- **🔵 Header azul**: `bg-primary text-white` matching con otras agrupaciones
- **💰 Formato correcto**: `₡125,000.00` usando FormatHelper consistente
- **📝 Producto pre-seleccionado**: Al editar, muestra el código actual
- **🔢 Cálculo en tiempo real**: Se actualiza al cambiar cantidad/precio/descuento
- **✅ Validación**: Campos obligatorios y valores mínimos

#### **Datos Temporales para Desarrollo:**
```javascript
// Productos temporales (simulando ERP)
const PRODUCTOS_TEMP = [
    { id: 'PROD001', nombre: 'Laptop Dell Inspiron 15', precio: 450000 },
    { id: 'PROD002', nombre: 'Monitor Samsung 24"', precio: 125000 },
    // ... más productos
];

// Interesados temporales (simulando HubSpot)
const INTERESADOS_TEMP = [
    { id: 1, nombre: 'Juan Carlos Rodríguez', email: 'juan@email.com', empresa: 'Tech S.A.' },
    // ... más interesados
];
```

### 🔗 **4. SINCRONIZACIÓN SERVIDOR-CLIENTE MEJORADA**

Se mejoró la forma en que el servidor pasa configuración al cliente:

#### **En la Vista Razor:**
```razor
@section Scripts {
    <!-- Exponer configuración del FormatHelper a JavaScript -->
    <script>
        window.FormatConfig = {
            moneda: '@Model.Moneda',
            simboloMoneda: '@FormatHelper.GetCurrencySymbol(Model.Moneda)',
            estado: '@Model.EstadoActual'
        };
    </script>
    <script src="~/js/cotizaciones/editar.js"></script>
}
```

#### **En el JavaScript:**
```javascript
function inicializarVista() {
    // Usar configuración del servidor si está disponible
    if (window.FormatConfig) {
        monedaActual = window.FormatConfig.moneda;
        estadoActual = window.FormatConfig.estado;
    } else {
        // Fallback al método de detección por DOM
        monedaActual = detectarMonedaDelFormulario();
        estadoActual = detectarEstadoDelBadge();
    }
    
    // Continuar inicialización...
}
```

### 🎯 **5. REGLAS DE NEGOCIO DOCUMENTADAS EN CÓDIGO**

Todas las reglas de negocio están ahora centralizadas y documentadas:

#### **Estados Editables (JavaScript):**
```javascript
window.FormatConfig = {
    estados: {
        'B': { editable: true },   // ÚNICO estado editable
        'P': { editable: false },  // En proceso de aprobación
        'A': { editable: false },  // Ya aprobada
        'E': { editable: false },  // Enviada al cliente
        'T': { editable: false },  // Aceptada por cliente
        'R': { editable: false },  // Rechazada por cliente
        'X': { editable: false }   // Archivada (terminal)
    }
};
```

#### **Transiciones Válidas (JavaScript):**
```javascript
transicionesPermitidas: {
    'B': ['P', 'X'], // Borrador puede ir a Pendiente o Archivada
    'P': ['A', 'B'], // Pendiente puede ir a Aprobada o volver a Borrador
    'A': ['E'],      // Aprobada solo puede ir a Enviada
    'E': ['T', 'R'], // Enviada puede ser Aceptada o Rechazada
    'T': ['X'],      // Aceptada solo puede archivarse
    'R': ['X'],      // Rechazada solo puede archivarse  
    'X': []          // Archivada es estado terminal
}
```

### 📋 **6. CSS ESPECÍFICO PARA ESTADOS NO EDITABLES**

Se agregaron estilos específicos para campos de solo lectura:

```css
/* Campos de solo lectura (no editables) */
.form-control[readonly],
.form-control[disabled],
.readonly-field {
    background-color: #f8f9fa !important;
    border-color: #dee2e6 !important;
    color: #6c757d !important;
    cursor: not-allowed;
}

/* Select deshabilitado */
select.form-control[disabled] {
    background-color: #f8f9fa !important;
    color: #6c757d !important;
}

/* Modal con header azul */
#modalEditarDetalle .modal-header.bg-primary {
    background-color: #007bff !important;
    border-bottom: 1px solid #0056b3;
}

/* Advertencia para estados no editables */
.alert-warning {
    border-left: 4px solid #ffc107;
}
```

### 🚀 **7. FLUJO DE USUARIO FINAL**

#### **Escenario 1: Cotización Editable (Borrador)**
```
Usuario ve tabla → Botón "Editar" visible → Clic editar → Vista completamente funcional
→ Puede modificar todo → Guardado exitoso
```

#### **Escenario 2: Cotización No Editable (Cualquier otro estado)**
```
Usuario ve tabla → Solo botón "Ver Detalle" → Si accede a /Editar/ directamente 
→ Redirección + error → Si de alguna manera accede → Vista solo lectura + avisos
```

#### **Protecciones Implementadas:**
- 🔒 **UI Level**: Botones ocultos/deshabilitados según estado
- 🔒 **Controller Level**: Validación y redirección automática  
- 🔒 **Frontend Level**: JavaScript preventivo con avisos
- 🔒 **Visual Level**: Estilos que indican campos de solo lectura

### ⚠️ **REGLAS CRÍTICAS A SEGUIR:**

1. **🚨 NUNCA permitir edición** en estados diferentes a 'B' (Borrador)
2. **📋 SIEMPRE usar FormatConfig.js** en lugar de duplicar lógica de estados
3. **💰 USAR FormatHelper de C#** como fuente de verdad para formateo
4. **🔄 SINCRONIZAR** cambios en estados/formatos entre C# y JavaScript
5. **⚡ CARGAR format-config.js ANTES** que scripts específicos de página

### 🔮 **Próximos Pasos Implementación:**

1. **💾 Implementar guardado real** en lugar de simulación
2. **🔗 Conectar con servicios reales** de HubSpot y ERP  
3. **📱 Optimización móvil** adicional para la vista de edición
4. **⚡ Validaciones del lado del servidor** para los formularios
5. **🧪 Tests unitarios** para las reglas de estado

---

## 🆕 **ADENDUM: CONFIGURACIÓN CENTRALIZADA Y FORMATEO (17/03/2026)**

### 📁 **Nuevos Archivos Críticos Agregados:**

#### **1. JavaScript Centralizado:**
- **📄 `~/js/shared/format-config.js`** - Configuración global de estados, monedas y reglas de negocio
- **🎯 Propósito**: Evitar duplicación de lógica entre módulos de JavaScript
- **⚡ Carga**: OBLIGATORIO cargar en `_Layout.cshtml` ANTES que otros scripts
- **🔄 Sincronización**: Mantiene consistencia con `FormatHelper.cs`

#### **2. Archivos de Edición:**
- **📄 `~/Views/Cotizaciones/Editar.cshtml`** - Vista completa de edición
- **🎨 `~/css/cotizaciones/editar.css`** - Estilos específicos para edición
- **🖱️ `~/js/cotizaciones/editar.js`** - Lógica de edición y modal de líneas

### ⚡ **Funciones JavaScript Globales Disponibles:**

```javascript
// Formateo (consistente con C# FormatHelper)
FormatUtils.formatCurrency(1250.50, 'USD')     → "$1,250.50"
FormatUtils.getCurrencySymbol('EUR')           → "€"

// Estados y validaciones
FormatUtils.isEditable('B')                    → true
FormatUtils.isEditable('P')                    → false
FormatUtils.getEstadoTexto('A')               → "Aprobada"
FormatUtils.isTransicionPermitida('B', 'P')   → true
```

### 🔒 **Reglas de Negocio Centralizadas:**

#### **Estados Editables:**
```javascript
const ESTADOS_EDITABLES = ['B']; // SOLO Borrador
const ESTADOS_COPIABLES = ['A', 'R']; // Solo Aprobada y Rechazada
const ESTADOS_ERP = ['T']; // Solo Aceptada puede ir al ERP
```

#### **Monedas Soportadas (Sincronizado con C#):**
```javascript
const MONEDAS = {
    'CRC': '₡', 'USD': '$', 'EUR': '€', 'GBP': '£', 
    'JPY': '¥', 'MXN': '$', 'CAD': '$', 'CNY': '¥'
};
```

### 🎯 **Lineamientos de Desarrollo:**

#### **✅ HACER:**
1. **Usar `FormatUtils.js`** para cualquier lógica de estados/formateo
2. **Cargar `format-config.js`** PRIMERO en cualquier página que use estados
3. **Consultar `FormatConfig.estados[codigo].editable`** antes de habilitar edición
4. **Usar `FormatHelper.cs`** como fuente de verdad para formateo del servidor

#### **❌ NO HACER:**
1. **Duplicar lógica** de estados en scripts individuales  
2. **Hardcodear** símbolos de moneda o reglas de estado
3. **Asumir** que cualquier estado diferente a 'B' es editable
4. **Crear** funciones de formateo personalizadas (usar centralizadas)

### 🔧 **Configuración de Servidor a Cliente:**

```razor
@section Scripts {
    <!-- Exponer configuración específica de la vista -->
    <script>
        window.FormatConfig = {
            moneda: '@Model.Moneda',
            simboloMoneda: '@FormatHelper.GetCurrencySymbol(Model.Moneda)',
            estado: '@Model.EstadoActual'
        };
    </script>
    <!-- Scripts que usan la configuración -->
    <script src="~/js/cotizaciones/editar.js"></script>
}
```

### 📋 **Checklist para Futuras Implementaciones:**

- [ ] ¿Está usando `FormatUtils` en lugar de lógica duplicada?
- [ ] ¿Está validando estado editable antes de mostrar campos de edición?
- [ ] ¿Está usando `FormatHelper.cs` para formateo del servidor?
- [ ] ¿Está cargando `format-config.js` antes que scripts específicos?
- [ ] ¿Las reglas de transición están centralizadas?

---

**💡 Recuerda**: Esta documentación debe ser la **primera referencia** para cualquier nueva funcionalidad relacionada con cotizaciones, estados o formateo. **Mantén la consistencia** con los patrones establecidos.

---

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
## 🛠️ **CONFIGURACIÓN DE PARÁMETROS DEL SISTEMA (AGREGADO)** 
Se implementó un sistema completo de parámetros configurables para el manejo centralizado de configuraciones
