# 📄 Motor de Generación de Documentos Word para Cotizaciones

## ✅ IMPLEMENTACIÓN COMPLETADA

Se ha implementado exitosamente el motor de generación de documentos Word para cotizaciones siguiendo la arquitectura del proyecto.

---

## 🏗️ ARQUITECTURA

### Application
- DTOs: CotizacionDocumentDto, DetalleDocumentDto
- Interface: IDocumentoCotizacionService

### Infrastructure
- DocumentoCotizacionService:
  - Placeholders simples
  - Bloque dinámico
  - Integración con parámetros
  - Logging

### UI
- DocumentosController
- Integración en vista Detalle

---

## 🔧 FUNCIONALIDAD

### Placeholders
{{COTIZACION_ID}}, {{CLIENTE}}, {{MONEDA}}, {{TOTAL}}, etc.

### Bloque dinámico
{{DETALLE_COTIZACION}} con:
- Producto
- Descripción
- Cantidad
- Precio
- Impuesto
- Total

---

## 🚀 MEJORAS IMPLEMENTADAS

### 1. Reemplazo robusto
Soluciona fragmentación de Word (runs)

### 2. Eliminación de placeholder
{{DETALLE_COTIZACION}} se elimina completamente

### 3. Moneda por cultura
- CRC → es-CR
- USD → en-US

### 4. Formato visual
- Separación de bloques
- Colores y jerarquía
- Espaciado

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

- OpenXML
- Servicio registrado en DI
- Parámetros del sistema

---

## 🧪 TESTING

Validar:
- Placeholders no visibles
- Moneda correcta
- Separación visual
- Detalle completo

---

## 🎯 RESULTADO

✔ Motor robusto  
✔ Arquitectura limpia  
✔ Documento profesional  

Pendiente: crear plantilla Word.
