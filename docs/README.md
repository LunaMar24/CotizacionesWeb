# Documentacion del Proyecto CotizacionesWeb

Esta carpeta contiene la documentacion tecnica completa del proyecto.

---

## Documentos Disponibles

### 1. CONTEXTO_PROYECTO.md
**Documento principal** con toda la informacion del proyecto:
- Stack tecnologico y arquitectura
- Lineamientos y patrones de diseno
- Componentes reutilizables
- Errores comunes a evitar
- Checklist para nuevos modulos
- Ejemplos de codigo

**Tamano**: ~19 KB | **Ultima actualizacion**: 10/03/2026 7:46 PM

### 2. COTIZACIONES_DOMINIO_FUNCIONAL.md
**Lineamientos funcionales y de dominio** del modulo de Cotizaciones:
- Estados validos y flujos de transicion
- Reglas de negocio y validaciones
- Estructura de base de datos actualizada
- Casos de uso y servicios de Application
- Matriz de acciones por estado
- Implementacion tecnica detallada

**Tamano**: ~15 KB | **Ultima actualizacion**: 14/03/2026

### 3. COTIZACIONES_IMPLEMENTACION.md
**Documentacion de implementacion** del modulo de Cotizaciones:
- Componentes implementados por capa
- Funcionalidades y logica de negocio
- Estructura de base de datos
- UI moderna con filtros y modales
- Scripts de datos de prueba
- Guia de testing y debugging

**Tamano**: ~17 KB | **Ultima actualizacion**: 12/03/2026 11:17 PM

### 4. SISTEMA_PERMISOS.md
**Documentacion especifica** del sistema de autorizacion:
- Componentes del sistema de permisos
- 28 permisos definidos en 6 categorias
- Ejemplos de uso en Controllers, Vistas y Layout
- Troubleshooting detallado
- Guia de configuracion inicial paso a paso

**Tamano**: ~16 KB | **Ultima actualizacion**: 10/03/2026 7:47 PM

### 5. CHECKBOX_CATEGORIAS.md
**Documentacion tecnica** del checkbox de categorias:
- Como funcionan los checkboxes en headers de categoria
- Estados: marcado, desmarcado, indeterminado
- Implementacion CSS y JavaScript
- Ejemplos visuales y codigo

**Tamano**: ~7 KB | **Ultima actualizacion**: 10/03/2026 8:29 PM

---

## Uso Recomendado

### Para compartir lineamientos funcionales de Cotizaciones:

1. Abre `COTIZACIONES_DOMINIO_FUNCIONAL.md`
2. Revisa los estados validos y flujos de transicion
3. Copia las reglas de negocio relevantes
4. En el nuevo chat, escribe:
   ```
   Implementa estas reglas funcionales para el modulo de Cotizaciones:
   
   [Pegar seccion relevante aqui]
   ```
5. Asegurate de seguir estrictamente las transiciones permitidas

### Para compartir contexto tecnico completo:

1. Abre `CONTEXTO_PROYECTO.md`
2. Copia todo el contenido (Ctrl+A, Ctrl+C)
3. En el nuevo chat, escribe:
   ```
   Lee este contexto del proyecto y familiarizate con los lineamientos:
   
   [Pegar contenido completo aqui]
   ```
4. Espera confirmacion del chat
5. Comienza a trabajar siguiendo los patrones establecidos

### Para resolver problemas de permisos:

1. Consulta `SISTEMA_PERMISOS.md`
2. Revisa la seccion de **Troubleshooting**
3. Verifica que hayas ejecutado los scripts SQL en orden
4. Confirma que los usuarios hayan hecho logout/login despues de cambios

---

## Mantenimiento de Documentos

### Cuando actualizar:
- Cuando se agreguen nuevos modulos
- Cuando se definan nuevos permisos
- Cuando se cambien patrones de diseno
- Cuando se identifiquen nuevos errores comunes

### Formato:
- Markdown con sintaxis GitHub-flavored
- Usar emojis para facilitar navegacion
- Ejemplos de codigo con syntax highlighting
- Secciones claramente delimitadas

---

## Estructura de la Documentacion

```
docs/
??? README.md                    # Este archivo
??? CONTEXTO_PROYECTO.md         # Lineamientos generales
??? SISTEMA_PERMISOS.md          # Sistema de autorizacion
??? CHECKBOX_CATEGORIAS.md       # Checkboxes de categoria
```

---

## Estado Actual del Proyecto

### Modulos Implementados
- Dashboard/Home (estadisticas, actividad reciente)
- Usuarios (CRUD completo + gestion roles + reset password)
- Roles (CRUD completo + gestion permisos con checkboxes de categoria)
- Cotizaciones (listado, filtros, versiones, historial - Estados basicos implementados)

### Modulos Pendientes
- Permisos (mantenimiento CRUD)
- Cotizaciones - Estados avanzados (PendienteAprobacion, Aceptada, flujos de transicion)
- Cotizaciones - Acciones por estado (Aprobar, Enviar a cliente, etc.)
- Cotizaciones - Integracion ERP
- Clientes
- Productos/Servicios
- Reportes

---

## Informacion de Contacto

**Proyecto**: CotizacionesWeb  
**Repositorio**: https://github.com/LunaMar24/CotizacionesWeb  
**Branch**: Marcela/TrabajoPrueba  
**Autor**: Marcela Jimenez  
**Version**: 1.2  

---

**Ultima actualizacion de esta documentacion**: 14 de marzo de 2026, 11:30 PM
