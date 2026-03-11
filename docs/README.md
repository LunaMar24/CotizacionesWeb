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

### 2. SISTEMA_PERMISOS.md
**Documentacion especifica** del sistema de autorizacion:
- Componentes del sistema de permisos
- 28 permisos definidos en 6 categorias
- Ejemplos de uso en Controllers, Vistas y Layout
- Troubleshooting detallado
- Guia de configuracion inicial paso a paso

**Tamano**: ~16 KB | **Ultima actualizacion**: 10/03/2026 7:47 PM

### 3. CHECKBOX_CATEGORIAS.md
**Documentacion tecnica** del checkbox de categorias:
- Como funcionan los checkboxes en headers de categoria
- Estados: marcado, desmarcado, indeterminado
- Implementacion CSS y JavaScript
- Ejemplos visuales y codigo

**Tamano**: ~7 KB | **Ultima actualizacion**: 10/03/2026 8:29 PM

---

## Uso Recomendado

### Para compartir contexto con un nuevo chat de GitHub Copilot:

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

### Modulos Pendientes
- Permisos (mantenimiento CRUD)
- Cotizaciones (modulo principal)
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

**Ultima actualizacion de esta documentacion**: 10 de marzo de 2026, 8:30 PM
