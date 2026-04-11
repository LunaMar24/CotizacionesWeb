using CotizacionesWeb.Application.Cotizaciones;
using CotizacionesWeb.Infrastructure.Data;
using CotizacionesWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Infrastructure.Services;

public class AssignInteresadoHubSpotService : IAssignInteresadoHubSpotService
{
  private readonly DbContextCotizaciones _context;
  private readonly ILogger<AssignInteresadoHubSpotService> _logger;

  public AssignInteresadoHubSpotService(
      DbContextCotizaciones context,
      ILogger<AssignInteresadoHubSpotService> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task<AssignInteresadoHubSpotResult> ResolverInteresadoAsync(
      ResolverInteresadoHubSpotRequest request,
      int? userId = null)
  {
    if (request is null)
      throw new ArgumentNullException(nameof(request));

    if (string.IsNullOrWhiteSpace(request.HubSpotObjectId))
      return new AssignInteresadoHubSpotResult(false, "El identificador de HubSpot es requerido.", null, null, null, null, null);

    if (string.IsNullOrWhiteSpace(request.HubSpotObjectType))
      return new AssignInteresadoHubSpotResult(false, "El tipo de objeto de HubSpot es requerido.", null, null, null, null, null);

    try
    {
      var interesado = await ObtenerInteresadoExistenteAsync(request.HubSpotObjectId, request.HubSpotObjectType);

      if (interesado is null)
      {
        // Crear nuevo interesado
        interesado = new Interesado
        {
          HubspotObjectId = request.HubSpotObjectId,
          HubspotObjectType = request.HubSpotObjectType,
          TipoInteresado = (char)request.TipoInteresado,
          Activo = true,
          FechaUltSync = DateTime.Now
        };
        
        _context.Interesados.Add(interesado);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(
            "Nuevo interesado creado desde HubSpot. InteresadoId: {InteresadoId}, HubSpotObjectId: {HubSpotObjectId}",
            interesado.InteresadoId,
            request.HubSpotObjectId);
      }
      else
      {
        // Actualizar interesado existente
        interesado.TipoInteresado = (char)request.TipoInteresado;
        interesado.Activo = true;
        interesado.FechaUltSync = DateTime.Now;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(
            "Interesado existente actualizado desde HubSpot. InteresadoId: {InteresadoId}, HubSpotObjectId: {HubSpotObjectId}",
            interesado.InteresadoId,
            request.HubSpotObjectId);
      }

      return new AssignInteresadoHubSpotResult(
          true,
          null,
          interesado.InteresadoId,
          interesado.TipoInteresado,
          request.NombreInteresado,
          request.EmailInteresado,
          request.EmpresaInteresado);
    }
    catch (Exception ex)
    {
      _logger.LogError(
          ex,
          "Error al resolver interesado desde HubSpot. HubSpotObjectId: {HubSpotObjectId}",
          request.HubSpotObjectId);

      return new AssignInteresadoHubSpotResult(false, "Error interno al resolver el interesado.", null, null, null, null, null);
    }
  }

  public async Task<AssignInteresadoHubSpotResult> AssignAsync(
      AssignInteresadoHubSpotRequest request,
      int? userId = null)
  {
    if (request is null)
      throw new ArgumentNullException(nameof(request));

    if (string.IsNullOrWhiteSpace(request.CotizacionId))
      return new AssignInteresadoHubSpotResult(false, "La cotización es requerida.", null, null, null, null, null);

    if (string.IsNullOrWhiteSpace(request.HubSpotObjectId))
      return new AssignInteresadoHubSpotResult(false, "El identificador de HubSpot es requerido.", null, null, null, null, null);

    if (string.IsNullOrWhiteSpace(request.HubSpotObjectType))
      return new AssignInteresadoHubSpotResult(false, "El tipo de objeto de HubSpot es requerido.", null, null, null, null, null);

    await using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
      var cotizacion = await ObtenerCotizacionAsync(request.CotizacionId);
      if (cotizacion is null)
      {
        return new AssignInteresadoHubSpotResult(false, "No se encontró la cotización.", null, null, null, null, null);
      }

      var versionVigente = await ObtenerVersionVigenteAsync(cotizacion);
      if (versionVigente is null)
      {
        return new AssignInteresadoHubSpotResult(false, "No se encontró la versión vigente de la cotización.", null, null, null, null, null);
      }

      var interesado = await ObtenerInteresadoExistenteAsync(request.HubSpotObjectId, request.HubSpotObjectType);

      if (interesado is null)
      {
        interesado = CrearInteresado(request);
        _context.Interesados.Add(interesado);
        await _context.SaveChangesAsync();
      }
      else
      {
        ActualizarInteresadoExistente(interesado, request);
      }

      cotizacion.InteresadoId = interesado.InteresadoId;
      ActualizarSnapshotVersion(versionVigente, request);

      await _context.SaveChangesAsync();
      await transaction.CommitAsync();

      _logger.LogInformation(
          "Interesado asignado correctamente. CotizacionId: {CotizacionId}, InteresadoId: {InteresadoId}, HubSpotObjectId: {HubSpotObjectId}",
          cotizacion.CotizacionId,
          interesado.InteresadoId,
          request.HubSpotObjectId);

      return new AssignInteresadoHubSpotResult(
          true,
          null,
          interesado.InteresadoId,
          interesado.TipoInteresado,
          versionVigente.NombreInteresado,
          versionVigente.EmailInteresado,
          versionVigente.EmpresaInteresado);
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();

      _logger.LogError(
          ex,
          "Error al asignar interesado desde HubSpot. CotizacionId: {CotizacionId}, HubSpotObjectId: {HubSpotObjectId}",
          request.CotizacionId,
          request.HubSpotObjectId);

      throw;
    }
  }

  private Task<Cotizacion?> ObtenerCotizacionAsync(string cotizacionId)
  {
    return _context.Cotizaciones
        .FirstOrDefaultAsync(c => c.CotizacionId == cotizacionId);
  }

  private Task<CotizacionVersion?> ObtenerVersionVigenteAsync(Cotizacion cotizacion)
  {
    return _context.CotizacionesVersiones
        .FirstOrDefaultAsync(v =>
            v.CotizacionId == cotizacion.CotizacionId &&
            v.VersionActual == cotizacion.VersionActual);
  }

  private Task<Interesado?> ObtenerInteresadoExistenteAsync(string hubSpotObjectId, string hubSpotObjectType)
  {
    return _context.Interesados
        .FirstOrDefaultAsync(i =>
            i.HubspotObjectId == hubSpotObjectId &&
            i.HubspotObjectType == hubSpotObjectType);
  }

  private static Interesado CrearInteresado(AssignInteresadoHubSpotRequest request)
  {
    return new Interesado
    {
      HubspotObjectId = request.HubSpotObjectId,
      HubspotObjectType = request.HubSpotObjectType,
      TipoInteresado = (char)request.TipoInteresado,
      Activo = true,
      FechaUltSync = DateTime.Now
    };
  }

  private static void ActualizarInteresadoExistente(Interesado interesado, AssignInteresadoHubSpotRequest request)
  {
    interesado.TipoInteresado = (char)request.TipoInteresado;
    interesado.Activo = true;
    interesado.FechaUltSync = DateTime.Now;
  }

  private static void ActualizarSnapshotVersion(CotizacionVersion version, AssignInteresadoHubSpotRequest request)
  {
    version.NombreInteresado = request.NombreInteresado;
    version.EmailInteresado = request.EmailInteresado ?? string.Empty;
    version.EmpresaInteresado = request.EmpresaInteresado ?? string.Empty;
  }
}