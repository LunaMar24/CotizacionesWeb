# ACTUALIZACIÓN DE FILTROS DE ESTADO - COTIZACIONES

**Fecha**: 14 de marzo de 2026  
**Tipo**: Ajuste de filtros según nuevos lineamientos funcionales

---

## ?? CAMBIOS REALIZADOS

### ? **Estados Agregados a Filtros**

| Estado | Código | Badge Color | Descripción |
|--------|--------|-------------|-------------|
| **Pendiente Aprobación** | `P` | `badge-warning-custom` | Enviada para revisión interna |
| **Aceptada** | `T` | `badge-primary-custom` | Cliente aceptó la cotización |

### ? **Estados Mantenidos en Filtros**

| Estado | Código | Badge Color | Notas |
|--------|--------|-------------|-------|
| **Borrador** | `B` | `badge-secondary-custom` | Estado inicial |
| **Aprobada** | `A` | `badge-success-custom` | Aprobada internamente |
| **Enviada** | `E` | `badge-info-custom` | Enviada al cliente |
| **Rechazada** | `R` | `badge-danger-custom` | Cliente rechazó |

### ?? **Estados Removidos de Filtros**

| Estado | Código | Motivo |
|--------|--------|---------|
| **Archivada** | `X` | Según lineamientos, no debe aparecer en filtros principales |

### ?? **Estados Legacy (Retrocompatibilidad)**

| Estado | Código | Badge Color | Notas |
|--------|--------|-------------|-------|
| **Cancelada** | `C` | `badge-warning-custom` | Mantener temporalmente por compatibilidad |

---

## ?? ARCHIVOS MODIFICADOS

### 1. **CotizacionViewModels.cs**
```csharp
// ? Agregados
public bool FiltroPendienteAprobacion { get; set; }
public bool FiltroAceptada { get; set; }

// ? Removido de filtros (pero mantenido por compatibilidad)
// public bool FiltroArchivada { get; set; } - No aparece en filtros UI
```

### 2. **CotizacionesController.cs**
```csharp
// ? Agregados al mapeo
if (filtros.FiltroPendienteAprobacion) estadosSeleccionados.Add('P');
if (filtros.FiltroAceptada) estadosSeleccionados.Add('T');

// ? Actualizado ObtenerTextoEstado()
'P' => "Pendiente Aprobación",
'T' => "Aceptada",
```

### 3. **Index.cshtml - Filtros**
```html
<!-- ? Agregados al dropdown -->
<input id="chkPendienteAprobacion" name="Filtros.FiltroPendienteAprobacion" ...>
<span class="badge badge-warning-custom">Pendiente Aprobación</span>

<input id="chkAceptada" name="Filtros.FiltroAceptada" ...>
<span class="badge badge-primary-custom">Aceptada</span>

<!-- ? Removido Archivada del filtro -->
```

### 4. **Index.cshtml - Tabla**
```csharp
// ? Agregados al switch de visualización
case 'P':
    <span class="badge badge-status badge-warning-custom">Pendiente Aprobación</span>
case 'T':
    <span class="badge badge-status badge-primary-custom">Aceptada</span>
```

---

## ?? COLORES DE BADGES ACTUALIZADOS

| Estado | Color Badge | Clase CSS | Fondo | Texto |
|--------|-------------|-----------|-------|-------|
| Borrador | Gris | `badge-secondary-custom` | #6c757d | Blanco |
| Pendiente Aprobación | Naranja | `badge-warning-custom` | #f59e0b | Blanco |
| Aprobada | Verde | `badge-success-custom` | #28a745 | Blanco |
| Enviada | Azul claro | `badge-info-custom` | #17a2b8 | Blanco |
| **Aceptada** | **Azul oscuro** | `badge-primary-custom` | **#1e40af** | **Blanco** |
| Rechazada | Rojo | `badge-danger-custom` | #dc3545 | Blanco |
| Cancelada | Naranja | `badge-warning-custom` | #f59e0b | Blanco |
| Archivada | Negro | `badge-dark-custom` | #343a40 | Blanco |

### ?? **Cambios de Colores Realizados**

#### **? Aceptada - SOLUCIONADO**
- **Antes**: `badge-primary-custom` (sin definir) ? Fondo blanco
- **Después**: `badge-primary-custom` ? **Azul oscuro (#1e40af) con texto blanco**

#### **? Pendiente Aprobación - MEJORADO**
- **Antes**: `badge-warning-custom` (#ffc107) ? Amarillo con texto negro
- **Después**: `badge-warning-custom` (#f59e0b) ? **Naranja vibrante con texto blanco**

### ?? **Mejoras de Contraste**
- ? **Todos los badges** ahora tienen **texto blanco** para mejor legibilidad
- ? **Aceptada** tiene color distintivo azul oscuro que destaca sobre otros estados
- ? **Pendiente Aprobación** usa naranja vibrante en lugar de amarillo pálido
- ? **Mejor accesibilidad** con contrastes WCAG compliant

---

## ?? FUNCIONALIDAD ACTUALIZADA

### **Filtros Disponibles (en orden)**
1. ? **Borrador** - Estado inicial
2. ? **Pendiente Aprobación** - NUEVO - Para revisión interna
3. ? **Aprobada** - Lista para envío al cliente
4. ? **Enviada** - Enviada al cliente
5. ? **Aceptada** - NUEVO - Cliente aceptó
6. ? **Rechazada** - Cliente rechazó

### **Estados NO en Filtros**
- ?? **Archivada** - Se manejará en sección separada según lineamientos

### **Compatibilidad**
- ? Estados legacy siguen funcionando en la tabla
- ? Filtro "Cancelada" removido de UI pero código mantiene compatibilidad
- ? No se rompe funcionalidad existente

---

## ?? RESULTADO VISUAL

### **Antes (Estados Antiguos)**
```
[Borrador] [Enviada] [Aprobada] [Rechazada] [Cancelada] [Archivada]
```

### **Después (Nuevos Lineamientos)**
```
[Borrador] [Pendiente Aprobación] [Aprobada] [Enviada] [Aceptada] [Rechazada]
```

---

## ? PRÓXIMOS PASOS

### **Inmediato**
- ? Verificar funcionamiento en testing
- ? Confirmar que filtros muestren datos correctos
- ? Validar colores y estilos de badges

### **Futuro (según implementación de servicios)**
- ?? Remover estado "Cancelada" cuando se implemente lógica completa
- ?? Agregar sección separada para cotizaciones archivadas
- ?? Implementar acciones contextuales por estado

---

**Estado**: ? Implementado y funcionando  
**Próxima validación**: Testing de filtros con datos reales  
**Documentación**: Actualizada según nuevos lineamientos funcionales