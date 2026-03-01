# CotizacionesWeb
Proyecto Tesis-Marcela Jiménez

Backoffice interno de cotizaciones construido con ASP.NET Core MVC en .NET 8.

---

## Arquitectura

La solución sigue una arquitectura por capas separadas en proyectos:

```
CotizacionesWeb/
├── src/
│   ├── CotizacionesWeb.Domain          # Entidades, enums, clases base
│   ├── CotizacionesWeb.Application     # Servicios por caso de uso, interfaces
│   ├── CotizacionesWeb.Infrastructure  # EF Core, autenticación, integraciones
│   └── CotizacionesWeb.UI              # ASP.NET Core MVC
└── tests/
    └── CotizacionesWeb.Tests           # xUnit
```

**Reglas de dependencia:**
- `UI` → `Application`
- `Application` → `Domain`
- `Infrastructure` → `Application` + `Domain`
- `Domain` → no depende de nadie

---

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server Express (desarrollo)
- Herramienta EF Core: `dotnet tool install --global dotnet-ef --version 8.0.14`

---

## Cómo ejecutar

1. Clona el repositorio:
   ```bash
   git clone https://github.com/LunaMar24/CotizacionesWeb.git
   cd CotizacionesWeb
   ```

2. Configura la cadena de conexión en `src/CotizacionesWeb.UI/appsettings.json` (o usa `appsettings.Development.json`):
   ```json
   "ConnectionStrings": {
     "CotizacionesDb": "Server=(localdb)\\mssqllocaldb;Database=CotizacionesWebDev;Trusted_Connection=True;"
   }
   ```

3. Aplica las migraciones (ver sección siguiente).

4. Ejecuta la aplicación:
   ```bash
   dotnet run --project src/CotizacionesWeb.UI
   ```

---

## Cómo crear una migración

```bash
dotnet ef migrations add <NombreMigracion> \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones \
  --output-dir Data/Migrations
```

---

## Cómo aplicar migraciones (desarrollo local)

```bash
dotnet ef database update \
  --project src/CotizacionesWeb.Infrastructure \
  --startup-project src/CotizacionesWeb.UI \
  --context DbContextCotizaciones
```

---

## Stack

- .NET 8 / ASP.NET Core MVC
- Bootstrap (UI)
- Entity Framework Core Code First
- Azure SQL Database (prod) / SQL Server Express (dev)
- Autenticación propia con Cookies
- Autorización por Roles
- Serilog (logs diarios en archivo JSON)
