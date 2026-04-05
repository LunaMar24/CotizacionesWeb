# Instrucciones para Plantilla de Cotización Word

## ?? Información General

Este archivo describe cómo crear y configurar la plantilla Word (`PlantillaCotizacion.docx`) para la generación automática de documentos de cotización.

---

## ?? Ubicación de la Plantilla

**Ruta requerida:** `src/CotizacionesWeb.UI/wwwroot/templates/PlantillaCotizacion.docx`

?? **Importante:** El archivo debe tener exactamente este nombre y estar en esta ubicación.

---

## ??? Placeholders Disponibles

### Placeholders Simples
Estos se reemplazan directamente por sus valores:

| Placeholder | Descripción | Ejemplo |
|-------------|-------------|---------|
| `{{COTIZACION_ID}}` | ID de la cotización | COT-0001 |
| `{{VERSION}}` | Versión de la cotización | v2.0 |
| `{{FECHA_COTIZACION}}` | Fecha de la cotización | 15/03/2026 |
| `{{CLIENTE}}` | Nombre del cliente | Juan Pérez |
| `{{EMPRESA}}` | Empresa del cliente | Tech Solutions S.A. |
| `{{EMAIL}}` | Email del cliente | juan@tech.com |
| `{{MONEDA}}` | Código de moneda | CRC |
| `{{TIPO_CAMBIO}}` | Tipo de cambio | 1.00 |
| `{{VIGENCIA}}` | Vigencia de la oferta | 30 días |
| `{{CONDICIONES_PAGO}}` | Condiciones de pago | Contado |
| `{{NOTAS_COMERCIALES}}` | Notas comerciales | Precios sujetos a... |
| `{{SUBTOTAL}}` | Subtotal formateado | ?125,000.00 |
| `{{IMPUESTO_TOTAL}}` | Impuesto formateado | ?16,250.00 |
| `{{TOTAL}}` | Total formateado | ?141,250.00 |

### Placeholder Especial
| Placeholder | Descripción |
|-------------|-------------|
| `{{DETALLE_COTIZACION}}` | Se reemplaza por el bloque completo de productos |

---

## ?? Formato del Bloque de Detalle

El placeholder `{{DETALLE_COTIZACION}}` se convierte en bloques como este para cada producto:

```
**PROD001 - Laptop Dell Inspiron**
Laptop para oficina con procesador Intel i5, 8GB RAM, 256GB SSD

Cantidad: 2.00
Precio unitario: ?450,000.00
Subtotal: ?900,000.00
Impuesto (13.0%): ?117,000.00
Total línea: ?1,017,000.00

```

---

## ?? Recomendaciones de Diseño

### Estructura Sugerida
```
[ENCABEZADO EMPRESA]
- Logo
- Información de contacto

COTIZACIÓN: {{COTIZACION_ID}} - {{VERSION}}
Fecha: {{FECHA_COTIZACION}}

CLIENTE:
- {{CLIENTE}}
- {{EMPRESA}}
- {{EMAIL}}

Moneda: {{MONEDA}}
Tipo de cambio: {{TIPO_CAMBIO}}

{{DETALLE_COTIZACION}}

RESUMEN:
- Subtotal: {{SUBTOTAL}}
- Impuesto: {{IMPUESTO_TOTAL}}
- TOTAL: {{TOTAL}}

TÉRMINOS:
- Vigencia: {{VIGENCIA}}
- Condiciones de pago: {{CONDICIONES_PAGO}}

{{NOTAS_COMERCIALES}}

[PIE DE PÁGINA]
```

### Formato Recomendado
- **Fuente:** Calibri o Arial 11pt
- **Márgenes:** 2.5 cm en todos los lados
- **Títulos:** Negrita, 14pt
- **Total:** Negrita, resaltado
- **Espaciado:** 1.15 líneas

---

## ?? Configuración de Parámetros

Los siguientes valores se pueden personalizar desde el sistema de parámetros:

| Parámetro | Placeholder Relacionado | Valor Por Defecto |
|-----------|-------------------------|-------------------|
| `COT_VIGENCIA` | `{{VIGENCIA}}` | 30 días |
| `COT_CONDICIONES_PAGO` | `{{CONDICIONES_PAGO}}` | Contado |
| `COT_NOTAS_COMERCIALES` | `{{NOTAS_COMERCIALES}}` | Precios sujetos a... |
| `COT_TITULO_DETALLE` | Se agrega antes del detalle | DETALLE DE PRODUCTOS Y SERVICIOS |

---

## ?? Pasos para Crear la Plantilla

1. **Crear documento Word nuevo**
2. **Diseñar el layout** con la estructura deseada
3. **Insertar placeholders** en las posiciones correctas
4. **Aplicar formato** (negritas, colores, etc.)
5. **Guardar como** `PlantillaCotizacion.docx`
6. **Colocar en** `src/CotizacionesWeb.UI/wwwroot/templates/`

---

## ? Verificación

Para verificar que la plantilla funciona correctamente:

1. Ir a `/Documentos/ValidarPlantilla` (requiere permisos de administrador)
2. El sistema mostrará:
   - ? Si la plantilla es válida
   - ?? Lista de placeholders encontrados
   - ?? Cualquier problema detectado

---

## ?? Solución de Problemas

### Error: "Plantilla no encontrada"
- ? Verificar ubicación: `wwwroot/templates/PlantillaCotizacion.docx`
- ? Verificar nombre exacto del archivo
- ? Verificar permisos de lectura

### Error: "Documento inválido"
- ? Abrir plantilla en Word y guardar nuevamente
- ? Verificar que no está corrupta
- ? Verificar que es formato .docx (no .doc)

### Placeholder no se reemplaza
- ? Verificar sintaxis exacta: `{{NOMBRE_PLACEHOLDER}}`
- ? No debe haber espacios extra dentro de las llaves
- ? Verificar que el placeholder está en la lista soportada

---

## ?? Soporte Técnico

Para problemas con la plantilla:
1. Verificar logs de la aplicación
2. Usar endpoint de validación
3. Revistar este documento
4. Contactar al administrador del sistema