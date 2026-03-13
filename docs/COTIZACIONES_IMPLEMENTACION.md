# MODULO DE COTIZACIONES - IMPLEMENTACION COMPLETA

**Fecha**: 12 de marzo de 2026, 11:15 PM  
**Fase**: Listado, Operaciones Basicas y UI Moderna

---

## COMPONENTES IMPLEMENTADOS

### 1. Domain Layer

**Enums creados:**
- `EstadoCotizacion.cs` - Estados del ciclo de vida (B/E/A/R/C/X)
- `TipoEvento.cs` - Eventos de auditoria (clase estatica con constantes string)
- `TipoArchivo.cs` - Tipos de archivo (A/S/C)
- `TipoInteresado.cs` - Clasificacion de interesados (P/E/O)
- `TipoParametro.cs` - Tipos de parametros (N/S/B)

**Entidades creadas (CORREGIDAS - Sin IDs duplicados):**
- `Interesado.cs` - Cliente/Contacto vinculado con HubSpot
- `Parametros.cs` - Configuracion del sistema
- `Cotizacion.cs` - Entidad principal
- `CotizacionVersion.cs` - Versionado historico con snapshot (usa Id de BaseEntity)
- `DetalleCotizacionVersion.cs` - Lineas de productos (usa Id de BaseEntity)
- `HistorialCotizacion.cs` - Auditoria de eventos (usa Id de BaseEntity)
- `ArchivoCotizacion.cs` - Control de archivado/reactivacion (usa Id de BaseEntity)

**CORRECCION IMPORTANTE**: Se eliminaron propiedades duplicadas:
- ? `CotizacionVersion.VersionId` (duplicaba Id)
- ? `HistorialCotizacion.HistorialId` (duplicaba Id)
- ? `DetalleCotizacionVersion.DetalleVersionId` (duplicaba Id)
- ? `ArchivoCotizacion.ArchivoId` (duplicaba Id)

Ahora todas usan correctamente `Id` heredado de `BaseEntity`.

### 2. Infrastructure Layer

**Configurations EF Core creadas:**
- `InteresadoConfiguration.cs`
- `ParametrosConfiguration.cs`
- `CotizacionConfiguration.cs`
- `CotizacionVersionConfiguration.cs` - Mapea Id ? VersionId en BD
- `DetalleCotizacionVersionConfiguration.cs` - Mapea Id ? DetalleVersionId en BD
- `HistorialCotizacionConfiguration.cs` - Mapea Id ? HistorialId en BD
- `ArchivoCotizacionConfiguration.cs` - Mapea Id ? ArchivoId en BD

**DbContext actualizado:**
- Agregados 7 DbSets nuevos para cotizaciones

**Migracion creada:**
- `AgregarTablasCompletasCotizacionesCorregido` - Todas las tablas sin campos duplicados

**Scripts SQL:**
- `SeedCotizacionesPrueba.sql` - Datos de prueba (3 cotizaciones)
- `LimpiarDatosCotizaciones.sql` - Limpiar datos y resetear IDs

**Servicios:**
- `CotizacionService.cs` - Implementacion completa de casos de uso

### 3. Application Layer

**DTOs y Models:**
- `CotizacionDtos.cs` - DTOs, Requests y Results para todas las operaciones

**Interfaces:**
- `ICotizacionService.cs` - Contrato del servicio con 8 metodos

**Casos de Uso Implementados:**
1. `GetCotizacionesListAsync` - Listar cotizaciones con filtros
2. `GetCotizacionCurrentHistoryAsync` - Historial de version actual
3. `GetCotizacionVersionsAsync` - Listar todas las versiones
4. `GetCotizacionVersionDetailAsync` - Detalle de version especifica
5. `GetCotizacionVersionHistoryAsync` - Historial de version especifica
6. `CopiarVersionActualAsync` - Copiar version actual
7. `CopiarVersionEspecificaAsync` - Copiar version antigua con confirmacion
8. `DuplicarCotizacionAsync` - Crear nueva cotizacion independiente

### 4. UI Layer

**Controller:**
- `CotizacionesController.cs` - Orquestacion sin logica de negocio

**ViewModels:**
- `CotizacionViewModels.cs` - ViewModels para UI

**Vistas:**
- `Index.cshtml` - Listado principal con filtros colapsables
- `_HistorialModal.cshtml` - Modal de historial con timeline
- `_VersionesModal.cshtml` - Modal de versiones con sub-acciones

**CSS:**
- `index.css` - Estilos modernos para listado, tabla y timeline

**JavaScript:**
- `index.js` - Funcionalidad de modales y acciones (sin tildes, con modal de confirmacion)

**Layout:**
- Menu "Listar Cotizaciones" agregado y funcional

---

## FUNCIONALIDADES IMPLEMENTADAS

### Listado Principal (Look and Feel Moderno)

**Header Superior:**
- Titulo grande con icono
- Subtitulo descriptivo
- Boton "CREAR COTIZACION" azul en esquina superior derecha
- Boton con efecto hover (elevacion y sombra)

**Filtros Colapsables:**
- Card con boton de colapsar/expandir
- Busqueda general con icono de lupa
- Fecha Desde con icono de calendario
- Fecha Hasta con icono de calendario
- Dropdown de estados con checkboxes (permite multiples)
- Botones compactos de Buscar y Limpiar

**Tabla Moderna:**
- Sin card wrapper (tabla directa con bordes redondeados)
- Header azul oscuro con texto en MAYUSCULAS
- Filas con hover gris suave
- Columnas: ID, Cliente, Empresa, Estado, Version, Monto, Fechas, Acciones
- Footer con total de registros

**Botones de Accion Coloreados:**
- ?? **Amarillo** (Editar) - Ver Detalle
- ?? **Azul** (Info) - Ver Historial
- ?? **Morado** (Secundario) - Ver Versiones
- ?? **Verde** (Copiar) - Copiar Version
- ?? **Rojo** (Duplicar) - Duplicar Cotizacion

### Modal de Confirmacion
- Reemplaza `confirm()` del navegador
- Colores dinamicos segun tipo (info/warning/danger/success)
- Soporta HTML en el mensaje
- Iconos personalizados
- Mejor UX que alerts nativos

### Historial
- Timeline visual con iconos
- Colores por tipo de evento
- Fecha y hora formateada
- Comentarios adicionales
- Ordenado cronologicamente

### Versiones
- Lista todas las versiones
- Indica version actual vigente
- Acciones por version:
  - Copiar (con modal de confirmacion si es historica)
  - Ver Detalle (pendiente)
  - Ver Historial (pendiente)

---

## LOGICA DE NEGOCIO

### Copiar Version

**Desde listado principal:**
- Copia la version actual vigente
- Incrementa NumeroVersion
- Marca nueva version como actual
- Copia todos los detalles
- Registra evento "Version Generada"
- Mantiene historial previo

**Desde sub-listado de versiones:**
- Permite copiar cualquier version
- Si no es la actual, muestra modal de advertencia
- Mismo comportamiento que copiar actual
- Advertencia de que usara datos historicos

### Duplicar Cotizacion
Crea nueva cotizacion independiente:
- Genera nuevo CotizacionId (formato COT-####)
- Estado inicial: Borrador (B)
- NumeroVersion: 1
- Copia solo lineas de detalle y configuraciones
- Limpia: InteresadoId, Notas
- Historial nuevo desde cero
- Registra evento "Creada"

---

## BASE DE DATOS

### Tablas Creadas (Con IDs Correctos)
1. `Interesado` - Id (PK, Identity)
2. `Parametros` - Id (PK, Identity)
3. `Cotizacion` - Id (PK, Identity)
4. `CotizacionVersion` - VersionId (PK, Identity) ? Mapeado desde Id
5. `DetalleCotizacionVersion` - DetalleVersionId (PK, Identity) ? Mapeado desde Id
6. `HistorialCotizacion` - HistorialId (PK, Identity) ? Mapeado desde Id
7. `ArchivoCotizacion` - ArchivoId (PK, Identity) ? Mapeado desde Id

**NOTA**: Ya NO existen campos duplicados como VersionId1, HistorialId1, etc.

### Indices Creados
- `Cotizacion.CotizacionId` (UNIQUE)
- `CotizacionVersion(CotizacionId, NumeroVersion)` (UNIQUE)
- `Interesado.HubspotObjectId`
- `HistorialCotizacion.FechaEvento`
- `ArchivoCotizacion.FechaArchivado`

---

## DATOS DE PRUEBA

**Script:** `SeedCotizacionesPrueba.sql`

**Contenido:**
- 3 Interesados de prueba
- 4 Parametros del sistema
- 3 Cotizaciones:
  - COT-0001: Borrador, 1 version
  - COT-0002: Enviada, 2 versiones (ejemplo de versionado)
  - COT-0003: Aprobada, 1 version
- Detalles de productos por version
- Historiales de eventos

**Script de Limpieza:** `LimpiarDatosCotizaciones.sql`
- Elimina todos los datos de prueba
- Resetea IDENTITY seeds
- Incluye verificacion

---

## COMO PROBAR

### 1. Ejecutar Script de Limpieza (si es necesario)
```sql
src/CotizacionesWeb.Infrastructure/Data/Scripts/LimpiarDatosCotizaciones.sql
```

### 2. Insertar Datos de Prueba
```sql
src/CotizacionesWeb.Infrastructure/Data/Scripts/SeedCotizacionesPrueba.sql
```

### 3. Ejecutar la Aplicacion
```bash
dotnet run --project src/CotizacionesWeb.UI
```

### 4. Navegar al Modulo
- Login con usuario admin
- Ir a: **Cotizaciones > Listar Cotizaciones**

### 5. Probar Funcionalidades
- Filtrar por estados (dropdown con checkboxes)
- Buscar por texto
- Filtrar por fechas
- Colapsar/expandir filtros
- Ver historial de cotizacion (modal)
- Ver versiones de cotizacion (modal)
- Copiar version actual (modal de confirmacion azul)
- Copiar version antigua (modal de advertencia amarillo)
- Duplicar cotizacion (modal de confirmacion amarillo)
- Verificar permisos (botones deshabilitados si no tiene COT_VIEW, COT_EDIT, COT_CREATE)

---

## ESTILOS Y COMPONENTES

### Badges de Estado Personalizados
- **Borrador**: `badge-secondary-custom` (gris, padding: 6px 14px, border-radius: 12px)
- **Enviada**: `badge-info-custom` (azul claro)
- **Aprobada**: `badge-success-custom` (verde)
- **Rechazada**: `badge-danger-custom` (rojo)
- **Cancelada**: `badge-warning-custom` (amarillo)
- **Archivada**: `badge-dark-custom` (negro)

### Timeline de Historial
- Componente AdminLTE timeline
- Colores dinamicos segun tipo de evento
- Iconos Font Awesome personalizados
- Linea vertical azul conectando eventos

### Botones de Accion Modernos
**Estilo circular (36x36px):**
- **Amarillo** (#f59e0b): Editar/Ver Detalle
- **Azul** (#3b82f6): Ver Historial
- **Morado** (#6366f1): Ver Versiones
- **Verde** (#10b981): Copiar
- **Rojo** (#ef4444): Duplicar

Con efecto hover: elevacion y sombra

### Card de Filtros
- Fondo gris claro (#f9fafb)
- Border gris (#e5e7eb)
- Border-radius: 8px
- Boton de colapsar/expandir en header

---

## PERMISOS UTILIZADOS

- `COT_VIEW` - Ver listado, historial, versiones
- `COT_EDIT` - Copiar versiones
- `COT_CREATE` - Duplicar cotizaciones, crear nuevas

---

## NOTAS TECNICAS

### Formato de IDs
1. **CotizacionId**: String con formato `COT-####` (generado automaticamente)
2. **NumeroVersion**: Int (aunque en BD es decimal(3,1) para permitir 1.1, 1.2, etc.)
3. **VersionActual**: Char '1' = actual, '0' = historica
4. **EstadoActual**: Char (B/E/A/R/C/X) con mapeo visual

### Snapshot Pattern
Los datos del interesado (nombre, email, empresa) se duplican en cada version para mantener historial inmutable.

### Confirmaciones
- Copiar version actual: Modal azul informativo
- Duplicar cotizacion: Modal amarillo de advertencia
- Copiar version historica: Modal amarillo con alerta destacada

### TipoEvento
Implementado como clase estatica con constantes string para que los valores en BD sean legibles:
- "Creada"
- "Version Generada"
- "Enviada"
- "Aprobada"
- "Rechazada"
- "Cancelada"
- "Archivada"
- "Reactivada"

---

## PENDIENTES PARA PROXIMAS FASES

### Funcionalidades No Implementadas
- Ver Detalle completo de version (modal)
- Editar cotizacion
- Archivar cotizacion
- Crear nueva cotizacion (formulario completo)
- Cambiar estado de cotizacion
- Enviar cotizacion al cliente
- Integracion con HubSpot
- Enviar a ERP
- Exportar PDF/Excel

### Validaciones Adicionales
- Validar que no se pueda copiar/duplicar cotizacion archivada
- Validar estados permitidos para cada accion
- Validar permisos especificos por estado
- Validar montos y calculos

---

## PROBLEMAS RESUELTOS

### 1. IDs Duplicados en BD
**Problema**: EF Core generaba campos adicionales (VersionId1, HistorialId1, etc.)

**Causa**: Las entidades tenian propiedades duplicadas ademas del Id heredado.

**Solucion**: 
- Eliminar propiedades duplicadas de las entidades
- Mapear Id ? NombreId en las configuraciones
- Revertir migracion incorrecta
- Crear nueva migracion corregida

### 2. Encoding en JavaScript
**Problema**: Caracteres especiales (ñ, á, é) se mostraban como símbolos raros.

**Solucion**: 
- Recrear archivos JavaScript con encoding UTF-8
- Evitar tildes en mensajes JavaScript
- Usar mensajes simples sin caracteres especiales

### 3. Alerts del Navegador
**Problema**: `confirm()` y `alert()` tienen mal UX y no se pueden estilizar.

**Solucion**:
- Crear modal de confirmacion generico Bootstrap
- Funcion `mostrarModalConfirmacion(titulo, mensaje, tipo, callback)`
- Colores dinamicos segun tipo
- Soporta HTML en mensajes

### 4. Filtros Ocupan Mucho Espacio
**Problema**: Filtros siempre visibles reducen espacio para tabla.

**Solucion**:
- Implementar cards colapsables en todos los modulos
- Usuarios, Roles y Cotizaciones ahora tienen filtros colapsables
- Boton de colapsar/expandir en header del card

---

## ARQUITECTURA DE VERSIONADO

### Flujo de Creacion de Version
```
1. Usuario crea cotizacion
   ?? Cotizacion(CotizacionId, VersionActual=1, Estado=B)
   ?? CotizacionVersion(NumeroVersion=1, VersionActual='1')
   ?? HistorialCotizacion(TipoEvento="Creada")

2. Usuario modifica y crea nueva version
   ?? CotizacionVersion anterior: VersionActual='0'
   ?? CotizacionVersion nueva: VersionActual='1', NumeroVersion=2
   ?? Cotizacion: VersionActual=2
   ?? HistorialCotizacion(TipoEvento="Version Generada")

3. Usuario duplica cotizacion
   ?? Nueva Cotizacion con nuevo CotizacionId
   ?? Nueva CotizacionVersion(NumeroVersion=1)
   ?? Copia solo detalles, sin cliente ni notas
```

### Reglas de Versionado
1. Solo una version puede ser actual ('1') por cotizacion
2. Al crear nueva version, la anterior se marca como historica ('0')
3. Se puede copiar cualquier version (actual o historica)
4. Duplicar crea una cotizacion completamente nueva

---

## ESTRUCTURA DE DATOS EN BD

### Cotizacion (Tabla Principal)
```sql
Id (PK, Identity)
CotizacionId (Unique, varchar(30)) -- COT-0001
InteresadoId (FK nullable)
EstadoActual (char) -- B/E/A/R/C/X
VersionActual (int) -- 1, 2, 3...
FechaCreacion (datetime2)
FechaUltimaActualizacion (datetime2 null)
MontoCotizacion (decimal(18,2))
FechaEnvio (datetime2 null)
CreatedAt, CreatedBy, ModifiedAt, ModifiedBy
```

### CotizacionVersion (Snapshot de Version)
```sql
VersionId (PK, Identity) ? Mapeado desde Id
CotizacionId (FK, varchar(30))
NumeroVersion (decimal(3,1))
FechaVersion (datetime2)
NombreInteresado (varchar(200))
EmailInteresado (varchar(150))
EmpresaInteresado (varchar(200))
SubTotal, Impuesto, Descuento, Total (decimal(18,2))
Moneda (varchar(3))
TipoCambio (decimal(18,2) null)
VersionActual (char) -- '1' actual, '0' historica
UsuarioCreacion (int null)
Notas (varchar(2000) null)
CreatedAt, CreatedBy, ModifiedAt, ModifiedBy

Index Unique: (CotizacionId, NumeroVersion)
```

### DetalleCotizacionVersion (Lineas de Productos)
```sql
DetalleVersionId (PK, Identity) ? Mapeado desde Id
VersionId (FK)
ProductoId (varchar(50))
Cantidad (decimal(18,2))
PrecioUnitario (decimal(18,2))
Descuento (decimal(18,2))
TotalLinea (decimal(18,2))
CreatedAt, CreatedBy, ModifiedAt, ModifiedBy
```

### HistorialCotizacion (Auditoria)
```sql
HistorialId (PK, Identity) ? Mapeado desde Id
VersionId (FK)
TipoEvento (varchar(50)) -- "Creada", "Enviada", etc.
FechaEvento (datetime2)
UsuarioEvento (int null)
Comentario (varchar(1000) null)
CreatedAt, CreatedBy, ModifiedAt, ModifiedBy

Index: FechaEvento
```

---

## DEBUGGING

Si algo no funciona:

### Backend
1. Verificar migracion aplicada: `SELECT * FROM CotizacionVersion`
2. Verificar que NO existen campos duplicados (VersionId1, etc.)
3. Verificar datos de prueba insertados correctamente
4. Verificar permisos asignados al rol del usuario
5. Verificar logs en `wwwroot/logs/log-YYYYMMDD.json`

### Frontend
1. Abrir consola del navegador (F12)
2. Verificar que no hay errores JavaScript
3. Verificar que modal de confirmacion se muestra correctamente
4. Verificar que filtros funcionan
5. Verificar encoding de caracteres (sin simbolos raros)

### Base de Datos
```sql
-- Ver estructura de CotizacionVersion
EXEC sp_help 'CotizacionVersion';

-- Debe mostrar:
-- VersionId (PK, Identity) ? CORRECTO
-- Sin VersionId1 ? CORRECTO

-- Verificar datos
SELECT * FROM Cotizacion;
SELECT * FROM CotizacionVersion;
SELECT * FROM DetalleCotizacionVersion;
SELECT * FROM HistorialCotizacion;
```

---

## DECISIONES DE DISENO

1. **IDs autonumericos**: Todos los Id son Identity en BD
2. **Mapeo de nombres**: Id de entidad ? NombreId en BD (via Fluent API)
3. **Sin propiedades duplicadas**: Solo Id de BaseEntity
4. **Modales en lugar de alerts**: Mejor UX y control de estilos
5. **Sin tildes en JS**: Evita problemas de encoding
6. **Filtros colapsables**: Mas espacio para datos
7. **Botones coloreados**: Facilita identificacion rapida de acciones
8. **Timeline para historial**: Visualizacion cronologica clara

---

## SIGUIENTES PASOS RECOMENDADOS

### Fase 2: Detalle y Edicion
1. Implementar modal "Ver Detalle" de version
2. Crear formulario de edicion de cotizacion
3. Validaciones de negocio completas
4. Calculos automaticos de totales

### Fase 3: Workflow de Estados
1. Acciones de cambio de estado (Enviar, Aprobar, Rechazar, etc.)
2. Validaciones de transiciones permitidas
3. Notificaciones por email
4. Integracion con HubSpot

### Fase 4: Integraciones
1. Sincronizacion con HubSpot
2. Envio a ERP
3. Generacion de PDF
4. Exportacion a Excel

---

**Version**: 2.0  
**Ultima actualizacion**: 12 de marzo de 2026, 11:15 PM  
**Autor**: Marcela Jimenez (con GitHub Copilot)  
**Estado**: Modulo base funcional con UI moderna
