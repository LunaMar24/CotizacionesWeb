# 📄 Motor de Generación de Documentos Word para Cotizaciones

## ✅ IMPLEMENTACIÓN COMPLETADA
Motor funcional para generar documentos Word a partir de plantilla (.docx) usando OpenXML, con reemplazo de placeholders y bloque dinámico de detalle.

---

## 🏗️ ARQUITECTURA

### Application
- DTOs: CotizacionDocumentDto, DetalleDocumentDto
- Interface: IDocumentoCotizacionService

### Infrastructure
- DocumentoCotizacionService:
  - Reemplazo de placeholders simples (body, headers, footers)
  - Bloque dinámico {{DETALLE_COTIZACION}}
  - Integración con parámetros
  - Logging y manejo de errores

### UI
- DocumentosController
- Integración en vista Detalle (descarga Word)

---

## 🔧 FUNCIONALIDAD

### Placeholders simples (ejemplos)
{{COTIZACION_ID}}, {{VERSION}}, {{FECHA_COTIZACION}}, {{CLIENTE}}, {{EMPRESA}}, {{EMAIL}}, {{MONEDA}}, {{TIPO_CAMBIO}}, {{VIGENCIA}}, {{CONDICIONES_PAGO}}, {{NOTAS_COMERCIALES}}, {{SUBTOTAL}}, {{IMPUESTO_TOTAL}}, {{TOTAL}}

### Bloque dinámico
- Placeholder: {{DETALLE_COTIZACION}} (solo en body, en su propio párrafo)
- Formato por línea:
  - Producto (negrita)
  - Descripción
  - Cantidad, Precio unitario
  - Subtotal, Impuesto (% y monto), Total línea
  - Separación visual entre bloques

---

## 🚀 MEJORAS IMPLEMENTADAS

### 1) Reemplazo robusto SIN perder formato
- Estrategia híbrida:
  - Reemplazo por run cuando es posible
  - Fallback robusto cuando el placeholder está fragmentado
- Evita pérdida de negrita/colores/tamaños

### 2) Encabezados y pies de página
- Se procesan Body, HeaderParts y FooterParts
- Mismos placeholders simples funcionan en todo el documento
- {{DETALLE_COTIZACION}} solo en body

### 3) Saltos de línea en placeholders
- Soporte para \r\n, \n, \r
- Uso de OpenXML Break para preservar enters
- Aplica a:
  - {{CONDICIONES_PAGO}}, {{NOTAS_COMERCIALES}}, {{VIGENCIA}}, etc.

### 4) Eliminación correcta de {{DETALLE_COTIZACION}}
- Se elimina el placeholder y se insertan bloques dinámicos

### 5) Formato de moneda por cultura
- CRC → es-CR
- USD → en-US

### 6) Separación visual y legibilidad
- Espaciado entre bloques
- Jerarquía visual

---

## 📁 ESTRUCTURA

src/
- Application/Documents
- Infrastructure/Services
- UI/Controllers

Plantilla:
wwwroot/templates/PlantillaCotizacion.docx

---

## ⚙️ CONFIGURACIÓN

- Paquete: DocumentFormat.OpenXml
- Registro DI:
  AddScoped<IDocumentoCotizacionService, DocumentoCotizacionService>()
- Parámetros:
  - DOC_PLANTILLA_PATH
  - COT_VIGENCIA
  - COT_CONDICIONES_PAGO
  - COT_NOTAS_COMERCIALES
  - COT_TITULO_DETALLE
  - COT_TITULO_RESUMEN

---

## 🧪 TESTING

Validar:
- Placeholders reemplazados (body/header/footer)
- Formato conservado (negrita/colores)
- Saltos de línea respetados
- Moneda correcta (CRC/USD)
- {{DETALLE_COTIZACION}} no visible y bloques correctos

---

## 🧠 REGLAS DE PLANTILLA

- Placeholders exactos: {{PLACEHOLDER}}
- {{DETALLE_COTIZACION}} en su propio párrafo
- Evitar mezclar texto + placeholder en la misma línea para el bloque
- Formato uniforme en placeholders (evitar fragmentación por estilos distintos)

---

## 🎯 RESULTADO

✔ Motor robusto  
✔ Formato preservado  
✔ Soporte de headers/footers  
✔ Soporte de saltos de línea  
✔ Documento profesional listo para PDF  

Pendiente: ajustar/afinar plantilla Word final según diseño comercial.
