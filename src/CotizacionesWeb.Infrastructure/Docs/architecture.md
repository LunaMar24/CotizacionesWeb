# Lineamientos Técnicos - Arquitectura General

## Estilo Arquitectónico
Monolito modular con separación por capas.

Capas:
- UI (ASP.NET Core MVC)
- Application (casos de uso)
- Domain (entidades)
- Infrastructure (datos e integraciones)

Reglas:
- Controllers no contienen lógica de negocio.
- Domain no depende de otras capas.
- Infrastructure implementa contratos de Application.

## Estructura
src/
 ├── CotizacionesWeb.UI
 ├── CotizacionesWeb.Application
 ├── CotizacionesWeb.Domain
 └── CotizacionesWeb.Infrastructure

## Acceso a datos
- EF Core Code First
- Dos DbContext:
  - DbContextCotizaciones
  - DbContextErp

No usar transacciones distribuidas.

## Integraciones
Servicios:
- IErpService
- IHubSpotService

Implementaciones en Infrastructure.

## Auditoría
BaseEntity:
- CreatedAt
- CreatedBy
- ModifiedAt
- ModifiedBy