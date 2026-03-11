# Checkbox de Categorías en Gestión de Permisos

## ? Mejora Implementada

Se ha agregado un **checkbox visual** en cada header de categoría para facilitar la selección/deselección masiva de permisos.

---

## ?? Comportamiento del Checkbox

### Estados Visuales

#### 1. **Desmarcado** (ningún permiso seleccionado)
```
[ ] Usuarios (6 permisos)
```
- Checkbox vacío con borde blanco
- Fondo semi-transparente

#### 2. **Marcado** (todos los permisos seleccionados)
```
[?] Usuarios (6 permisos)
```
- Checkbox con ? (check mark)
- Fondo blanco sólido
- Ícono azul

#### 3. **Indeterminado** (algunos permisos seleccionados)
```
[?] Usuarios (6 permisos)
```
- Checkbox con ? (minus)
- Fondo semi-transparente
- Indica selección parcial

---

## ??? Interacción del Usuario

### Al hacer clic en el checkbox de categoría:

**Si ninguno está marcado o algunos:**
- ? Marca TODOS los permisos de la categoría
- Checkbox pasa a estado "marcado"

**Si todos están marcados:**
- ? Desmarca TODOS los permisos de la categoría
- Checkbox pasa a estado "desmarcado"

### Al hacer clic en un permiso individual:

**Automáticamente actualiza el checkbox de categoría:**
- 0 seleccionados ? Checkbox desmarcado
- Todos seleccionados ? Checkbox marcado
- Algunos seleccionados ? Checkbox indeterminado (?)

---

## ?? Implementación Técnica

### Vista (_PermisosModal.cshtml)
```razor
<div class="categoria-header" data-categoria="@grupo.Key">
    <input type="checkbox" 
           class="categoria-checkbox" 
           id="cat_@grupo.Key.Replace(" ", "_")"
           title="Seleccionar/Deseleccionar todos">
    <label for="cat_@grupo.Key.Replace(" ", "_")" class="categoria-label">
        <i class="fas fa-folder"></i>
        <strong>@grupo.Key</strong>
        <span class="badge badge-light ml-2">@grupo.Count() permisos</span>
    </label>
</div>

<!-- Permisos de la categoría -->
<input type="checkbox" 
       class="permiso-checkbox" 
       data-categoria="@grupo.Key"
       name="permisosIds" 
       value="@permiso.Id">
```

### CSS (create.css)
```css
.categoria-checkbox {
    width: 22px;
    height: 22px;
    cursor: pointer;
    appearance: none;
    background-color: rgba(255, 255, 255, 0.2);
    border: 2px solid white;
    border-radius: 4px;
    position: relative;
}

.categoria-checkbox:checked {
    background-color: white;
}

.categoria-checkbox:checked::before {
    content: "\f00c"; /* Check mark */
    font-family: "Font Awesome 6 Free";
    color: var(--color-primary);
}

.categoria-checkbox:indeterminate {
    background-color: rgba(255, 255, 255, 0.7);
}

.categoria-checkbox:indeterminate::before {
    content: "\f068"; /* Minus */
    font-family: "Font Awesome 6 Free";
    color: var(--color-primary);
}
```

### JavaScript (index.js)

#### Evento 1: Cambio en checkbox de categoría
```javascript
$(document).on('change', '.categoria-checkbox', function() {
    const categoria = $(this).closest('.categoria-header').data('categoria');
    const isChecked = $(this).is(':checked');
    
    // Marcar/desmarcar todos los permisos de la categoría
    $(`.permiso-checkbox[data-categoria="${categoria}"]`).prop('checked', isChecked);
});
```

#### Evento 2: Cambio en permiso individual
```javascript
$(document).on('change', '.permiso-checkbox', function() {
    const categoria = $(this).data('categoria');
    const checkboxes = $(`.permiso-checkbox[data-categoria="${categoria}"]`);
    const checkedCount = checkboxes.filter(':checked').length;
    const totalCount = checkboxes.length;
    
    const categoriaCheckbox = $(`.categoria-header[data-categoria="${categoria}"] .categoria-checkbox`);
    
    if (checkedCount === 0) {
        categoriaCheckbox.prop('checked', false);
        categoriaCheckbox.prop('indeterminate', false);
    } else if (checkedCount === totalCount) {
        categoriaCheckbox.prop('checked', true);
        categoriaCheckbox.prop('indeterminate', false);
    } else {
        categoriaCheckbox.prop('checked', false);
        categoriaCheckbox.prop('indeterminate', true); // Estado intermedio
    }
});
```

#### Evento 3: Inicialización al abrir modal
```javascript
$(document).on('shown.bs.modal', '#permisosManageModal', function() {
    $('.categoria-group').each(function() {
        // Calcular estado inicial de cada checkbox de categoría
        // basado en los permisos ya seleccionados
    });
});
```

---

## ?? Beneficios de la Implementación

### Usabilidad Mejorada
? **Visual claro**: El checkbox muestra el estado de la categoría  
? **Feedback inmediato**: Se actualiza al cambiar permisos individuales  
? **Estado indeterminado**: Indica selección parcial  
? **Accesibilidad**: Funciona con teclado (Tab + Space)  

### Eficiencia
? **Selección rápida**: Un click para marcar 6+ permisos  
? **Menos errores**: Visual claro del estado actual  
? **Intuitivo**: Comportamiento estándar de checkboxes  

---

## ?? Estados del Checkbox

| Permisos Seleccionados | Estado Checkbox | Icono | Color Fondo |
|------------------------|-----------------|-------|-------------|
| 0 de 6                 | Desmarcado      | Vacío | Semi-transparente |
| 3 de 6                 | Indeterminado   | ?     | Blanco 70% |
| 6 de 6                 | Marcado         | ?     | Blanco 100% |

---

## ??? Ejemplo Visual

```
??????????????????????????????????????????????
? [?] Usuarios          ? 6 permisos ?      ?  <- Todos marcados
??????????????????????????????????????????????
?   [?] USR_VIEW - Ver usuarios              ?
?   [?] USR_CREATE - Crear nuevos usuarios   ?
?   [?] USR_EDIT - Editar usuarios           ?
?   [?] USR_DELETE - Eliminar usuarios       ?
?   [?] USR_ROLES - Gestionar roles          ?
?   [?] USR_RESET_PWD - Resetear contraseñas ?
??????????????????????????????????????????????

??????????????????????????????????????????????
? [?] Roles             ? 5 permisos ?      ?  <- Algunos marcados
??????????????????????????????????????????????
?   [?] ROL_VIEW - Ver roles                 ?
?   [?] ROL_CREATE - Crear nuevos roles      ?
?   [ ] ROL_EDIT - Editar roles              ?
?   [ ] ROL_DELETE - Eliminar roles          ?
?   [ ] ROL_PERMISOS - Gestionar permisos    ?
??????????????????????????????????????????????

??????????????????????????????????????????????
? [ ] Cotizaciones      ? 7 permisos ?      ?  <- Ninguno marcado
??????????????????????????????????????????????
?   [ ] COT_VIEW - Ver cotizaciones          ?
?   [ ] COT_CREATE - Crear cotizaciones      ?
?   ...                                       ?
??????????????????????????????????????????????
```

---

## ?? Notas Técnicas

### Propiedad `indeterminate`
La propiedad `indeterminate` de un checkbox:
- Solo se puede establecer con JavaScript: `.prop('indeterminate', true)`
- No existe en HTML: `<input type="checkbox" indeterminate>` ?
- Es un estado visual, NO un tercer valor (sigue siendo checked o unchecked)
- Útil para representar selección parcial en jerarquías

### Font Awesome Icons
- `\f00c`: check (?)
- `\f068`: minus (?)
- `\f00d`: times (?)

### CSS `appearance: none`
Necesario para personalizar completamente el checkbox:
```css
appearance: none;
-webkit-appearance: none;
-moz-appearance: none;
```

---

**Última actualización**: 10 de marzo de 2026, 8:00 PM  
**Autor**: Marcela Jiménez (con GitHub Copilot)
