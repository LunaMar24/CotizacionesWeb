# ?? SISTEMA DE MODALES DE CONFIRMACIÓN

**Fecha de Implementación**: 17 de marzo de 2026  
**Versión**: 1.0

---

## ?? **DESCRIPCIÓN GENERAL**

El proyecto implementa un sistema centralizado de modales de confirmación usando Bootstrap 4.6, eliminando el uso de `alert()`, `confirm()` y `prompt()` nativos del navegador para mantener consistencia visual y mejor UX.

---

## ??? **ARQUITECTURA DEL SISTEMA**

### **Componentes Principales:**

#### **1. Modal HTML en Layout** (`_Layout.cshtml`)
```html
<!-- Modal de Confirmación Genérico -->
<div class="modal fade" id="modalConfirmacion" tabindex="-1" role="dialog">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header" id="modalConfirmacionHeader">
        <h5 class="modal-title" id="modalConfirmacionTitulo">
          <i class="fas fa-question-circle"></i> Confirmar Acción
        </h5>
        <button type="button" class="close" data-dismiss="modal">
          <span>&times;</span>
        </button>
      </div>
      <div class="modal-body" id="modalConfirmacionMensaje">
        <!-- Mensaje dinámico -->
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-dismiss="modal">
          <i class="fas fa-times"></i> Cancelar
        </button>
        <button type="button" class="btn btn-primary" id="btnConfirmarAccion">
          <i class="fas fa-check"></i> Aceptar
        </button>
      </div>
    </div>
  </div>
</div>
```

#### **2. Función Global** (`modals.js`)
```javascript
window.mostrarModalConfirmacionGlobal(titulo, mensaje, tipo, onConfirm, onCancel, opciones)
```

**Ubicación**: `~/js/modals.js`  
**Cargado en**: `_Layout.cshtml` (disponible globalmente)

---

## ?? **TIPOS DE MODALES SOPORTADOS**

### **Configuraciones Visuales:**

| Tipo | Header | Icono | Botón | Uso |
|------|--------|-------|-------|-----|
| `info` | ?? Azul | `fa-info-circle` | Azul | Información general |
| `warning` | ?? Amarillo | `fa-exclamation-triangle` | Amarillo | Advertencias |
| `danger` | ?? Rojo | `fa-exclamation-circle` | Rojo | Acciones destructivas |
| `success` | ?? Verde | `fa-check-circle` | Verde | Confirmaciones positivas |
| `exit` | ?? Amarillo | `fa-sign-out-alt` | Amarillo | Salir sin guardar |

---

## ?? **USO DE LA FUNCIÓN GLOBAL**

### **Sintaxis:**
```javascript
window.mostrarModalConfirmacionGlobal(
    titulo,        // string: Título del modal
    mensaje,       // string: Mensaje HTML permitido
    tipo,          // string: 'info', 'warning', 'danger', 'success', 'exit'
    onConfirm,     // function: Callback si confirma
    onCancel,      // function: Callback si cancela (opcional)
    opciones       // object: Opciones adicionales (opcional)
);
```

### **Opciones Adicionales:**
```javascript
{
    btnTextoConfirmar: 'Texto Personalizado',  // Texto del botón de confirmar
    btnTextoCancelar: 'Texto Personalizado'    // Texto del botón de cancelar
}
```

---

## ?? **EJEMPLOS DE USO**

### **1. Confirmación de Eliminación:**
```javascript
window.mostrarModalConfirmacionGlobal(
    'Eliminar Detalle',
    '¿Está seguro de que desea eliminar esta línea de detalle?<br><br>' +
    '<small class="text-muted">Esta acción no se puede deshacer.</small>',
    'danger',
    function() {
        // Usuario confirmó: ejecutar eliminación
        ejecutarEliminacion(id);
    },
    null, // No necesita callback de cancelación
    {
        btnTextoConfirmar: 'Eliminar',
        btnTextoCancelar: 'Cancelar'
    }
);
```

### **2. Salir Sin Guardar:**
```javascript
window.mostrarModalConfirmacionGlobal(
    'Cambios Sin Guardar',
    '¿Está seguro de que desea salir sin guardar los cambios?<br><br>' +
    '<strong class="text-danger">Se perderán todos los cambios realizados.</strong>',
    'exit',
    function() {
        // Usuario confirmó: permitir salida
        window.location.href = '/ListaInicial';
    },
    function() {
        // Usuario canceló: quedarse en la página
        console.log('Usuario decidió quedarse');
    },
    {
        btnTextoConfirmar: 'Salir Sin Guardar',
        btnTextoCancelar: 'Quedarme Aquí'
    }
);
```

### **3. Advertencia Informativa:**
```javascript
window.mostrarModalConfirmacionGlobal(
    'Cotización Sin Productos',
    '¿Está seguro de que desea guardar la cotización sin líneas de productos?<br><br>' +
    '<small class="text-muted">La cotización se guardará como borrador.</small>',
    'warning',
    function() {
        // Usuario confirmó: continuar guardado
        guardarCotizacion();
    },
    function() {
        // Usuario canceló: restaurar interfaz
        restaurarBotonGuardar();
    },
    {
        btnTextoConfirmar: 'Guardar Sin Productos',
        btnTextoCancelar: 'Cancelar'
    }
);
```

### **4. Cambio de Moneda:**
```javascript
window.mostrarModalConfirmacionGlobal(
    'Confirmar Cambio de Moneda',
    `¿Está seguro de que desea cambiar la moneda de <strong>${monedaAnterior}</strong> a <strong>${monedaNueva}</strong>?`,
    'warning',
    function() {
        // Usuario confirmó: aplicar cambio
        aplicarCambioMoneda(monedaNueva);
    },
    function() {
        // Usuario canceló: revertir combo
        revertirSeleccionMoneda(monedaAnterior);
    },
    {
        btnTextoConfirmar: 'Cambiar Moneda',
        btnTextoCancelar: 'Cancelar'
    }
);
```

---

## ?? **LIMITACIONES Y CONSIDERACIONES**

### **?? beforeunload - Limitación del Navegador:**

**PROBLEMA**: Los navegadores modernos **NO PERMITEN** usar modales personalizados en el evento `beforeunload`.

**ALCANCE**: Este evento se dispara para:
- ? Cerrar pestaña del navegador
- ? Cerrar ventana del navegador
- ? Refrescar página (F5)
- ? Navegar con botones Atrás/Adelante

**SOLUCIÓN IMPLEMENTADA**:
```javascript
// ?? INEVITABLE: beforeunload DEBE usar mensaje nativo
$(window).on('beforeunload', function(e) {
    if (verificarCambiosSinGuardar()) {
        // Mensaje genérico (navegadores muestran su propio texto)
        const mensaje = 'Tiene cambios sin guardar que se perderán';
        e.returnValue = mensaje;
        return mensaje;
    }
});

// ? MODAL BOOTSTRAP: Para navegación interna (links, botones)
$(document).on('click', 'a[href]', function(e) {
    if (verificarCambiosSinGuardar()) {
        e.preventDefault();
        const urlDestino = $(this).attr('href');
        
        window.mostrarModalConfirmacionGlobal(
            'Cambios Sin Guardar',
            '¿Está seguro de que desea salir sin guardar?',
            'exit',
            function() {
                window.location.href = urlDestino;
            }
        );
    }
});
```

**RESULTADO**:
- ? **Navegación interna**: Modal Bootstrap (mejor UX)
- ?? **Cerrar tab/ventana**: Mensaje nativo del navegador (inevitable)

---

## ?? **IMPLEMENTACIÓN EN ARCHIVOS DEL PROYECTO**

### **Archivos Actualizados:**

#### **1. `modals.js` (Función Global)**
- ? `window.mostrarModalConfirmacionGlobal()` - Función principal
- ? `obtenerConfiguracionModalGlobal()` - Configuración visual
- ? `usarConfirmacionNativaGlobal()` - Fallback seguro

#### **2. `cotizaciones/editar.js`**
- ? `mostrarModalConfirmacion()` - Wrapper local que usa la global
- ? `confirmarSalidaConCambios()` - Modal para salir sin guardar
- ? `eliminarDetalle()` - Modal para confirmar eliminación
- ? `mostrarModalConfirmacionMoneda()` - Modal para cambio de moneda
- ? `ejecutarGuardadoCotizacion()` - Modal para validaciones

#### **3. `configuracion/parametros.js`**
- ? `mostrarNotificacion()` - Actualizado para usar `window.showNotification`
- ? Interceptación de navegación en pestañas con modal

---

## ? **BENEFICIOS DE LA IMPLEMENTACIÓN**

### **?? Consistencia Visual:**
- ? Todos los modales siguen el diseño AdminLTE
- ? Colores y estilos estandarizados por tipo
- ? Iconografía consistente (Font Awesome)

### **?? Mantenibilidad:**
- ? **Función centralizada**: Un solo lugar para cambios
- ? **Principio DRY**: No hay duplicación de código
- ? **Fallbacks seguros**: Degradación elegante si algo falla

### **?? Experiencia de Usuario:**
- ? **Modales no bloqueantes**: Mejor que `confirm()` nativo
- ? **Feedback visual claro**: Colores según severidad
- ? **Accesibilidad**: Soporte para teclado (ESC para cancelar)
- ? **Responsive**: Funciona en desktop y móvil

### **?? Extensibilidad:**
- ? **Fácil agregar nuevos tipos**: Solo extender configuración
- ? **Textos personalizables**: Botones con texto específico
- ? **Callbacks flexibles**: onConfirm y onCancel opcionales

---

## ?? **ANTI-PATRONES A EVITAR**

### **? NUNCA HACER:**

#### **1. Usar alert/confirm/prompt nativos:**
```javascript
// ? MAL
if (confirm('¿Está seguro?')) {
    eliminar();
}

// ? BIEN
window.mostrarModalConfirmacionGlobal(
    'Confirmar Eliminación',
    '¿Está seguro de que desea eliminar?',
    'danger',
    function() { eliminar(); }
);
```

#### **2. Duplicar implementación de modales:**
```javascript
// ? MAL - Implementar modal propio en cada archivo
function miModalConfirmacion() {
    const modal = $('#miModal');
    // ... implementación duplicada
}

// ? BIEN - Usar función global
window.mostrarModalConfirmacionGlobal(...);
```

#### **3. Intentar personalizar beforeunload con modales:**
```javascript
// ? MAL - No funciona en navegadores modernos
$(window).on('beforeunload', function(e) {
    $('#modalConfirmacion').modal('show'); // ¡NO FUNCIONA!
    e.preventDefault();
});

// ? BIEN - Usar mensaje nativo para beforeunload, modal para navegación interna
$(window).on('beforeunload', function(e) {
    if (hayCambios()) {
        e.returnValue = 'Mensaje nativo';
        return 'Mensaje nativo';
    }
});

// Para navegación interna (links):
$(document).on('click', 'a', function(e) {
    if (hayCambios()) {
        e.preventDefault();
        mostrarModalConfirmacionGlobal(...); // ? ESTO SÍ FUNCIONA
    }
});
```

---

## ?? **CHECKLIST DE IMPLEMENTACIÓN**

### **Para Nuevas Funcionalidades:**
- [ ] ¿Necesitas confirmación del usuario? ? Usar `mostrarModalConfirmacionGlobal`
- [ ] ¿Es para navegación interna? ? Interceptar click + modal
- [ ] ¿Es para cerrar tab/ventana? ? beforeunload con mensaje nativo
- [ ] ¿Necesitas notificación simple? ? Usar `window.showNotification`
- [ ] ¿El modal está en el Layout? ? Verificar que existe `#modalConfirmacion`

### **Al Actualizar Código Existente:**
- [ ] ¿Hay `alert()` o `confirm()`? ? Reemplazar por modal
- [ ] ¿Hay duplicación de código de modales? ? Centralizar
- [ ] ¿Los callbacks se ejecutan correctamente? ? Verificar timing
- [ ] ¿Funciona sin JavaScript? ? Agregar fallback apropiado

---

## ?? **DEBUGGING Y TROUBLESHOOTING**

### **Verificar Disponibilidad:**
```javascript
// En consola del navegador
console.log('Modal global disponible:', typeof window.mostrarModalConfirmacionGlobal === 'function');
console.log('Modal HTML existe:', $('#modalConfirmacion').length > 0);
console.log('Bootstrap modal disponible:', typeof $.fn.modal === 'function');
```

### **Problemas Comunes:**

#### **Modal no se muestra:**
```javascript
// Verificar orden de carga de scripts en _Layout.cshtml:
// 1. jQuery
// 2. Bootstrap
// 3. AdminLTE
// 4. modals.js ? Debe estar ANTES de scripts específicos
// 5. Scripts de página
```

#### **Callbacks no se ejecutan:**
```javascript
// Verificar que los callbacks son funciones válidas
window.mostrarModalConfirmacionGlobal(
    'Título',
    'Mensaje',
    'info',
    function() {
        console.log('? Callback onConfirm ejecutado');
        // Tu código aquí
    },
    function() {
        console.log('? Callback onCancel ejecutado');
        // Tu código aquí
    }
);
```

---

## ?? **CASOS DE USO DOCUMENTADOS**

### **Caso 1: Cambios Sin Guardar (Cotizaciones)**
```javascript
// Detectar navegación interna
$(document).on('click', 'a[href]', function(e) {
    if (verificarCambiosSinGuardar()) {
        e.preventDefault();
        const urlDestino = $(this).attr('href');
        
        window.mostrarModalConfirmacionGlobal(
            'Cambios Sin Guardar',
            '¿Está seguro de que desea salir sin guardar los cambios?<br><br>' +
            '<strong class="text-danger">Se perderán todos los cambios realizados.</strong>',
            'exit',
            function() {
                marcarComoGuardado();
                window.location.href = urlDestino;
            },
            null,
            {
                btnTextoConfirmar: 'Salir Sin Guardar',
                btnTextoCancelar: 'Quedarme Aquí'
            }
        );
    }
});
```

### **Caso 2: Cambio de Categoría (Parámetros)**
```javascript
$(document).on('click', '.categoria-tab', function(e) {
    if (checkUnsavedChanges()) {
        e.preventDefault();
        const urlDestino = $(this).attr('href');
        
        window.mostrarModalConfirmacionGlobal(
            'Cambios Sin Guardar',
            '¿Está seguro de que desea cambiar de categoría sin guardar?',
            'exit',
            function() {
                window.location.href = urlDestino;
            },
            null,
            {
                btnTextoConfirmar: 'Cambiar Sin Guardar',
                btnTextoCancelar: 'Quedarme Aquí'
            }
        );
    }
});
```

### **Caso 3: Validación de Formulario:**
```javascript
if (erroresValidacion.length > 0) {
    window.mostrarModalConfirmacionGlobal(
        'Errores de Validación',
        '<strong>No se puede guardar por los siguientes errores:</strong><br><br>' +
        erroresValidacion.join('<br>'),
        'warning',
        function() {
            // Al cerrar, enfocar primer campo con error
            $('#primerCampoError').focus();
        },
        null,
        {
            btnTextoConfirmar: 'Entendido',
            btnTextoCancelar: 'Cerrar'
        }
    );
}
```

---

## ?? **MIGRACIÓN DE CÓDIGO LEGACY**

### **Patrón de Migración:**

#### **Antes (alert/confirm nativo):**
```javascript
if (confirm('¿Está seguro de que desea eliminar?')) {
    eliminar();
}
```

#### **Después (modal Bootstrap):**
```javascript
window.mostrarModalConfirmacionGlobal(
    'Confirmar Eliminación',
    '¿Está seguro de que desea eliminar?',
    'danger',
    function() { eliminar(); }
);
```

---

## ?? **DIFERENCIAS CON FUNCIONES LOCALES**

### **En `editar.js`:**
```javascript
// Función LOCAL (wrapper de la global)
function mostrarModalConfirmacion(titulo, mensaje, tipo, onConfirm, onCancel, opciones) {
    // Intenta usar la global primero
    if (typeof window.mostrarModalConfirmacionGlobal === 'function') {
        window.mostrarModalConfirmacionGlobal(titulo, mensaje, tipo, onConfirm, onCancel, opciones);
        return;
    }
    
    // Fallback a implementación local si global no disponible
    // ... código de fallback
}
```

**VENTAJAS**:
- ? Compatible con código existente
- ? Usa versión global si está disponible
- ? Fallback local para robustez

---

## ?? **NOTAS PARA DESARROLLADORES**

### **?? Buenas Prácticas:**

1. **Siempre usar la función global** `window.mostrarModalConfirmacionGlobal` en código nuevo
2. **Personalizar textos de botones** para claridad contextual
3. **Incluir información relevante** en el mensaje del modal
4. **Manejar ambos callbacks** (onConfirm y onCancel) cuando sea necesario
5. **Documentar limitaciones** de beforeunload en comentarios

### **?? Reglas Críticas:**

1. **NUNCA usar** `alert()`, `confirm()` o `prompt()` del navegador
2. **SIEMPRE usar** modal Bootstrap para confirmaciones en navegación interna
3. **DOCUMENTAR** que beforeunload requiere mensaje nativo (inevitable)
4. **PROBAR** que los callbacks se ejecutan correctamente
5. **VERIFICAR** que el modal está en el Layout antes de usarlo

---

## ?? **ARCHIVOS RELACIONADOS**

### **JavaScript:**
- `~/js/modals.js` - Función global y configuración
- `~/js/site.js` - Función de notificaciones
- `~/js/cotizaciones/editar.js` - Implementación en cotizaciones
- `~/js/configuracion/parametros.js` - Implementación en parámetros

### **Vistas:**
- `~/Views/Shared/_Layout.cshtml` - Modal HTML genérico
- `~/Views/Cotizaciones/Editar.cshtml` - Uso del modal
- `~/Views/Configuracion/Parametros.cshtml` - Uso del modal

### **CSS:**
- `~/css/modals.css` - Estilos de modales

---

## ? **ESTADO DE IMPLEMENTACIÓN**

- ? **Función global creada** en `modals.js`
- ? **Modal HTML agregado** en `_Layout.cshtml`
- ? **editar.js actualizado** para usar modal
- ? **parametros.js actualizado** para usar modal
- ? **Documentación completa** de limitaciones de beforeunload
- ? **Fallbacks implementados** para robustez
- ? **Consistencia visual** con AdminLTE

---

**Versión**: 1.0  
**Última actualización**: 17 de marzo de 2026  
**Autor**: Marcela Jiménez (con GitHub Copilot)
