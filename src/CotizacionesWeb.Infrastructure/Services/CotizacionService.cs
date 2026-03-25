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
    private readonly IParametroSistemaService _parametroService;

    public CotizacionService(
        DbContextCotizaciones context,
        ILogger<CotizacionService> logger,
        IParametroSistemaService parametroService)
    {
        _context = context;
        _logger = logger;
        _parametroService = parametroService;
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

        // Filtrar por moneda específica (eficiente en SQL)
        if (!string.IsNullOrWhiteSpace(request.Moneda))
        {
            var moneda = request.Moneda.Trim().ToUpper();
            // Validación opcional: verificar que la moneda es válida según FormatHelper
            // Si se necesita acceso al helper desde Infrastructure, se podría inyectar o crear una interfaz
            query = query.Where(x => x.Cotizacion.Moneda.ToUpper() == moneda);
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
                // Información financiera
                c.Moneda, // Moneda de la cotización (movida desde version)
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
        // Obtener la cotización para saber cuál es la versión actual vigente y obtener la moneda
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
            'P', // Temporal: usar 'P' como default hasta obtener la relación con Interesado
            v.SubTotal,
            v.Impuesto,
            v.Descuento,
            v.Total,
            cotizacion.Moneda, // ? CORREGIDO: Moneda desde Cotizacion
            v.TipoCambio, // TipoCambio se mantiene en version
            0, // Siempre 0 porque son versiones históricas (no actuales)
            v.Notas
        )).ToList();
    }

    public async Task<CotizacionVersionDetalleDto?> GetCotizacionVersionDetailAsync(int versionId)
    {
        var versionConInteresadoYCotizacion = await (from ver in _context.CotizacionesVersiones
                                          join cot in _context.Cotizaciones
                                              on ver.CotizacionId equals cot.CotizacionId
                                          join inter in _context.Interesados
                                              on cot.InteresadoId equals inter.InteresadoId into interesadosGroup
                                          from inter in interesadosGroup.DefaultIfEmpty()
                                          where ver.VersionId == versionId
                                          select new { Version = ver, Cotizacion = cot, Interesado = inter })
                                         .FirstOrDefaultAsync();

        if (versionConInteresadoYCotizacion == null)
        {
            return null;
        }

        var version = versionConInteresadoYCotizacion.Version;
        var cotizacion = versionConInteresadoYCotizacion.Cotizacion;
        var interesado = versionConInteresadoYCotizacion.Interesado;

        // Obtener detalles de la versión
        var detalles = await _context.DetallesCotizacionVersion
            .Where(d => d.VersionId == versionId)
            .ToListAsync();

        var versionDto = new CotizacionVersionDto(
            version.VersionId,  // CORREGIDO: Usar VersionId
            version.CotizacionId,
            version.NumeroVersion,
            version.FechaVersion,
            version.NombreInteresado,
            version.EmailInteresado,
            version.EmpresaInteresado,
            interesado?.TipoInteresado ?? 'P', // Nuevo campo - usar 'P' como default
            version.SubTotal,
            version.Impuesto,
            version.Descuento,
            version.Total,
            cotizacion.Moneda, // ? CORREGIDO: Moneda desde Cotizacion
            version.TipoCambio, // TipoCambio se mantiene en version
            version.VersionActual,
            version.Notas
        );

        var detallesDto = detalles.Select(d => new DetalleCotizacionDto(
            d.DetalleVersionId,  // CORREGIDO: Usar DetalleVersionId
            d.VersionId,
            d.ProductoId,
            d.ProductoId, // TODO: Obtener nombre del producto desde ERP
            d.Cantidad,
            d.PrecioUnitario,
            d.Descuento,
            d.TotalLinea
        )).ToList();

        return new CotizacionVersionDetalleDto(versionDto, detallesDto);
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

    public async Task<CopiarVersionResult> CopiarVersionActualAsync(string cotizacionId, string? comentario = null, int? userId = null)
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
            
            // Crear nueva versión con lógica de incremento correcta
            var nuevoNumeroVersion = CalcularNuevaVersion(versionVigente.NumeroVersion);
            var nuevaVersion = new CotizacionVersion
            {
                CotizacionId = cotizacion.CotizacionId,
                NumeroVersion = nuevoNumeroVersion,
                FechaVersion = DateTime.Now,
                NombreInteresado = versionVigente.NombreInteresado,
                EmailInteresado = versionVigente.EmailInteresado,
                EmpresaInteresado = versionVigente.EmpresaInteresado,
                SubTotal = versionVigente.SubTotal,
                Impuesto = versionVigente.Impuesto,
                Descuento = versionVigente.Descuento,
                Total = versionVigente.Total,
                TipoCambio = versionVigente.TipoCambio, // TipoCambio se mantiene en version
                VersionActual = nuevoVersionActual,
                Notas = versionVigente.Notas
                // CORREGIDO: Remover CreatedAt y CreatedBy - los maneja AuditInterceptor automáticamente
            };

            _context.CotizacionesVersiones.Add(nuevaVersion);
            await _context.SaveChangesAsync();

            // Copiar detalles
            foreach (var detalle in detalles)
            {
                var nuevoDetalle = new DetalleCotizacionVersion
                {
                    VersionId = nuevaVersion.VersionId,
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario,
                    Descuento = detalle.Descuento,
                    TotalLinea = detalle.TotalLinea
                    // CORREGIDO: Remover CreatedAt y CreatedBy - los maneja AuditInterceptor automáticamente
                };
                _context.DetallesCotizacionVersion.Add(nuevoDetalle);
            }

            // Actualizar cotización para apuntar a la nueva versión vigente
            cotizacion.VersionActual = nuevoVersionActual;
            
            // ?? CAMBIO CRÍTICO: Al crear una nueva versión, la cotización vuelve a estado Borrador
            // porque es una nueva versión que debe pasar por todo el ciclo de estados
            cotizacion.EstadoActual = (char)EstadoCotizacion.Borrador;

            // Crear comentario para el historial
            var comentarioFinal = string.IsNullOrWhiteSpace(comentario) 
                ? $"Nueva versión {nuevoNumeroVersion} generada desde versión {versionVigente.NumeroVersion}"
                : $"Nueva versión generada: {comentario.Trim()}";

            // Registrar evento en historial
            var historial = new HistorialCotizacion
            {
                VersionId = nuevaVersion.VersionId,
                TipoEvento = "VersionGenerada",
                FechaEvento = DateTime.Now,
                UsuarioEvento = userId ?? 0, // CORREGIDO: Usuario dinámico, 0 si no se proporciona
                Comentario = comentarioFinal
            };
            _context.HistorialesCotizacion.Add(historial);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Nueva versión {NumeroVersion} creada para cotización {CotizacionId}", 
                nuevoNumeroVersion, cotizacionId);

            return new CopiarVersionResult(true, null, nuevaVersion.VersionId, nuevoNumeroVersion);  // CORREGIDO: No cast a int
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al copiar versión actual de cotización {CotizacionId}", cotizacionId);
            return new CopiarVersionResult(false, "Error al crear nueva versión", null, null);
        }
    }

    public async Task<CopiarVersionResult> CopiarVersionEspecificaAsync(CopiarVersionRequest request, int? userId = null)
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
                .FirstOrDefaultAsync(v => v.VersionId == request.VersionIdBase);  // CORREGIDO: Usar VersionId

            if (versionBase == null)
            {
                return new CopiarVersionResult(false, "Versión base no encontrada", null, null);
            }

            // Generar nuevo identificador único para la nueva versión
            var nuevoVersionActual = await GenerarNuevoVersionActualAsync();
            
            // Crear nueva versión con lógica de incremento correcta
            var nuevoNumeroVersion = CalcularNuevaVersion(versionBase.NumeroVersion);
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
                TipoCambio = versionBase.TipoCambio, // TipoCambio se mantiene en version
                VersionActual = nuevoVersionActual,
                Notas = versionBase.Notas
                // CORREGIDO: Remover CreatedAt y CreatedBy - los maneja AuditInterceptor automáticamente
            };

            _context.CotizacionesVersiones.Add(nuevaVersion);
            await _context.SaveChangesAsync();

            // Copiar detalles
            foreach (var detalle in versionBase.Detalles)
            {
                var nuevoDetalle = new DetalleCotizacionVersion
                {
                    VersionId = nuevaVersion.VersionId,
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario,
                    Descuento = detalle.Descuento,
                    TotalLinea = detalle.TotalLinea
                    // CORREGIDO: Remover CreatedAt y CreatedBy - los maneja AuditInterceptor automáticamente
                };
                _context.DetallesCotizacionVersion.Add(nuevoDetalle);
            }

            // Actualizar cotización para apuntar a la nueva versión vigente
            cotizacion.VersionActual = nuevoVersionActual;
            
            // ?? CAMBIO CRÍTICO: Al crear una nueva versión, la cotización vuelve a estado Borrador
            // porque es una nueva versión que debe pasar por todo el ciclo de estados
            cotizacion.EstadoActual = (char)EstadoCotizacion.Borrador;

            // Crear comentario para el historial
            var comentarioFinal = string.IsNullOrWhiteSpace(request.Comentario) 
                ? $"Nueva versión {nuevoNumeroVersion} generada desde versión {(request.EsVersionAntigua ? "antigua " : "")}{versionBase.NumeroVersion}"
                : $"Nueva versión generada: {request.Comentario.Trim()}";

            // Registrar evento en historial
            var historial = new HistorialCotizacion
            {
                VersionId = nuevaVersion.VersionId,
                TipoEvento = "VersionGenerada",
                FechaEvento = DateTime.Now,
                UsuarioEvento = userId ?? 0, // CORREGIDO: Usuario dinámico, 0 si no se proporciona
                Comentario = comentarioFinal
            };
            _context.HistorialesCotizacion.Add(historial);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Nueva versión {NumeroVersion} creada desde versión {VersionBase} para cotización {CotizacionId}",
                nuevoNumeroVersion, versionBase.NumeroVersion, request.CotizacionId);

            return new CopiarVersionResult(true, null, nuevaVersion.VersionId, nuevoNumeroVersion);  // CORREGIDO: No cast a int
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al copiar versión específica de cotización {CotizacionId}", request.CotizacionId);
            return new CopiarVersionResult(false, "Error al crear nueva versión", null, null);
        }
    }

    public async Task<DuplicarCotizacionResult> DuplicarCotizacionAsync(DuplicarCotizacionRequest request, int? userId = null)
    {
        try
        {
            _logger.LogInformation("Iniciando duplicación de cotización {CotizacionId}", request.CotizacionIdBase);

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
                _logger.LogWarning("Cotización base {CotizacionId} no encontrada", request.CotizacionIdBase);
                return new DuplicarCotizacionResult(false, "Cotización base no encontrada", null);
            }

            var cotizacionBase = cotizacionBaseConVersion.Cotizacion;
            var versionVigente = cotizacionBaseConVersion.Version;

            _logger.LogInformation("Cotización base encontrada: {CotizacionId}, Versión: {VersionActual}", 
                cotizacionBase.CotizacionId, cotizacionBase.VersionActual);

            // Obtener los detalles de la versión vigente
            var detalles = await _context.DetallesCotizacionVersion
                .Where(d => d.VersionId == versionVigente.VersionId)
                .ToListAsync();

            _logger.LogInformation("Encontrados {CantidadDetalles} detalles para copiar", detalles.Count);

            // Usar transacción para garantizar integridad
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Generar nuevo ID de cotización
                var nuevoCotizacionId = await GenerarNuevoCotizacionIdAsync();
                _logger.LogInformation("Nuevo ID de cotización generado: {NuevoCotizacionId}", nuevoCotizacionId);
                
                // Generar nuevo identificador único para la primera versión
                var nuevoVersionActual = await GenerarNuevoVersionActualAsync();
                _logger.LogInformation("Nuevo VersionActual generado: {NuevoVersionActual}", nuevoVersionActual);

                // Crear nueva cotización
                var nuevaCotizacion = new Cotizacion
                {
                    CotizacionId = nuevoCotizacionId,
                    InteresadoId = null, // Se limpia el interesado
                    EstadoActual = (char)EstadoCotizacion.Borrador,
                    VersionActual = nuevoVersionActual, // Apuntar a la nueva versión
                    MontoCotizacion = versionVigente.Total,
                    Moneda = cotizacionBase.Moneda, // Copiar moneda desde la cotización base
                };

                _context.Cotizaciones.Add(nuevaCotizacion);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Nueva cotización creada en BD");

                // Crear primera versión
                var nuevaVersion = new CotizacionVersion
                {
                    CotizacionId = nuevoCotizacionId,
                    NumeroVersion = 1.0m,
                    FechaVersion = DateTime.Now,
                    NombreInteresado = "", // Se limpia
                    EmailInteresado = "", // Se limpia
                    EmpresaInteresado = "", // Se limpia
                    SubTotal = versionVigente.SubTotal,
                    Impuesto = versionVigente.Impuesto,
                    Descuento = versionVigente.Descuento,
                    Total = versionVigente.Total,
                    TipoCambio = versionVigente.TipoCambio, // TipoCambio se mantiene en version
                    // Moneda ya no se guarda en version (está en Cotizacion)
                    VersionActual = nuevoVersionActual,
                    Notas = null // Se limpian las notas
                };

                _context.CotizacionesVersiones.Add(nuevaVersion);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Nueva versión creada con VersionId: {VersionId}", nuevaVersion.VersionId);

                // Copiar líneas de detalle
                foreach (var detalle in detalles)
                {
                    var nuevoDetalle = new DetalleCotizacionVersion
                    {
                        VersionId = nuevaVersion.VersionId,
                        ProductoId = detalle.ProductoId,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Descuento = detalle.Descuento,
                        TotalLinea = detalle.TotalLinea
                    };
                    _context.DetallesCotizacionVersion.Add(nuevoDetalle);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Detalles copiados exitosamente");

                // Registrar evento inicial en historial
                var historial = new HistorialCotizacion
                {
                    VersionId = nuevaVersion.VersionId,
                    TipoEvento = "Creada",
                    FechaEvento = DateTime.Now,
                    UsuarioEvento = userId ?? 0,
                    Comentario = $"Cotización creada por duplicación de {request.CotizacionIdBase}"
                };
                _context.HistorialesCotizacion.Add(historial);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Historial registrado");

                // Confirmar transacción
                await transaction.CommitAsync();

                _logger.LogInformation("Cotización {NuevoCotizacionId} duplicada exitosamente de {CotizacionIdBase}",
                    nuevoCotizacionId, request.CotizacionIdBase);

                return new DuplicarCotizacionResult(true, null, nuevoCotizacionId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error en transacción al duplicar cotización {CotizacionId}", request.CotizacionIdBase);
                throw; // Re-lanzar para que sea capturado por el catch externo
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al duplicar cotización {CotizacionId}", request.CotizacionIdBase);
            return new DuplicarCotizacionResult(false, $"Error al duplicar cotización: {ex.Message}", null);
        }
    }

    private async Task<string> GenerarNuevoCotizacionIdAsync()
    {
        try
        {
            // ?? NUEVO: Usar el sistema de parámetros para generar consecutivos
            _logger.LogInformation("Generando nuevo ID de cotización usando sistema de parámetros");
            
            var nuevoConsecutivo = await _parametroService.ObtenerSiguienteConsecutivoCotizacionAsync();
            _logger.LogInformation("Nuevo ID de cotización generado: {NuevoConsecutivo}", nuevoConsecutivo);
            
            return nuevoConsecutivo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico al generar ID usando parámetros, usando método fallback");
            
            // ?? FALLBACK: Método anterior como respaldo
            try
            {
                var ultimoNumero = await _context.Cotizaciones
                    .Where(c => c.CotizacionId.StartsWith("COT-"))
                    .Select(c => c.CotizacionId.Substring(4))
                    .Where(s => s.Length == 4)
                    .Select(s => int.Parse(s))
                    .DefaultIfEmpty(0)
                    .MaxAsync();

                var nuevoNumero = ultimoNumero + 1;
                var fallbackId = $"COT-{nuevoNumero:D4}";
                
                _logger.LogWarning("Usando ID fallback: {FallbackId}", fallbackId);
                return fallbackId;
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx, "Error crítico: tanto el sistema de parámetros como el fallback fallaron");
                throw new InvalidOperationException("No se pudo generar un ID de cotización válido", fallbackEx);
            }
        }
    }

    private async Task<int> GenerarNuevoVersionActualAsync()
    {
        try
        {
            // Usar una consulta más segura que maneje el caso cuando no hay versiones
            var maxVersionActual = await _context.CotizacionesVersiones
                .Select(v => (int?)v.VersionActual)
                .DefaultIfEmpty(0) // Si no hay elementos, usar 0
                .MaxAsync() ?? 0;
            
            return maxVersionActual + 1;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al obtener máximo VersionActual, usando 1 como fallback");
            return 1; // Fallback para el primer caso
        }
    }

    private static decimal CalcularNuevaVersion(decimal versionActual)
    {
        // Implementar lógica de incremento según documentación:
        // Si es versión mayor (1.0, 2.0, 3.0) ? incrementar entero: 1.0 ? 2.0
        // Si es versión menor (1.1, 1.2, 2.5) ? incrementar decimal: 1.1 ? 1.2
        
        if (versionActual % 1 == 0)
        {
            // Es versión mayor (ej: 1.0, 2.0)
            return versionActual + 1.0m;
        }
        else
        {
            // Es versión menor (ej: 1.1, 2.5)
            return versionActual + 0.1m;
        }
    }

    /// <summary>
    /// Actualiza los detalles de una versión de forma inteligente preservando la auditoría.
    /// UPDATE para existentes, INSERT para nuevos, DELETE solo para eliminados.
    /// </summary>
    private async Task ActualizarDetallesVersionAsync(int versionId, List<ActualizarDetalleRequest> nuevosDetalles)
    {
        // Obtener detalles existentes
        var detallesExistentes = await _context.DetallesCotizacionVersion
            .Where(d => d.VersionId == versionId)
            .ToListAsync();

        var detallesExistentesDict = detallesExistentes.ToDictionary(d => d.DetalleVersionId);
        var idsEnRequest = nuevosDetalles.Where(d => d.DetalleVersionId > 0).Select(d => d.DetalleVersionId).ToHashSet();
        
        _logger.LogInformation("?? Actualizando detalles de versión {VersionId}. Existentes en BD: {Existentes}, En request: {Request}", 
            versionId, detallesExistentes.Count, nuevosDetalles.Count);
        
        _logger.LogInformation("?? IDs en request: [{IdsEnRequest}]", 
            string.Join(", ", idsEnRequest));
        _logger.LogInformation("?? IDs existentes en BD: [{IdsExistentes}]", 
            string.Join(", ", detallesExistentes.Select(d => d.DetalleVersionId)));

        // 1. ACTUALIZAR detalles existentes que siguen en el request
        var actualizados = 0;
        foreach (var detalleRequest in nuevosDetalles.Where(d => d.DetalleVersionId > 0))
        {
            if (detallesExistentesDict.TryGetValue(detalleRequest.DetalleVersionId, out var detalleExistente))
            {
                // Actualizar registro existente (preserva CreatedAt y CreatedBy)
                detalleExistente.ProductoId = detalleRequest.ProductoId;
                detalleExistente.Cantidad = detalleRequest.Cantidad;
                detalleExistente.PrecioUnitario = detalleRequest.PrecioUnitario;
                detalleExistente.Descuento = detalleRequest.Descuento;
                detalleExistente.TotalLinea = detalleRequest.TotalLinea;
                
                actualizados++;
                _logger.LogDebug("? Detalle {DetalleId} actualizado: {Producto} x{Cantidad}", 
                    detalleRequest.DetalleVersionId, detalleRequest.ProductoId, detalleRequest.Cantidad);
            }
            else
            {
                _logger.LogWarning("?? Detalle {DetalleId} en request pero no existe en BD", detalleRequest.DetalleVersionId);
            }
        }

        // 2. INSERTAR nuevos detalles (DetalleVersionId = 0)
        var insertados = 0;
        foreach (var detalleRequest in nuevosDetalles.Where(d => d.DetalleVersionId == 0))
        {
            var nuevoDetalle = new DetalleCotizacionVersion
            {
                VersionId = versionId,
                ProductoId = detalleRequest.ProductoId,
                Cantidad = detalleRequest.Cantidad,
                PrecioUnitario = detalleRequest.PrecioUnitario,
                Descuento = detalleRequest.Descuento,
                TotalLinea = detalleRequest.TotalLinea
                // CreatedAt, CreatedBy, ModifiedAt, ModifiedBy ? AuditInterceptor
            };

            _context.DetallesCotizacionVersion.Add(nuevoDetalle);
            insertados++;
            
            _logger.LogDebug("? Nuevo detalle agregado: {Producto} x{Cantidad}", 
                detalleRequest.ProductoId, detalleRequest.Cantidad);
        }

        // 3. ?? ELIMINAR FÍSICAMENTE detalles que ya no están en el request
        var detallesAEliminar = detallesExistentes.Where(d => !idsEnRequest.Contains(d.DetalleVersionId)).ToList();
        if (detallesAEliminar.Any())
        {
            _logger.LogWarning("??? ELIMINANDO {Count} detalles de BD: [{Ids}]", 
                detallesAEliminar.Count, 
                string.Join(", ", detallesAEliminar.Select(d => $"{d.DetalleVersionId}({d.ProductoId})")));
            
            _context.DetallesCotizacionVersion.RemoveRange(detallesAEliminar);
            
            _logger.LogInformation("?? Detalles FÍSICAMENTE eliminados de BD: {Count}", detallesAEliminar.Count);
        }
        else
        {
            _logger.LogInformation("?? No hay detalles para eliminar");
        }

        _logger.LogInformation("? Gestión de detalles completada para versión {VersionId}. Actualizados: {Actualizados}, Insertados: {Insertados}, Eliminados: {Eliminados}", 
            versionId, actualizados, insertados, detallesAEliminar.Count);
    }

    /// <summary>
    /// Calcula los totales de una versión basándose en sus detalles actuales en la base de datos.
    /// </summary>
    private async Task<(decimal subtotal, decimal totalDescuentos, decimal impuesto, decimal total)> CalcularTotalesVersionAsync(int versionId)
    {
        // Obtener detalles actuales después de las modificaciones
        var detalles = await _context.DetallesCotizacionVersion
            .Where(d => d.VersionId == versionId)
            .ToListAsync();

        decimal subtotal = 0;
        decimal totalDescuentos = 0;

        foreach (var detalle in detalles)
        {
            // Subtotal = suma de (cantidad × precio unitario) sin descuentos
            var subtotalLinea = detalle.Cantidad * detalle.PrecioUnitario;
            subtotal += subtotalLinea;
            
            // Acumular descuentos
            totalDescuentos += detalle.Descuento;
        }

        // Subtotal después de descuentos
        var subtotalConDescuentos = subtotal - totalDescuentos;
        
        // Calcular impuesto sobre el subtotal con descuentos (13%)
        var impuesto = subtotalConDescuentos * 0.13m;
        
        // Total final
        var total = subtotalConDescuentos + impuesto;

        _logger.LogDebug("Totales calculados para versión {VersionId}: Subtotal={Subtotal}, Descuentos={Descuentos}, Impuesto={Impuesto}, Total={Total}",
            versionId, subtotal, totalDescuentos, impuesto, total);

        return (subtotal, totalDescuentos, impuesto, total);
    }

    public async Task<ActualizarCotizacionResult> ActualizarCotizacionAsync(ActualizarCotizacionRequest request, int? userId = null)
    {
        try
        {
            _logger.LogInformation("=== INICIO ACTUALIZACIÓN COTIZACIÓN ===");
            _logger.LogInformation("CotizacionId: {CotizacionId}, VersionId: {VersionId}", 
                request.CotizacionId, request.VersionId);
            _logger.LogInformation("Moneda en request: '{Moneda}'", request.Moneda);
            _logger.LogInformation("==========================================");

            // Buscar la cotización y verificar estado
            var cotizacion = await _context.Cotizaciones
                .FirstOrDefaultAsync(c => c.CotizacionId == request.CotizacionId);

            if (cotizacion == null)
            {
                _logger.LogWarning("Cotización {CotizacionId} no encontrada", request.CotizacionId);
                return new ActualizarCotizacionResult(false, "Cotización no encontrada");
            }

            _logger.LogInformation("Cotización encontrada: Estado='{EstadoActual}', MonedaActual='{MonedaActual}'",
                cotizacion.EstadoActual, cotizacion.Moneda);

            // Validar que esté en estado Borrador
            var estadoEsBorrador = cotizacion.EstadoActual == 'B';
            var estadoEsProbablementeBorrador = (int)cotizacion.EstadoActual == 66; // ASCII de 'B'
            
            if (!estadoEsBorrador && !estadoEsProbablementeBorrador)
            {
                _logger.LogWarning("VALIDACIÓN FALLIDA - Intento de editar cotización {CotizacionId} en estado '{Estado}'", 
                    request.CotizacionId, cotizacion.EstadoActual);
                return new ActualizarCotizacionResult(false, "Solo se pueden editar cotizaciones en estado Borrador");
            }

            // Buscar la versión a actualizar
            var version = await _context.CotizacionesVersiones
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.VersionId == request.VersionId);

            if (version == null || version.CotizacionId != request.CotizacionId)
            {
                _logger.LogWarning("Versión {VersionId} no encontrada para cotización {CotizacionId}", 
                    request.VersionId, request.CotizacionId);
                return new ActualizarCotizacionResult(false, "Versión no encontrada");
            }

            _logger.LogInformation("Versión encontrada: VersionId {VersionId}, NumeroVersion {NumeroVersion}", 
                version.VersionId, version.NumeroVersion);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // ?? VERIFICACIÓN CRÍTICA: ¿Hay cambio de moneda?
                bool hayCambioMoneda = !string.IsNullOrEmpty(request.Moneda) && request.Moneda != cotizacion.Moneda;
                
                _logger.LogInformation("?? ANÁLISIS CAMBIO DE MONEDA:");
                _logger.LogInformation("  - Moneda actual en BD: '{MonedaActual}'", cotizacion.Moneda);
                _logger.LogInformation("  - Moneda en request: '{MonedaRequest}'", request.Moneda);
                _logger.LogInformation("  - ¿Hay cambio?: {HayCambio}", hayCambioMoneda);

                if (hayCambioMoneda)
                {
                    _logger.LogInformation("?? PROCESANDO CAMBIO DE MONEDA: {MonedaAnterior} -> {MonedaNueva}", 
                        cotizacion.Moneda, request.Moneda);

                    // Validar que se puede cambiar (estado Borrador y sin líneas en BD)
                    if (cotizacion.EstadoActual == 'B')
                    {
                        var lineasEnBD = await _context.DetallesCotizacionVersion
                            .Where(d => d.VersionId == version.VersionId)
                            .CountAsync();
                        
                        _logger.LogInformation("?? Validación cambio moneda: LineasEnBD={LineasEnBD}", lineasEnBD);
                        
                        if (lineasEnBD == 0)
                        {
                            _logger.LogInformation("? CAMBIO DE MONEDA AUTORIZADO");
                            
                            // ?? ACTUALIZAR MONEDA INMEDIATAMENTE
                            var monedaAnterior = cotizacion.Moneda;
                            cotizacion.Moneda = request.Moneda;
                            
                            _logger.LogInformation("?? Actualizando moneda en entidad: {Antes} -> {Despues}", 
                                monedaAnterior, cotizacion.Moneda);
                            
                            // Marcar como modificado y persistir inmediatamente
                            _context.Cotizaciones.Update(cotizacion);
                            await _context.SaveChangesAsync();
                            
                            _logger.LogInformation("?? SaveChanges ejecutado para moneda");
                            
                            // ?? VERIFICACIÓN INMEDIATA
                            var verificacion = await _context.Cotizaciones
                                .AsNoTracking()
                                .FirstOrDefaultAsync(c => c.CotizacionId == cotizacion.CotizacionId);
                            
                            if (verificacion?.Moneda == request.Moneda)
                            {
                                _logger.LogInformation("? CONFIRMADO: Moneda persistida correctamente en BD: '{Moneda}'", 
                                    verificacion.Moneda);
                            }
                            else
                            {
                                _logger.LogError("? ERROR CRÍTICO: Moneda NO se persistió. BD='{MonedaBD}', Esperado='{MonedaEsperada}'", 
                                    verificacion?.Moneda, request.Moneda);
                                await transaction.RollbackAsync();
                                return new ActualizarCotizacionResult(false, "Error crítico: la moneda no se pudo guardar en la base de datos");
                            }
                        }
                        else
                        {
                            _logger.LogWarning("? CAMBIO DE MONEDA RECHAZADO: Hay {LineasEnBD} líneas en BD", lineasEnBD);
                            await transaction.RollbackAsync();
                            return new ActualizarCotizacionResult(false, 
                                $"No se puede cambiar la moneda cuando hay {lineasEnBD} líneas en la base de datos. Elimine todas las líneas primero.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("? CAMBIO DE MONEDA RECHAZADO: Estado '{Estado}' no es Borrador", 
                            cotizacion.EstadoActual);
                        await transaction.RollbackAsync();
                        return new ActualizarCotizacionResult(false, "Solo se puede cambiar la moneda en estado Borrador");
                    }
                }
                else
                {
                    _logger.LogInformation("?? No hay cambio de moneda solicitado");
                }

                // Actualizar datos de la versión
                version.NombreInteresado = request.NombreInteresado;
                version.EmailInteresado = request.EmailInteresado;
                version.EmpresaInteresado = request.EmpresaInteresado;
                version.Notas = request.Notas;
                
                // ?? Actualizar número de versión si se proporciona
                if (request.NumeroVersion.HasValue && request.NumeroVersion.Value != version.NumeroVersion)
                {
                    _logger.LogInformation("Número de versión actualizado: {VersionAnterior} -> {VersionNueva}", 
                        version.NumeroVersion, request.NumeroVersion.Value);
                    version.NumeroVersion = request.NumeroVersion.Value;
                }

                // Actualizar tipo de cambio si se proporciona
                if (request.TipoCambio.HasValue)
                {
                    version.TipoCambio = request.TipoCambio.Value;
                    _logger.LogInformation("Tipo de cambio actualizado a: {TipoCambio}", request.TipoCambio.Value);
                }

                // GESTIÓN DE DETALLES
                if (request.Detalles != null && request.Detalles.Any())
                {
                    await ActualizarDetallesVersionAsync(version.VersionId, request.Detalles);
                    _logger.LogInformation("Detalles actualizados para versión {VersionId}: {CantidadDetalles} elementos", 
                        version.VersionId, request.Detalles.Count);
                }
                else
                {
                    _logger.LogInformation("Cotización guardada sin detalles (estado borrador)");
                    // Limpiar detalles existentes si los hubiera
                    var detallesExistentes = await _context.DetallesCotizacionVersion
                        .Where(d => d.VersionId == version.VersionId)
                        .ToListAsync();
                    
                    if (detallesExistentes.Any())
                    {
                        _logger.LogInformation("Eliminando {CantidadDetalles} detalles existentes", detallesExistentes.Count);
                        _context.DetallesCotizacionVersion.RemoveRange(detallesExistentes);
                    }
                }

                // Calcular totales después de la actualización de detalles
                var (subtotal, totalDescuentos, impuesto, total) = await CalcularTotalesVersionAsync(version.VersionId);

                // Actualizar totales en la versión
                version.SubTotal = subtotal;
                version.Descuento = totalDescuentos;
                version.Impuesto = impuesto;
                version.Total = total;

                // Actualizar monto de la cotización
                cotizacion.MontoCotizacion = total;

                _logger.LogInformation("?? Totales calculados: SubTotal={SubTotal}, Impuesto={Impuesto}, Total={Total}", 
                    subtotal, impuesto, total);

                // SaveChanges final para el resto de cambios
                await _context.SaveChangesAsync();

                // Registrar en historial
                var historial = new HistorialCotizacion
                {
                    VersionId = version.VersionId,
                    TipoEvento = "Actualizada",
                    FechaEvento = DateTime.Now,
                    UsuarioEvento = userId ?? 0,
                    Comentario = hayCambioMoneda ? 
                        $"Cotización actualizada - Moneda cambiada a {request.Moneda}" : 
                        "Cotización actualizada"
                };

                _context.HistorialesCotizacion.Add(historial);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("=== ACTUALIZACIÓN COMPLETADA EXITOSAMENTE ===");
                _logger.LogInformation("CotizacionId: {CotizacionId}, Total: {Total}, Moneda: {Moneda}",
                    request.CotizacionId, total, cotizacion.Moneda);
                _logger.LogInformation("=============================================");

                return new ActualizarCotizacionResult(true, null);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error en transacción al actualizar cotización {CotizacionId}", request.CotizacionId);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cotización {CotizacionId}", request.CotizacionId);
            return new ActualizarCotizacionResult(false, $"Error interno: {ex.Message}");
        }
    }

    public async Task<CotizacionDebugInfoDto> GetCotizacionDebugInfoAsync(string cotizacionId)
    {
        try
        {
            _logger.LogInformation("=== DEBUGGING INFORMACIÓN DE COTIZACIÓN {CotizacionId} ===", cotizacionId);
            
            // Consultar cotización directamente
            var cotizacion = await _context.Cotizaciones
                .FirstOrDefaultAsync(c => c.CotizacionId == cotizacionId);

            if (cotizacion == null)
            {
                _logger.LogWarning("Cotización {CotizacionId} no encontrada en base de datos", cotizacionId);
                return new CotizacionDebugInfoDto(
                    cotizacionId, 
                    'X', 
                    "NO ENCONTRADA", 
                    0, 
                    null, 
                    null, 
                    null, 
                    false);
            }

            _logger.LogInformation("Cotización encontrada: Estado='{Estado}' (char: {EstadoChar}, ASCII: {EstadoAscii}), VersionActual={VersionActual}", 
                cotizacion.EstadoActual, cotizacion.EstadoActual, (int)cotizacion.EstadoActual, cotizacion.VersionActual);

            // Verificar comparación con 'B'
            var esBorrador = cotizacion.EstadoActual == 'B';
            _logger.LogInformation("Comparación Estado == 'B': {EsBorrador} (Estado: '{Estado}', B: '{B}', EstadoAscii: {EstadoAscii}, BAscii: {BAscii})",
                esBorrador, cotizacion.EstadoActual, 'B', (int)cotizacion.EstadoActual, (int)'B');

            // Obtener información de la versión actual
            var version = await _context.CotizacionesVersiones
                .FirstOrDefaultAsync(v => v.VersionId == cotizacion.VersionActual);

            var versionInfo = version != null ? 
                $"VersionId={version.VersionId}, NumeroVersion={version.NumeroVersion}" : 
                "Versión no encontrada";

            _logger.LogInformation("Información de versión: {VersionInfo}", versionInfo);

            var estadoTexto = cotizacion.EstadoActual switch
            {
                'B' => "Borrador",
                'P' => "Pendiente Aprobación", 
                'A' => "Aprobada",
                'E' => "Enviada",
                'T' => "Aceptada",
                'R' => "Rechazada",
                'C' => "Cancelada",
                'X' => "Archivada",
                _ => $"Desconocido (char: {(int)cotizacion.EstadoActual})"
            };

            return new CotizacionDebugInfoDto(
                cotizacion.CotizacionId,
                cotizacion.EstadoActual,
                estadoTexto,
                cotizacion.VersionActual,
                cotizacion.CreatedAt,
                cotizacion.ModifiedAt,
                versionInfo,
                true
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener información de debugging para cotización {CotizacionId}", cotizacionId);
            return new CotizacionDebugInfoDto(
                cotizacionId, 
                'X', 
                "ERROR EN CONSULTA", 
                0, 
                null, 
                null, 
                ex.Message, 
                false);
        }
    }

    public async Task<CotizacionVersionDetalleDto?> GetCotizacionCurrentVersionDetailAsync(string cotizacionId)
    {
        try
        {
            _logger.LogInformation("Obteniendo detalle de versión actual para cotización {CotizacionId}", cotizacionId);
            
            // Buscar la cotización para obtener el VersionActual
            var cotizacion = await _context.Cotizaciones
                .FirstOrDefaultAsync(c => c.CotizacionId == cotizacionId);

            if (cotizacion == null)
            {
                _logger.LogWarning("Cotización {CotizacionId} no encontrada", cotizacionId);
                return null;
            }

            _logger.LogInformation("Cotización encontrada: VersionActual={VersionActual}", cotizacion.VersionActual);

            // Buscar la versión específica usando CotizacionId y VersionActual
            var version = await _context.CotizacionesVersiones
                .FirstOrDefaultAsync(v => v.CotizacionId == cotizacionId && v.VersionActual == cotizacion.VersionActual);

            if (version == null)
            {
                _logger.LogWarning("Versión actual {VersionActual} no encontrada para cotización {CotizacionId}", 
                    cotizacion.VersionActual, cotizacionId);
                return null;
            }

            _logger.LogInformation("Versión actual encontrada: VersionId={VersionId}, NumeroVersion={NumeroVersion}", 
                version.VersionId, version.NumeroVersion);

            // Ahora usar el método existente con el VersionId correcto
            return await GetCotizacionVersionDetailAsync(version.VersionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalle de versión actual para cotización {CotizacionId}", cotizacionId);
            return null;
        }
    }
}
