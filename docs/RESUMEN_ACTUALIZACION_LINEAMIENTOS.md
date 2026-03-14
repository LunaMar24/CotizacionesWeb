# RESUMEN DE ACTUALIZACIONES - LINEAMIENTOS FUNCIONALES COTIZACIONES

**Fecha**: 14 de marzo de 2026  
**Tipo**: Actualización de lineamientos funcionales y de dominio

---

## ?? ARCHIVOS ACTUALIZADOS

### 1. **docs/COTIZACIONES_DOMINIO_FUNCIONAL.md** *(NUEVO)*
- ? **Creado**: Documento completo con lineamientos funcionales
- ? **Estados**: Definición de 7 estados válidos (B/P/A/E/T/R/X)
- ? **Flujos**: Mapeo completo de transiciones permitidas
- ? **Reglas**: 15+ reglas de negocio detalladas
- ? **Estructura**: Nuevos campos para BD (FechaAceptacion, FechaRechazo, etc.)
- ? **Casos de uso**: 8 servicios sugeridos para Application
- ? **Implementación**: Ejemplos de código y validaciones

### 2. **docs/README.md** *(ACTUALIZADO)*
- ? **Agregado**: Referencias al nuevo documento COTIZACIONES_DOMINIO_FUNCIONAL.md
- ? **Actualizado**: Sección de documentos disponibles
- ? **Actualizado**: Estado del proyecto (módulos implementados/pendientes)
- ? **Actualizado**: Fecha de última modificación

### 3. **docs/COTIZACIONES_IMPLEMENTACION.md** *(ACTUALIZADO)*
- ? **Agregado**: Referencia a los nuevos lineamientos
- ? **Nota importante**: Indicación para consultar el nuevo documento funcional

### 4. **src/CotizacionesWeb.Domain/Enums/EstadoCotizacion.cs** *(ACTUALIZADO)*
- ? **Agregado**: `PendienteAprobacion = 'P'`
- ? **Agregado**: `Aceptada = 'T'`
- ? **Removido**: `Cancelada = 'C'` (deprecado en nuevos lineamientos)

### 5. **src/CotizacionesWeb.Domain/Enums/TipoEvento.cs** *(ACTUALIZADO)*
- ? **Agregados nuevos eventos**:
  - `EnviadaAProbacion`
  - `DevueltaABorrador`
  - `EnviadaCliente`
  - `AceptadaCliente`
  - `RechazadaCliente`
  - `EnviadaERP`
- ? **Mantenidos eventos legacy** para retrocompatibilidad
- ? **Organizados por categorías** con comentarios

---

## ?? NUEVOS LINEAMIENTOS IMPLEMENTADOS

### Estados y Transiciones
- ? **7 estados válidos** definidos claramente
- ? **Matriz de transiciones** con reglas estrictas
- ? **Flujos principales**: Aceptada y Rechazada
- ? **Flujos alternativos**: Archivado desde múltiples estados

### Reglas de Negocio
- ? **Nota obligatoria** para PendienteAprobacion ? Borrador
- ? **Fechas automáticas** para Aceptacion y Rechazo
- ? **Control ERP** con flags y fechas
- ? **Validaciones de consistencia** entre campos

### Arquitectura Sugerida
- ? **Servicios por caso de uso** en Application
- ? **Validador de transiciones** reutilizable
- ? **Separación de responsabilidades**: Controllers solo coordinan
- ? **Eventos de historial** granulares y descriptivos

---

## ?? CAMBIOS EN BASE DE DATOS (PENDIENTES)

### Nuevos Campos en Cotización
```sql
ALTER TABLE Cotizacion ADD
    FechaAceptacion datetime2 NULL,
    FechaRechazo datetime2 NULL,
    EnviadoERP varchar(1) NOT NULL DEFAULT 'N',
    FechaEnvioERP datetime2 NULL;
```

### Constraints Sugeridos
```sql
ALTER TABLE Cotizacion ADD CONSTRAINT CK_Cotizacion_EnviadoERP 
    CHECK (EnviadoERP IN ('S', 'N'));
```

---

## ?? PRÓXIMOS PASOS SUGERIDOS

### Fase 1: Estructura (Inmediata)
1. ? Agregar nuevos campos a entidad Cotización
2. ? Crear migración de base de datos
3. ? Actualizar configuraciones EF Core
4. ? Verificar retrocompatibilidad

### Fase 2: Servicios (1-2 días)
1. ? Implementar validador de transiciones
2. ? Crear servicios de Application por caso de uso
3. ? Implementar lógica de negocio
4. ? Tests unitarios para validaciones

### Fase 3: UI (2-3 días)
1. ? Actualizar filtros con nuevos estados
2. ? Agregar botones de acción por estado
3. ? Modales para notas obligatorias
4. ? Validaciones en frontend

### Fase 4: Testing (1 día)
1. ? Pruebas de transiciones
2. ? Validación de reglas de negocio
3. ? Pruebas de UI
4. ? Verificación de performance

---

## ?? CONSIDERACIONES IMPORTANTES

### Retrocompatibilidad
- ? **Estados existentes** siguen funcionando
- ? **Eventos legacy** se mantienen
- ? **No se rompe** funcionalidad actual
- ? **Migración gradual** posible

### Performance
- ? **Validaciones en memoria** (no impacto en BD)
- ? **Índices existentes** siguen siendo válidos
- ? **Queries optimizadas** mantienen rendimiento
- ? **Campos nuevos nullable** (no requiere datos históricos)

### Seguridad
- ? **Validaciones server-side** obligatorias
- ? **Permisos por acción** según estado
- ? **Historial inmutable** de cambios
- ? **Trazabilidad completa** de transiciones

---

## ?? RECURSOS PARA IMPLEMENTACIÓN

### Documentos de Referencia
- **Principal**: `docs/COTIZACIONES_DOMINIO_FUNCIONAL.md`
- **Técnico**: `docs/COTIZACIONES_IMPLEMENTACION.md`
- **Contexto**: `docs/CONTEXTO_PROYECTO.md`

### Enums Actualizados
- `EstadoCotizacion`: 7 estados válidos
- `TipoEvento`: 11 eventos (6 nuevos + 5 legacy)

### Validaciones Base
```csharp
// Estado actual ? Estados permitidos
var allowedTransitions = new Dictionary<char, char[]>
{
    { 'B', new[] { 'P', 'X' } },        // Borrador
    { 'P', new[] { 'A', 'B' } },        // PendienteAprobacion
    { 'A', new[] { 'E' } },             // Aprobada
    { 'E', new[] { 'T', 'R' } },        // Enviada
    { 'T', new[] { 'X' } },             // Aceptada
    { 'R', new[] { 'X' } },             // Rechazada
    { 'X', new char[0] }                // Archivada
};
```

---

## ?? BENEFICIOS DE LA ACTUALIZACIÓN

### Para el Negocio
- ? **Flujo de aprobación** interno estructurado
- ? **Trazabilidad completa** de decisiones
- ? **Control de envíos** al ERP
- ? **Reportería** por estado granular

### Para Desarrollo
- ? **Arquitectura limpia** por casos de uso
- ? **Validaciones centralizadas** reutilizables
- ? **Código mantenible** y testeable
- ? **Escalabilidad** para futuras reglas

### Para Usuario Final
- ? **Interfaz intuitiva** con acciones contextuales
- ? **Validaciones claras** y mensajes útiles
- ? **Flujo guiado** sin pasos confusos
- ? **Historial detallado** de cambios

---

**Estado**: ? Documentación actualizada, código base preparado  
**Próximo hito**: Implementación de nuevos campos y servicios  
**Responsable**: Equipo de desarrollo  
**Fecha estimada**: 17-20 de marzo de 2026