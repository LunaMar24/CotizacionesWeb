# CotizacionesWeb

Backoffice interno para la gestion de cotizaciones de Aplix S.A., construido con ASP.NET Core MVC sobre .NET 8.

Este repositorio contiene el codigo fuente de la aplicacion, pruebas automatizadas y documentacion tecnica y funcional del sistema.

> La documentacion oficial del proyecto se encuentra en [`/docs`](docs/README.md).

---

## Proposito del sistema

CotizacionesWeb permite gestionar el ciclo de vida de una cotizacion comercial, desde su creacion inicial hasta su aprobacion, envio al cliente, aceptacion o rechazo, archivo e integracion con sistemas externos.

El sistema incluye:

- Gestion de cotizaciones, versiones, detalle e historial.
- Flujo de estados de cotizacion.
- Gestion de usuarios, roles y permisos.
- Parametros configurables del sistema.
- Generacion de documentos Word de cotizacion.
- Notificaciones por correo electronico.
- Integracion con HubSpot.
- Integracion con ERP.
- Procesos en segundo plano para notificaciones e integracion.

---

## Estructura general del repositorio

```text
CotizacionesWeb/
|-- README.md
|-- docs/
|   |-- README.md
|   |-- funcional/
|   |-- tecnico/
|   |-- usuario/
|   |-- ia/
|   `-- anexos/
|-- src/
|   |-- CotizacionesWeb.Domain/
|   |-- CotizacionesWeb.Application/
|   |-- CotizacionesWeb.Infrastructure/
|   `-- CotizacionesWeb.UI/
`-- tests/
    `-- CotizacionesWeb.Tests/
```

---

## Arquitectura

La solucion sigue una arquitectura por capas:

| Proyecto | Responsabilidad |
|---|---|
| `CotizacionesWeb.Domain` | Entidades, enums y elementos centrales del dominio. |
| `CotizacionesWeb.Application` | Contratos, DTOs e interfaces de casos de uso. |
| `CotizacionesWeb.Infrastructure` | EF Core, servicios, seguridad, integraciones, documentos y procesos en segundo plano. |
| `CotizacionesWeb.UI` | Aplicacion ASP.NET Core MVC, controladores, vistas, filtros, helpers y configuracion web. |
| `CotizacionesWeb.Tests` | Pruebas automatizadas. |

Reglas de dependencia principales:

- `UI` consume `Application`.
- `Application` depende de `Domain`.
- `Infrastructure` implementa contratos de `Application` y utiliza `Domain`.
- `Domain` no debe depender de otras capas.

Para mas detalle, consultar [`docs/tecnico/arquitectura.md`](docs/tecnico/arquitectura.md).

---

## Stack tecnologico

- .NET 8
- ASP.NET Core MVC
- Entity Framework Core 8
- SQL Server / Azure SQL
- Autenticacion con cookies
- Autorizacion por roles y permisos
- Serilog
- Bootstrap / AdminLTE / jQuery
- OpenXML para generacion de documentos Word
- xUnit para pruebas

---

## Requisitos previos

- .NET 8 SDK
- SQL Server o Azure SQL
- Herramienta de EF Core CLI:

```bash
dotnet tool install --global dotnet-ef --version 8.0.14
```

---

## Configuracion inicial

La aplicacion utiliza archivos de configuracion en:

```text
src/CotizacionesWeb.UI/appsettings.json
src/CotizacionesWeb.UI/appsettings.Development.json
src/CotizacionesWeb.UI/appsettings.Production.json
```

Configuraciones principales:

- `ConnectionStrings:CotizacionesDb`
- `ConnectionStrings:ErpDb`
- `Email`
- Serilog
- Parametros de integraciones cuando apliquen

Consultar [`docs/tecnico/configuracion-ejecucion.md`](docs/tecnico/configuracion-ejecucion.md).

---

## Ejecucion local

```bash
dotnet restore
dotnet build
dotnet ef database update \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
dotnet run --project src/CotizacionesWeb.UI
```

Para mas detalles de migraciones y base de datos, consultar [`docs/tecnico/base-datos-migraciones.md`](docs/tecnico/base-datos-migraciones.md).

---

## Documentacion

La documentacion oficial esta en [`/docs`](docs/README.md).

Puntos de entrada recomendados:

- [`docs/README.md`](docs/README.md): indice principal de documentacion.
- [`docs/funcional/vision-general.md`](docs/funcional/vision-general.md): descripcion funcional del sistema.
- [`docs/tecnico/arquitectura.md`](docs/tecnico/arquitectura.md): arquitectura tecnica.
- [`docs/usuario/manual-usuario.md`](docs/usuario/manual-usuario.md): manual de usuario.
- [`docs/ia/contexto-copilot.md`](docs/ia/contexto-copilot.md): contexto reutilizable para IA/Copilot.

---

## Modulos principales

| Modulo | Descripcion |
|---|---|
| Cotizaciones | Gestion del ciclo de vida de cotizaciones. |
| Usuarios | Administracion de usuarios del sistema. |
| Roles y permisos | Control de acceso por permisos funcionales. |
| Parametros | Configuracion funcional y tecnica desde el sistema. |
| Documentos | Generacion y descarga de documentos Word. |
| Notificaciones | Envio de correos y procesamiento en segundo plano. |
| HubSpot | Busqueda/asignacion de interesados y datos relacionados. |
| ERP | Integracion de pedidos o cotizaciones aceptadas hacia ERP. |

---

## Pruebas

```bash
dotnet test
```

La solucion incluye pruebas unitarias en:

```text
tests/CotizacionesWeb.Tests/
```

