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
        // Usar una consulta con JOIN explícito para evitar problemas de EF
        var query = from cot in _context.Cotizaciones
                    join ver in _context.CotizacionesVersiones
                        on new { cot.CotizacionId, VersionId = cot.VersionActual }
                        equals new { ver.CotizacionId, VersionId = ver.VersionActual }
                    select new { Cotizacion = cot, Version = ver };

        // Filtrar por estados si se proporcionan
        if (request.Estados != null && request.Estados.Any())
        {
            query = query.Where(x => request.Estados.Contains(x.Cotizacion.EstadoActual));
        }

        // Filtrar por rango de fechas
        if (request.FechaDesde.HasValue)
        {
            var fechaDesdeInicioDia = request.FechaDesde.Value.Date; // 00:00:00
            query = query.Where(x => x.Cotizacion.CreatedAt >= fechaDesdeInicioDia);
        }

        if (request.FechaHasta.HasValue)
        {
            var fechaHastaFinDia = request.FechaHasta.Value.Date.AddDays(1).AddTicks(-1); // 23:59:59.999
            query = query.Where(x => x.Cotizacion.CreatedAt <= fechaHastaFinDia);
        }

        // Filtrar por búsqueda general (solo texto: ID, Nombre, Empresa)
        if (!string.IsNullOrWhiteSpace(request.Busqueda))
        {
            var busqueda = request.Busqueda.ToLower();
            query = query.Where(x =>
                x.Cotizacion.CotizacionId.ToLower().Contains(busqueda) ||
                x.Version.NombreInteresado.ToLower().Contains(busqueda) ||
                x.Version.EmpresaInteresado.ToLower().Contains(busqueda)
            );
        }

        // Filtrar por rango de monto (eficiente en SQL)
        if (request.MontoDesde.HasValue)
        {
            query = query.Where(x => x.Cotizacion.MontoCotizacion >= request.MontoDesde.Value);
        }

        if (request.MontoHasta.HasValue)
        {
            query = query.Where(x => x.Cotizacion.MontoCotizacion <= request.MontoHasta.Value);
        }

        // Filtrar por versión específica (eficiente en SQL)
        if (request.Version.HasValue)
        {
            query = query.Where(x => x.Version.NumeroVersion == request.Version.Value);
        }

        var resultados = await query
            .OrderByDescending(x => x.Cotizacion.CreatedAt)
            .ToListAsync();

        return resultados.Select(result =>
        {
            var c = result.Cotizacion;
            var versionVigente = result.Version;
            
            return new CotizacionListDto(
                0,  // FASE 3: El DTO aún espera un ID numérico, usar 0 temporalmente
                c.CotizacionId,
                c.InteresadoId,
                versionVigente?.NombreInteresado ?? "",
                versionVigente?.EmpresaInteresado ?? "",
                c.EstadoActual,
                c.VersionActual,
                versionVigente?.NumeroVersion ?? 1.0m, // NumeroVersion específico de la versión vigente
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
        // Buscar la cotización y su versión vigente usando un JOIN explícito
        var cotizacionConVersion = await (from cot in _context.Cotizaciones
                                          join ver in _context.CotizacionesVersiones
                                              on new { cot.CotizacionId, VersionId = cot.VersionActual }
                                              equals new { ver.CotizacionId, VersionId = ver.VersionActual }
                                          where cot.CotizacionId == cotizacionId
                                          select new { Cotizacion = cot, Version = ver })
                                         .FirstOrDefaultAsync();

        if (cotizacionConVersion == null)
        {
            return new List<HistorialCotizacionDto>();
        }

        // Obtener los historiales de la versión vigente
        var historiales = await _context.HistorialesCotizacion
            .Where(h => h.VersionId == cotizacionConVersion.Version.VersionId)  // FASE 3: Usar VersionId
            .OrderByDescending(h => h.FechaEvento)
            .ToListAsync();

        var usuariosIds = historiales
            .Where(h => h.UsuarioEvento.HasValue)
            .Select(h => h.UsuarioEvento!.Value)
            .Distinct()
            .ToList();

        var usuarios = await _context.Usuarios
            .Where(u => usuariosIds.Contains(u.UsuarioId))  // CORREGIDO: Usar UsuarioId
            .ToDictionaryAsync(u => u.UsuarioId, u => u.Nombre);  // CORREGIDO: Usar UsuarioId

        return historiales.Select(h => new HistorialCotizacionDto(
            h.HistorialId,  // FASE 3: Usar HistorialId en lugar de Id
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
        // Obtener la cotización para saber cuál es la versión actual vigente
        var cotizacion = await _context.Cotizaciones
            .FirstOrDefaultAsync(c => c.CotizacionId == cotizacionId);

        if (cotizacion == null)
        {
            return new List<CotizacionVersionDto>();
        }

        var versionActualId = cotizacion.VersionActual;

        // Obtener SOLO las versiones anteriores (excluir la versión actual)
        var versiones = await _context.CotizacionesVersiones
            .Where(v => v.CotizacionId == cotizacionId && v.VersionActual != versionActualId)
            .OrderByDescending(v => v.NumeroVersion)
            .ToListAsync();

        return versiones.Select(v => new CotizacionVersionDto(
            v.VersionId,  // CORREGIDO: Usar VersionId en lugar de Id
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
            0, // Siempre 0 porque son versiones históricas (no actuales)
            v.Notas
        )).ToList();
    }

    public async Task<CotizacionVersionDetalleDto?> GetCotizacionVersionDetailAsync(int versionId)
    {
        var version = await _context.CotizacionesVersiones
            .Include(v => v.Detalles)
            .FirstOrDefaultAsync(v => v.VersionId == versionId);  // CORREGIDO: Usar VersionId

        if (version == null)
        {
            return null;
        }

        var versionDto = new CotizacionVersionDto(
            version.VersionId,  // CORREGIDO: Usar VersionId
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
            d.DetalleVersionId,  // CORREGIDO: Usar DetalleVersionId
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
            .Where(u => usuariosIds.Contains(u.UsuarioId))  // CORREGIDO: Usar UsuarioId
            .ToDictionaryAsync(u => u.UsuarioId, u => u.Nombre);  // CORREGIDO: Usar UsuarioId

        return historiales.Select(h => new HistorialCotizacionDto(
            h.HistorialId,  // FASE 3: Usar HistorialId en lugar de Id
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
            // Buscar la cotización y su versión vigente usando un JOIN explícito
            var cotizacionConVersion = await (from cot in _context.Cotizaciones
                                              join ver in _context.CotizacionesVersiones
                                                  on new { cot.CotizacionId, VersionId = cot.VersionActual }
                                                  equals new { ver.CotizacionId, VersionId = ver.VersionActual }
                                              where cot.CotizacionId == cotizacionId
                                              select new { Cotizacion = cot, Version = ver })
                                             .FirstOrDefaultAsync();

            if (cotizacionConVersion == null)
            {
                return new CopiarVersionResult(false, "Cotización no encontrada", null, null);
            }

            var cotizacion = cotizacionConVersion.Cotizacion;
            var versionVigente = cotizacionConVersion.Version;

            // Obtener los detalles de la versión vigente
            var detalles = await _context.DetallesCotizacionVersion
                .Where(d => d.VersionId == versionVigente.VersionId)  // CORREGIDO: Usar VersionId
                .ToListAsync();

            // Generar nuevo identificador único para la nueva versión
            var nuevoVersionActual = await GenerarNuevoVersionActualAsync();
            
            // Crear nueva versión
            var nuevoNumeroVersion = versionVigente.NumeroVersion + 1;
            var nuevaVersion = new CotizacionVersion
            {
                CotizacionId = cotizacion.CotizacionId,
                NumeroVersion = nuevoNumeroVersion,
                FechaVersion = DateTime.Now,  // CORREGIDO: datetime en lugar de datetime2
                NombreInteresado = versionVigente.NombreInteresado,
                EmailInteresado = versionVigente.EmailInteresado,
                EmpresaInteresado = versionVigente.EmpresaInteresado,
                SubTotal = versionVigente.SubTotal,
                Impuesto = versionVigente.Impuesto,
                Descuento = versionVigente.Descuento,
                Total = versionVigente.Total,
                Moneda = versionVigente.Moneda,
                TipoCambio = versionVigente.TipoCambio,
                VersionActual = nuevoVersionActual,
                Notas = versionVigente.Notas,
                // CORREGIDO: Agregar auditoría
                CreatedAt = DateTime.Now,
                CreatedBy = 1  // Usuario sistema
            };

            _context.CotizacionesVersiones.Add(nuevaVersion);
            await _context.SaveChangesAsync();

            // Copiar detalles
            foreach (var detalle in detalles)
            {
                var nuevoDetalle = new DetalleCotizacionVersion
                {
                    VersionId = nuevaVersion.VersionId,  // CORREGIDO: Usar VersionId
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario,
                    Descuento = detalle.Descuento,
                    TotalLinea = detalle.TotalLinea,
                    // CORREGIDO: Agregar auditoría
                    CreatedAt = DateTime.Now,
                    CreatedBy = 1  // Usuario sistema
                };
                _context.DetallesCotizacionVersion.Add(nuevoDetalle);
            }

            // Actualizar cotización para apuntar a la nueva versión vigente
            cotizacion.VersionActual = nuevoVersionActual;

            // Registrar evento en historial
            var historial = new HistorialCotizacion
            {
                VersionId = nuevaVersion.VersionId,  // CORREGIDO: Usar VersionId
                TipoEvento = TipoEvento.VersionGenerada.ToString(),  // CORREGIDO: Convertir enum a string
                FechaEvento = DateTime.Now,  // CORREGIDO: datetime en lugar de datetime2
                UsuarioEvento = 1,  // CORREGIDO: Agregar usuario sistema
                Comentario = $"Nueva versión {nuevoNumeroVersion} generada desde versión {versionVigente.NumeroVersion}"
            };
            _context.HistorialesCotizacion.Add(historial);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Nueva versión {NumeroVersion} creada para cotización {CotizacionId}", 
                nuevoNumeroVersion, cotizacionId);

            return new CopiarVersionResult(true, null, nuevaVersion.Id, (int)nuevoNumeroVersion);
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

            // Generar nuevo identificador único para la nueva versión
            var nuevoVersionActual = await GenerarNuevoVersionActualAsync();
            
            // Crear nueva versión
            var nuevoNumeroVersion = versionBase.NumeroVersion + 1;
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
                VersionActual = nuevoVersionActual,
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

            // Actualizar cotización para apuntar a la nueva versión vigente
            cotizacion.VersionActual = nuevoVersionActual;

            // Registrar evento en historial
            var historial = new HistorialCotizacion
            {
                VersionId = nuevaVersion.Id,
                TipoEvento = TipoEvento.VersionGenerada,
                FechaEvento = DateTime.Now,
                Comentario = request.EsVersionAntigua
                    ? $"Nueva versión {nuevoNumeroVersion} generada desde versión antigua {versionBase.NumeroVersion}"
                    : $"Nueva versión {nuevoNumeroVersion} generada desde versión {versionBase.NumeroVersion}"
            };
            _context.HistorialesCotizacion.Add(historial);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Nueva versión {NumeroVersion} creada desde versión {VersionBase} para cotización {CotizacionId}",
                nuevoNumeroVersion, versionBase.NumeroVersion, request.CotizacionId);

            return new CopiarVersionResult(true, null, nuevaVersion.Id, (int)nuevoNumeroVersion);
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
            // Buscar la cotización base y su versión vigente usando un JOIN explícito
            var cotizacionBaseConVersion = await (from cotizacion in _context.Cotizaciones
                                                  join version in _context.CotizacionesVersiones
                                                      on new { cotizacion.CotizacionId, VersionId = cotizacion.VersionActual }
                                                      equals new { version.CotizacionId, VersionId = version.VersionActual }
                                                  where cotizacion.CotizacionId == request.CotizacionIdBase
                                                  select new { Cotizacion = cotizacion, Version = version })
                                                 .FirstOrDefaultAsync();

            if (cotizacionBaseConVersion == null)
            {
                return new DuplicarCotizacionResult(false, "Cotización base no encontrada", null);
            }

            var cotizacionBase = cotizacionBaseConVersion.Cotizacion;
            var versionVigente = cotizacionBaseConVersion.Version;

            // Obtener los detalles de la versión vigente
            var detalles = await _context.DetallesCotizacionVersion
                .Where(d => d.VersionId == versionVigente.Id)
                .ToListAsync();

            // Generar nuevo ID de cotización
            var nuevoCotizacionId = await GenerarNuevoCotizacionIdAsync();
            
            // Generar nuevo identificador único para la primera versión
            var nuevoVersionActual = await GenerarNuevoVersionActualAsync();

            // Crear nueva cotización
            var nuevaCotizacion = new Cotizacion
            {
                CotizacionId = nuevoCotizacionId,
                InteresadoId = null, // Se limpia el interesado
                EstadoActual = (char)EstadoCotizacion.Borrador,
                VersionActual = nuevoVersionActual, // Apuntar a la nueva versión
                MontoCotizacion = versionVigente.Total
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
                SubTotal = versionVigente.SubTotal,
                Impuesto = versionVigente.Impuesto,
                Descuento = versionVigente.Descuento,
                Total = versionVigente.Total,
                Moneda = versionVigente.Moneda,
                TipoCambio = versionVigente.TipoCambio,
                VersionActual = nuevoVersionActual, // Usar el identificador único generado
                Notas = null // Se limpian las notas
            };

            _context.CotizacionesVersiones.Add(nuevaVersion);
            await _context.SaveChangesAsync();

            // Copiar líneas de detalle
            foreach (var detalle in detalles)
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

    private async Task<int> GenerarNuevoVersionActualAsync()
    {
        // Generar un identificador único para VersionActual
        var maxVersionActual = await _context.CotizacionesVersiones
            .MaxAsync(v => (int?)v.VersionActual) ?? 0;
        
        return maxVersionActual + 1;
    }
}
