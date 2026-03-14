# LINEAMIENTOS FUNCIONALES Y DE DOMINIO - MÓDULO COTIZACIONES

**Fecha**: 14 de marzo de 2026  
**Versión**: 2.0  
**Autor**: Marcela Jiménez

---

## ?? ESTADOS DE LA COTIZACIÓN

### Estados Válidos

Los estados válidos de la cotización son:

| Código | Estado | Descripción |
|--------|--------|-------------|
| **B** | Borrador | Cotización en construcción, puede modificarse |
| **P** | PendienteAprobacion | Enviada para revisión y aprobación interna |
| **A** | Aprobada | Aprobada internamente, lista para envío al cliente |
| **E** | Enviada | Enviada al cliente, esperando respuesta |
| **T** | Aceptada | Cliente aceptó la cotización |
| **R** | Rechazada | Cliente rechazó la cotización |
| **X** | Archivada | Estado terminal administrativo |

### Reglas Importantes

- ? Los estados **T (Aceptada)** y **R (Rechazada)** representan el resultado final de una cotización enviada al cliente
- ? **X (Archivada)** es un estado terminal administrativo
- ? Solo puede existir un estado actual por cotización
- ? El estado debe aplicarse siempre sobre la versión actual vigente de la cotización

---

## ?? FLUJOS VÁLIDOS

### Flujo Principal - Cotización Aceptada
```
Borrador ? PendienteAprobacion ? Aprobada ? Enviada ? Aceptada
```

### Flujo Principal - Cotización Rechazada
```
Borrador ? PendienteAprobacion ? Aprobada ? Enviada ? Rechazada
```

### Flujos Adicionales Permitidos
```
Borrador ? Archivada
Aceptada ? Archivada
Rechazada ? Archivada
PendienteAprobacion ? Borrador (requiere nota obligatoria)
```

---

## ? REGLAS DE TRANSICIÓN

### 1. **Borrador (B)**
- ? Puede pasar a **PendienteAprobacion**
- ? Puede pasar a **Archivada**

### 2. **PendienteAprobacion (P)**
- ? Puede pasar a **Aprobada**
- ? Puede volver a **Borrador**
- ?? **REGLA ESPECIAL**: Si vuelve a Borrador, es **obligatorio** registrar una nota explicando el motivo

### 3. **Aprobada (A)**
- ? Puede pasar a **Enviada**

### 4. **Enviada (E)**
- ? Puede pasar a **Aceptada**
- ? Puede pasar a **Rechazada**

### 5. **Aceptada (T)**
- ? Puede pasar a **Archivada**

### 6. **Rechazada (R)**
- ? Puede pasar a **Archivada**

### 7. **Archivada (X)**
- ?? **No debe permitir transiciones posteriores**, salvo que en el futuro se defina una regla explícita de reactivación

---

## ?? REGLA ESPECIAL - VUELTA A BORRADOR

### Cuando PendienteAprobacion ? Borrador:
- ? Se debe **solicitar una nota obligatoria**
- ? La nota debe quedar **registrada en historial**
- ? La nota debe **asociarse a la versión actual** sobre la cual ocurrió el cambio
- ? El tipo de evento debe ser **"DevueltaABorrador"**

---

## ??? CAMBIOS DE ESTRUCTURA EN COTIZACIÓN

### Nuevos Campos a Agregar

| Campo | Tipo | Descripción | Reglas |
|-------|------|-------------|---------|
| **FechaAceptacion** | `datetime null` | Fecha cuando se marcó como aceptada | Solo cuando estado = T |
| **FechaRechazo** | `datetime null` | Fecha cuando se marcó como rechazada | Solo cuando estado = R |
| **EnviadoERP** | `varchar(1)` | Indica si fue enviada al ERP | Valores: S/N, Default: N |
| **FechaEnvioERP** | `datetime null` | Fecha de envío al ERP | Solo cuando EnviadoERP = S |

### Reglas para Nuevos Campos

#### **FechaAceptacion**
- ? Debe establecerse cuando el estado cambie a **Aceptada (T)**
- ? Debe permanecer **null** en cualquier otro estado

#### **FechaRechazo**
- ? Debe establecerse cuando el estado cambie a **Rechazada (R)**
- ? Debe permanecer **null** en cualquier otro estado

#### **EnviadoERP**
- ? Valores válidos: **S** o **N**
- ? Valor por defecto recomendado: **N**
- ? Indica si la cotización ya fue enviada al ERP

#### **FechaEnvioERP**
- ? Debe establecerse cuando **EnviadoERP** cambie a **S**
- ? Debe permanecer **null** mientras **EnviadoERP** sea **N**

---

## ?? REGLAS DE CONSISTENCIA

### Validaciones Obligatorias

| Condición | Regla |
|-----------|-------|
| `EstadoActual = T` | `FechaAceptacion` debe tener valor |
| `EstadoActual = R` | `FechaRechazo` debe tener valor |
| `EnviadoERP = S` | `FechaEnvioERP` debe tener valor |
| `EnviadoERP = N` | `FechaEnvioERP` debe ser null |

### Reglas de Negocio ERP

- ?? Una cotización **no debe enviarse al ERP más de una vez** salvo que exista una regla explícita de reenvío
- ? La cotización debe estar al menos en estado **Aprobada** antes de poder marcarse como enviada al ERP
- ?? El envío al ERP puede ocurrir antes o después de **Enviada** al cliente; por ahora no asumir automatismo

---

## ?? IMPACTO EN HISTORIAL

### Eventos Funcionales Requeridos

| Evento | Código | Cuándo Registrar |
|--------|--------|------------------|
| **Creada** | "Creada" | Al crear nueva cotización |
| **VersionGenerada** | "VersionGenerada" | Al copiar/crear nueva versión |
| **EnviadaAProbacion** | "EnviadaAProbacion" | Borrador ? PendienteAprobacion |
| **DevueltaABorrador** | "DevueltaABorrador" | PendienteAprobacion ? Borrador |
| **Aprobada** | "Aprobada" | PendienteAprobacion ? Aprobada |
| **EnviadaCliente** | "EnviadaCliente" | Aprobada ? Enviada |
| **AceptadaCliente** | "AceptadaCliente" | Enviada ? Aceptada |
| **RechazadaCliente** | "RechazadaCliente" | Enviada ? Rechazada |
| **Archivada** | "Archivada" | Cualquier ? Archivada |
| **EnviadaERP** | "EnviadaERP" | Al marcar EnviadoERP = S |

### Regla Especial para DevueltaABorrador

Cuando una cotización pase de **PendienteAprobacion** a **Borrador**:
- ? Registrar evento **"DevueltaABorrador"**
- ? Guardar **comentario obligatorio** proporcionado por el usuario
- ? El campo `Comentario` del historial debe contener la nota explicativa

---

## ??? IMPACTO EN PANTALLA PRINCIPAL

### Listado de Cotizaciones

La pantalla principal debe:
- ? Listar las cotizaciones mostrando la **versión actual vigente** y su **estado actual**
- ? Mostrar los nuevos campos de fecha cuando corresponda

### Filtros por Estado

Los filtros deben soportar los nuevos estados:
- ? **Borrador**
- ? **Pendiente Aprobación**
- ? **Aprobada**
- ? **Enviada**
- ? **Aceptada**
- ? **Rechazada**

?? **IMPORTANTE**: El estado **Archivada** **NO** debe presentarse en el mantenimiento de cotizaciones ya que esto tendrá otra opción dentro del sistema.

---

## ?? REGLAS PARA ACCIONES SEGÚN ESTADO

### Matriz de Acciones Permitidas

| Acción | Estados Permitidos | Observaciones |
|--------|--------------------|---------------|
| **Copiar** | Cualquiera con versión actual | Genera nueva versión dentro de la misma cotización |
| **Duplicar** | Cualquiera | Crea nueva cotización independiente en Borrador |
| **Enviar a aprobación** | **B** solamente | B ? P |
| **Aprobar** | **P** solamente | P ? A |
| **Devolver a borrador** | **P** solamente | P ? B, requiere nota obligatoria |
| **Enviar al cliente** | **A** solamente | A ? E |
| **Marcar como aceptada** | **E** solamente | E ? T |
| **Marcar como rechazada** | **E** solamente | E ? R |
| **Archivar** | **B, T, R** | ? X, requiere confirmación |
| **Enviar al ERP** | **A, E, T** mínimo | Solo si cumple reglas de negocio |

### Confirmaciones Requeridas

- ? **Archivar**: Debe solicitar confirmación por parte del usuario
- ? **Devolver a borrador**: Debe solicitar nota obligatoria
- ? **Enviar al ERP**: Debe solicitar confirmación

---

## ??? CASOS DE USO SUGERIDOS PARA APPLICATION

### Servicios Separados por Caso de Uso

Crear servicios específicos en la capa **Application**:

```csharp
public interface ISendQuoteToApprovalService
{
    Task<Result> SendToApprovalAsync(string cotizacionId);
}

public interface IReturnQuoteToDraftService
{
    Task<Result> ReturnToDraftAsync(string cotizacionId, string nota);
}

public interface IApproveQuoteService
{
    Task<Result> ApproveAsync(string cotizacionId);
}

public interface ISendQuoteToCustomerService
{
    Task<Result> SendToCustomerAsync(string cotizacionId);
}

public interface IAcceptQuoteService
{
    Task<Result> AcceptAsync(string cotizacionId);
}

public interface IRejectQuoteService
{
    Task<Result> RejectAsync(string cotizacionId);
}

public interface IArchiveQuoteService
{
    Task<Result> ArchiveAsync(string cotizacionId);
}

public interface IMarkQuoteAsSentToErpService
{
    Task<Result> MarkAsSentToErpAsync(string cotizacionId);
}
```

### Responsabilidades Esperadas

Cada servicio debe:
- ? **Validar transición de estado** según matriz permitida
- ? **Actualizar campos relacionados** (fechas, flags)
- ? **Registrar historial** con evento correspondiente
- ?? **NO colocar esta lógica en Controllers**

---

## ?? ENUM SUGERIDO PARA ESTADO EN DOMAIN

### Actualización de EstadoCotizacion

```csharp
namespace CotizacionesWeb.Domain.Enums;

public enum EstadoCotizacion
{
    Borrador = 'B',
    PendienteAprobacion = 'P',
    Aprobada = 'A',
    Enviada = 'E',
    Aceptada = 'T',
    Rechazada = 'R',
    Archivada = 'X'
}
```

---

## ?? LINEAMIENTOS PARA COPILOT

### ? Reglas Obligatorias

1. **Respetar estrictamente las transiciones de estado definidas**
2. **No permitir cambios de estado fuera de los flujos válidos**
3. **Implementar validaciones de transición en la capa Application**
4. **Registrar historial en cada cambio de estado relevante**
5. **Exigir nota obligatoria al regresar de PendienteAprobacion a Borrador**
6. **Mantener consistencia entre EstadoActual y las fechas FechaAceptacion / FechaRechazo**
7. **Mantener consistencia entre EnviadoERP y FechaEnvioERP**

### ?? Restricciones

1. **No poner lógica de transición de estados en Controllers**
2. **Controllers solo coordinan request/response y llaman servicios de Application**
3. **No permitir transiciones desde estado Archivada**
4. **No mostrar estado Archivada en filtros del mantenimiento**

---

## ?? IMPLEMENTACIÓN TÉCNICA

### Base de Datos - Nuevos Campos

```sql
ALTER TABLE Cotizacion ADD
    FechaAceptacion datetime2 NULL,
    FechaRechazo datetime2 NULL,
    EnviadoERP varchar(1) NOT NULL DEFAULT 'N',
    FechaEnvioERP datetime2 NULL;

-- Constraint para EnviadoERP
ALTER TABLE Cotizacion ADD CONSTRAINT CK_Cotizacion_EnviadoERP 
    CHECK (EnviadoERP IN ('S', 'N'));
```

### Validaciones en Application

```csharp
public class StateTransitionValidator
{
    private static readonly Dictionary<char, char[]> AllowedTransitions = new()
    {
        { 'B', new[] { 'P', 'X' } },           // Borrador ? PendienteAprobacion, Archivada
        { 'P', new[] { 'A', 'B' } },           // PendienteAprobacion ? Aprobada, Borrador
        { 'A', new[] { 'E' } },                // Aprobada ? Enviada
        { 'E', new[] { 'T', 'R' } },           // Enviada ? Aceptada, Rechazada
        { 'T', new[] { 'X' } },                // Aceptada ? Archivada
        { 'R', new[] { 'X' } },                // Rechazada ? Archivada
        { 'X', new char[0] }                   // Archivada ? Ninguno
    };

    public bool IsTransitionAllowed(char fromState, char toState)
    {
        return AllowedTransitions.ContainsKey(fromState) && 
               AllowedTransitions[fromState].Contains(toState);
    }
}
```

### Actualización de Campos por Estado

```csharp
public void UpdateStateFields(Cotizacion cotizacion, char newState)
{
    switch (newState)
    {
        case 'T': // Aceptada
            cotizacion.FechaAceptacion = DateTime.Now;
            cotizacion.FechaRechazo = null;
            break;
        case 'R': // Rechazada
            cotizacion.FechaRechazo = DateTime.Now;
            cotizacion.FechaAceptacion = null;
            break;
        default:
            // Otros estados no afectan estas fechas
            break;
    }
    cotizacion.EstadoActual = newState;
}
```

---

## ?? PRÓXIMOS PASOS

### Fase 1: Actualización de Estructura
1. ? Actualizar enum `EstadoCotizacion`
2. ? Agregar nuevos campos a entidad `Cotizacion`
3. ? Crear migración de base de datos
4. ? Actualizar configuraciones EF Core

### Fase 2: Servicios de Transición
1. ? Implementar servicios de Application para cada transición
2. ? Implementar validador de transiciones
3. ? Actualizar `TipoEvento` con nuevos eventos
4. ? Crear tests unitarios para validaciones

### Fase 3: UI y Acciones
1. ? Actualizar pantalla principal con nuevos estados
2. ? Agregar botones de acción según estado actual
3. ? Implementar modales para notas obligatorias
4. ? Actualizar filtros de estado

### Fase 4: Integraciones
1. ? Implementar envío al ERP
2. ? Notificaciones por cambio de estado
3. ? Auditoría completa de transiciones
4. ? Reportes por estado

---

## ?? CONSIDERACIONES IMPORTANTES

### Migración de Datos Existentes

Al implementar los nuevos estados:
- ? Las cotizaciones existentes mantendrán su estado actual
- ? Los nuevos campos tendrán valores por defecto apropiados
- ? No se requiere migración de datos complejos

### Retrocompatibilidad

- ? Los estados existentes (B, E, A, R, C, X) siguen siendo válidos
- ? Se agregan nuevos estados (P, T) sin afectar los existentes
- ? El estado C (Cancelada) puede mantenerse para retrocompatibilidad

### Performance

- ? Las validaciones de transición son operaciones en memoria
- ? No impactan significativamente el rendimiento
- ? Los índices existentes siguen siendo efectivos

---

**Versión**: 2.0  
**Última actualización**: 14 de marzo de 2026  
**Próxima revisión**: Al implementar los cambios
