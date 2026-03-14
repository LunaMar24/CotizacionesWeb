using CotizacionesWeb.Application.Cotizaciones;
using CotizacionesWeb.Domain.Entities;
using CotizacionesWeb.Domain.Enums;
using CotizacionesWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Infrastructure.Services;

public class CotizacionService : ICotizacionService
{
    private readonly DbContextCotizaciones _context;
    private readonly ILogger<CotizacionService> _logger;

    public CotizacionService(
        DbContextCotizaciones context,
        ILogger<CotizacionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<CotizacionListDto>> GetCotizacionesListAsync(GetCotizacionesListRequest request)
    {
        var query = _context.Cotizaciones
            .Include(c => c.Versiones.Where(v => v.VersionActual == '1'))
            .AsQueryable();

        // Filtrar por estados si se proporcionan
        if (request.Estados != null && request.Estados.Any())
        {
            query = query.Where(c => request.Estados.Contains(c.EstadoActual));
        }

        // Filtrar por búsqueda general (CotizacionId, Nombre, Empresa)
        if (!string.IsNullOrWhiteSpace(request.Busqueda))
        {
            var busqueda = request.Busqueda.ToLower();
            query = query.Where(c =>
                c.CotizacionId.ToLower().Contains(busqueda) ||
                c.Versiones.Any(v => v.VersionActual == '1' && 
                    (v.NombreInteresado.ToLower().Contains(busqueda) ||
                     v.EmpresaInteresado.ToLower().Contains(busqueda)))
            );
        }

        // Filtrar por rango de fechas
        if (request.FechaDesde.HasValue)
        {
            var fechaDesdeInicioDia = request.FechaDesde.Value.Date; // 00:00:00
            query = query.Where(c => c.CreatedAt >= fechaDesdeInicioDia);
        }

        if (request.FechaHasta.HasValue)
        {
            var fechaHastaFinDia = request.FechaHasta.Value.Date.AddDays(1).AddTicks(-1); // 23:59:59.999
            query = query.Where(c => c.CreatedAt <= fechaHastaFinDia);
        }

        var cotizaciones = await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return cotizaciones.Select(c =>
        {
            var versionActual = c.Versiones.FirstOrDefault(v => v.VersionActual == '1');
            return new CotizacionListDto(
                c.Id,
                c.CotizacionId,
                c.InteresadoId,
                versionActual?.NombreInteresado ?? "",
                versionActual?.EmpresaInteresado ?? "",
                c.EstadoActual,
                c.VersionActual,
                c.CreatedAt, // Using CreatedAt instead of FechaCreacion
                c.ModifiedAt, // Using ModifiedAt instead of FechaUltimaActualizacion
                c.MontoCotizacion,
                c.FechaEnvio,
                // Nuevos campos según lineamientos funcionales
                c.FechaAceptacion,
                c.FechaRechazo,
                c.EnviadoERP,
                c.FechaEnvioERP
            );
        }).ToList();
    }

    public async Task<List<HistorialCotizacionDto>> GetCotizacionCurrentHistoryAsync(string cotizacionId)
    {
        var cotizacion = await _context.Cotizaciones
            .Include(c => c.Versiones.Where(v => v.VersionActual == '1'))
                .ThenInclude(v => v.Historiales)
            .FirstOrDefaultAsync(c => c.CotizacionId == cotizacionId);

        if (cotizacion == null)
        {
            return new List<HistorialCotizacionDto>();
        }

        var versionActual = cotizacion.Versiones.FirstOrDefault(v => v.VersionActual == '1');
        if (versionActual == null)
        {
            return new List<HistorialCotizacionDto>();
        }

        var historiales = versionActual.Historiales
            .OrderByDescending(h => h.FechaEvento)
            .ToList();

        var usuariosIds = historiales
            .Where(h => h.UsuarioEvento.HasValue)
            .Select(h => h.UsuarioEvento!.Value)
            .Distinct()
            .ToList();

        var usuarios = await _context.Usuarios
            .Where(u => usuariosIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Nombre);

        return historiales.Select(h => new HistorialCotizacionDto(
            h.Id,
            h.VersionId,
            h.TipoEvento,
            h.FechaEvento,
            h.UsuarioEvento,
            h.UsuarioEvento.HasValue && usuarios.ContainsKey(h.UsuarioEvento.Value)
                ? usuarios[h.UsuarioEvento.Value]
                : null,
            h.Comentario
        )).ToList();
    }

    public async Task<List<CotizacionVersionDto>> GetCotizacionVersionsAsync(string cotizacionId)
    {
        var versiones = await _context.CotizacionesVersiones
            .Where(v => v.CotizacionId == cotizacionId)
            .OrderByDescending(v => v.NumeroVersion)
            .ToListAsync();

        return versiones.Select(v => new CotizacionVersionDto(
            v.Id,
            v.CotizacionId,
            v.NumeroVersion,
            v.FechaVersion,
            v.NombreInteresado,
            v.EmailInteresado,
            v.EmpresaInteresado,
            v.SubTotal,
            v.Impuesto,
            v.Descuento,
            v.Total,
            v.Moneda,
            v.TipoCambio,
            v.VersionActual,
            v.Notas
        )).ToList();
    }

    public async Task<CotizacionVersionDetalleDto?> GetCotizacionVersionDetailAsync(int versionId)
    {
        var version = await _context.CotizacionesVersiones
            .Include(v => v.Detalles)
            .FirstOrDefaultAsync(v => v.Id == versionId);

        if (version == null)
        {
            return null;
        }

        var versionDto = new CotizacionVersionDto(
            version.Id,
            version.CotizacionId,
            version.NumeroVersion,
            version.FechaVersion,
            version.NombreInteresado,
            version.EmailInteresado,
            version.EmpresaInteresado,
            version.SubTotal,
            version.Impuesto,
            version.Descuento,
            version.Total,
            version.Moneda,
            version.TipoCambio,
            version.VersionActual,
            version.Notas
        );

        var detalles = version.Detalles.Select(d => new DetalleCotizacionDto(
            d.Id,
            d.VersionId,
            d.ProductoId,
            d.ProductoId, // TODO: Obtener nombre del producto desde ERP
            d.Cantidad,
            d.PrecioUnitario,
            d.Descuento,
            d.TotalLinea
        )).ToList();

        return new CotizacionVersionDetalleDto(versionDto, detalles);
    }

    public async Task<List<HistorialCotizacionDto>> GetCotizacionVersionHistoryAsync(int versionId)
    {
        var historiales = await _context.HistorialesCotizacion
            .Where(h => h.VersionId == versionId)
            .OrderByDescending(h => h.FechaEvento)
            .ToListAsync();

        var usuariosIds = historiales
            .Where(h => h.UsuarioEvento.HasValue)
            .Select(h => h.UsuarioEvento!.Value)
            .Distinct()
            .ToList();

        var usuarios = await _context.Usuarios
            .Where(u => usuariosIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Nombre);

        return historiales.Select(h => new HistorialCotizacionDto(
            h.Id,
            h.VersionId,
            h.TipoEvento,
            h.FechaEvento,
            h.UsuarioEvento,
            h.UsuarioEvento.HasValue && usuarios.ContainsKey(h.UsuarioEvento.Value)
                ? usuarios[h.UsuarioEvento.Value]
                : null,
            h.Comentario
        )).ToList();
    }

    public async Task<CopiarVersionResult> CopiarVersionActualAsync(string cotizacionId)
    {
        try
        {
            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Versiones.Where(v => v.VersionActual == '1'))
                    .ThenInclude(v => v.Detalles)
                .FirstOrDefaultAsync(c => c.CotizacionId == cotizacionId);

            if (cotizacion == null)
            {
                return new CopiarVersionResult(false, "Cotización no encontrada", null, null);
            }

            var versionBase = cotizacion.Versiones.FirstOrDefault(v => v.VersionActual == '1');
            if (versionBase == null)
            {
                return new CopiarVersionResult(false, "No se encontró versión actual", null, null);
            }

            // Marcar versión actual como no actual
            versionBase.VersionActual = '0';

            // Crear nueva versión
            var nuevoNumeroVersion = cotizacion.VersionActual + 1;
            var nuevaVersion = new CotizacionVersion
            {
                CotizacionId = cotizacion.CotizacionId,
                NumeroVersion = nuevoNumeroVersion,
                FechaVersion = DateTime.Now,
                NombreInteresado = versionBase.NombreInteresado,
                EmailInteresado = versionBase.EmailInteresado,
                EmpresaInteresado = versionBase.EmpresaInteresado,
                SubTotal = versionBase.SubTotal,
                Impuesto = versionBase.Impuesto,
                Descuento = versionBase.Descuento,
                Total = versionBase.Total,
                Moneda = versionBase.Moneda,
                TipoCambio = versionBase.TipoCambio,
                VersionActual = '1',
                Notas = versionBase.Notas
            };

            _context.CotizacionesVersiones.Add(nuevaVersion);
            await _context.SaveChangesAsync();

            // Copiar detalles
            foreach (var detalle in versionBase.Detalles)
            {
                var nuevoDetalle = new DetalleCotizacionVersion
                {
                    VersionId = nuevaVersion.Id,
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario,
                    Descuento = detalle.Descuento,
                    TotalLinea = detalle.TotalLinea
                };
                _context.DetallesCotizacionVersion.Add(nuevoDetalle);
            }

            // Actualizar cotización
            cotizacion.VersionActual = nuevoNumeroVersion;

            // Registrar evento en historial
            var historial = new HistorialCotizacion
            {
                VersionId = nuevaVersion.Id,
                TipoEvento = TipoEvento.VersionGenerada,
                FechaEvento = DateTime.Now,
                Comentario = $"Nueva versión generada desde versión {versionBase.NumeroVersion}"
            };
            _context.HistorialesCotizacion.Add(historial);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Nueva versión {NumeroVersion} creada para cotización {CotizacionId}", 
                nuevoNumeroVersion, cotizacionId);

            return new CopiarVersionResult(true, null, nuevaVersion.Id, nuevoNumeroVersion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al copiar versión actual de cotización {CotizacionId}", cotizacionId);
            return new CopiarVersionResult(false, "Error al crear nueva versión", null, null);
        }
    }

    public async Task<CopiarVersionResult> CopiarVersionEspecificaAsync(CopiarVersionRequest request)
    {
        try
        {
            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Versiones)
                    .ThenInclude(v => v.Detalles)
                .FirstOrDefaultAsync(c => c.CotizacionId == request.CotizacionId);

            if (cotizacion == null)
            {
                return new CopiarVersionResult(false, "Cotización no encontrada", null, null);
            }

            var versionBase = await _context.CotizacionesVersiones
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == request.VersionIdBase);

            if (versionBase == null)
            {
                return new CopiarVersionResult(false, "Versión base no encontrada", null, null);
            }

            // Marcar versión actual como no actual
            var versionActualPrevia = cotizacion.Versiones.FirstOrDefault(v => v.VersionActual == '1');
            if (versionActualPrevia != null)
            {
                versionActualPrevia.VersionActual = '0';
            }

            // Crear nueva versión
            var nuevoNumeroVersion = cotizacion.VersionActual + 1;
            var nuevaVersion = new CotizacionVersion
            {
                CotizacionId = cotizacion.CotizacionId,
                NumeroVersion = nuevoNumeroVersion,
                FechaVersion = DateTime.Now,
                NombreInteresado = versionBase.NombreInteresado,
                EmailInteresado = versionBase.EmailInteresado,
                EmpresaInteresado = versionBase.EmpresaInteresado,
                SubTotal = versionBase.SubTotal,
                Impuesto = versionBase.Impuesto,
                Descuento = versionBase.Descuento,
                Total = versionBase.Total,
                Moneda = versionBase.Moneda,
                TipoCambio = versionBase.TipoCambio,
                VersionActual = '1',
                Notas = versionBase.Notas
            };

            _context.CotizacionesVersiones.Add(nuevaVersion);
            await _context.SaveChangesAsync();

            // Copiar detalles
            foreach (var detalle in versionBase.Detalles)
            {
                var nuevoDetalle = new DetalleCotizacionVersion
                {
                    VersionId = nuevaVersion.Id,
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario,
                    Descuento = detalle.Descuento,
                    TotalLinea = detalle.TotalLinea
                };
                _context.DetallesCotizacionVersion.Add(nuevoDetalle);
            }

            // Actualizar cotización
            cotizacion.VersionActual = nuevoNumeroVersion;

            // Registrar evento en historial
            var historial = new HistorialCotizacion
            {
                VersionId = nuevaVersion.Id,
                TipoEvento = TipoEvento.VersionGenerada,
                FechaEvento = DateTime.Now,
                Comentario = request.EsVersionAntigua
                    ? $"Nueva versión generada desde versión antigua {versionBase.NumeroVersion}"
                    : $"Nueva versión generada desde versión {versionBase.NumeroVersion}"
            };
            _context.HistorialesCotizacion.Add(historial);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Nueva versión {NumeroVersion} creada desde versión {VersionBase} para cotización {CotizacionId}",
                nuevoNumeroVersion, versionBase.NumeroVersion, request.CotizacionId);

            return new CopiarVersionResult(true, null, nuevaVersion.Id, nuevoNumeroVersion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al copiar versión específica de cotización {CotizacionId}", request.CotizacionId);
            return new CopiarVersionResult(false, "Error al crear nueva versión", null, null);
        }
    }

    public async Task<DuplicarCotizacionResult> DuplicarCotizacionAsync(DuplicarCotizacionRequest request)
    {
        try
        {
            var cotizacionBase = await _context.Cotizaciones
                .Include(c => c.Versiones.Where(v => v.VersionActual == '1'))
                    .ThenInclude(v => v.Detalles)
                .FirstOrDefaultAsync(c => c.CotizacionId == request.CotizacionIdBase);

            if (cotizacionBase == null)
            {
                return new DuplicarCotizacionResult(false, "Cotización base no encontrada", null);
            }

            var versionBase = cotizacionBase.Versiones.FirstOrDefault(v => v.VersionActual == '1');
            if (versionBase == null)
            {
                return new DuplicarCotizacionResult(false, "No se encontró versión actual de la cotización base", null);
            }

            // Generar nuevo ID de cotización
            var nuevoCotizacionId = await GenerarNuevoCotizacionIdAsync();

            // Crear nueva cotización
            var nuevaCotizacion = new Cotizacion
            {
                CotizacionId = nuevoCotizacionId,
                InteresadoId = null, // Se limpia el interesado
                EstadoActual = (char)EstadoCotizacion.Borrador,
                VersionActual = 1,
                MontoCotizacion = versionBase.Total
            };

            _context.Cotizaciones.Add(nuevaCotizacion);
            await _context.SaveChangesAsync();

            // Crear primera versión
            var nuevaVersion = new CotizacionVersion
            {
                CotizacionId = nuevoCotizacionId,
                NumeroVersion = 1,
                FechaVersion = DateTime.Now,
                NombreInteresado = "", // Se limpia
                EmailInteresado = "", // Se limpia
                EmpresaInteresado = "", // Se limpia
                SubTotal = versionBase.SubTotal,
                Impuesto = versionBase.Impuesto,
                Descuento = versionBase.Descuento,
                Total = versionBase.Total,
                Moneda = versionBase.Moneda,
                TipoCambio = versionBase.TipoCambio,
                VersionActual = '1',
                Notas = null // Se limpian las notas
            };

            _context.CotizacionesVersiones.Add(nuevaVersion);
            await _context.SaveChangesAsync();

            // Copiar líneas de detalle
            foreach (var detalle in versionBase.Detalles)
            {
                var nuevoDetalle = new DetalleCotizacionVersion
                {
                    VersionId = nuevaVersion.Id,
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario,
                    Descuento = detalle.Descuento,
                    TotalLinea = detalle.TotalLinea
                };
                _context.DetallesCotizacionVersion.Add(nuevoDetalle);
            }

            // Registrar evento inicial en historial
            var historial = new HistorialCotizacion
            {
                VersionId = nuevaVersion.Id,
                TipoEvento = TipoEvento.Creada,
                FechaEvento = DateTime.Now,
                Comentario = $"Cotización creada por duplicación de {request.CotizacionIdBase}"
            };
            _context.HistorialesCotizacion.Add(historial);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Cotización {NuevoCotizacionId} creada por duplicación de {CotizacionIdBase}",
                nuevoCotizacionId, request.CotizacionIdBase);

            return new DuplicarCotizacionResult(true, null, nuevoCotizacionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al duplicar cotización {CotizacionId}", request.CotizacionIdBase);
            return new DuplicarCotizacionResult(false, "Error al duplicar cotización", null);
        }
    }

    private async Task<string> GenerarNuevoCotizacionIdAsync()
    {
        var ultimaCotizacion = await _context.Cotizaciones
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync();

        if (ultimaCotizacion == null)
        {
            return "COT-0001";
        }

        // Extraer número del ID (formato: COT-0001)
        var partes = ultimaCotizacion.CotizacionId.Split('-');
        if (partes.Length == 2 && int.TryParse(partes[1], out int numero))
        {
            return $"COT-{(numero + 1):D4}";
        }

        // Fallback: contar cotizaciones y sumar 1
        var count = await _context.Cotizaciones.CountAsync();
        return $"COT-{(count + 1):D4}";
    }
}
