using CotizacionesWeb.Application.Configuracion;
using CotizacionesWeb.Application.Integrations.Erp;
using CotizacionesWeb.Application.Integrations.HubSpot;
using CotizacionesWeb.Domain.Entities;
using CotizacionesWeb.Domain.Enums;
using CotizacionesWeb.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace CotizacionesWeb.Infrastructure.Integrations.Erp;

/// <summary>
/// Servicio de integración con ERP para generación de pedidos
/// ACTUALIZADO: Nuevos campos MONEDA(1), NIVEL_PRECIO(12) en cabecera y PORCENTAJE_IMPUESTO en detalle
/// CORREGIDO: Vulnerabilidades de SQL injection usando FormattableString
/// </summary>
public class ErpPedidoService : IErpPedidoService
{
  private readonly DbContextCotizaciones _contextCotizaciones;
  private readonly DbContextErp _contextErp;
  private readonly IErpConfigurationService _erpConfigService;
  private readonly IHubSpotClienteErpService _hubspotClienteErpService;
  private readonly IParametroSistemaService _parametroSistemaService;
  private readonly ILogger<ErpPedidoService> _logger;

  // Estados válidos para envío a ERP
  private const char ESTADO_ACEPTADA = (char)EstadoCotizacion.Aceptada;

  // Estados de integración
  private const string ESTADO_PENDIENTE = "Pendiente";
  private const string ESTADO_PROCESADO = "Procesado";
  private const string ESTADO_NO_SINCRONIZADO = "NoSincronizado";
  private const string ESTADO_ERROR = "Error";

  public ErpPedidoService(
      DbContextCotizaciones contextCotizaciones,
      DbContextErp contextErp,
      IErpConfigurationService erpConfigService,
      IHubSpotClienteErpService hubspotClienteErpService,
      IParametroSistemaService parametroSistemaService,
      ILogger<ErpPedidoService> logger)
  {
    _contextCotizaciones = contextCotizaciones;
    _contextErp = contextErp;
    _erpConfigService = erpConfigService;
    _hubspotClienteErpService = hubspotClienteErpService;
    _parametroSistemaService = parametroSistemaService;
    _logger = logger;
  }

  /// <summary>
  /// Valida que el esquema ERP sea seguro para usar en SQL dinámico
  /// Solo permite letras, números y underscore
  /// </summary>
  private static bool EsEsquemaSeguro(string esquema)
  {
    if (string.IsNullOrWhiteSpace(esquema))
      return false;

    // Solo permitir letras, números y underscore
    return esquema.All(c => char.IsLetterOrDigit(c) || c == '_') &&
           esquema.Length <= 50; // Limitar longitud por seguridad
  }

  /// <summary>
  /// Valida que la versión recibida corresponda a la versión vigente de la cotización
  /// Sigue la lógica: Cotizacion.VersionActual -> CotizacionVersion donde VersionActual = Cotizacion.VersionActual -> validar VersionId
  /// </summary>
  private async Task<bool> ValidarVersionVigenteAsync(string cotizacionId, int versionId)
  {
    try
    {
      // 1. Obtener el VersionActual de la cotización
      var cotizacion = await _contextCotizaciones.Cotizaciones
          .FirstOrDefaultAsync(c => c.CotizacionId == cotizacionId);

      if (cotizacion == null)
      {
        _logger.LogWarning("Cotización {CotizacionId} no encontrada para validación de versión", cotizacionId);
        return false;
      }

      // 2. Buscar la versión vigente en CotizacionesVersiones usando CotizacionId y VersionActual
      var versionVigente = await _contextCotizaciones.CotizacionesVersiones
          .FirstOrDefaultAsync(v => v.CotizacionId == cotizacionId &&
                                   v.VersionActual == cotizacion.VersionActual);

      if (versionVigente == null)
      {
        _logger.LogWarning("No se encontró la versión vigente para cotización {CotizacionId} con VersionActual {VersionActual}",
            cotizacionId, cotizacion.VersionActual);
        return false;
      }

      // 3. Validar que el VersionId recibido coincida con el VersionId de la versión vigente
      var esVersionVigente = versionVigente.VersionId == versionId;

      if (!esVersionVigente)
      {
        _logger.LogWarning("VersionId {VersionId} no corresponde a la versión vigente de la cotización {CotizacionId}. " +
            "VersionActual: {VersionActual}, VersionId vigente: {VersionIdVigente}",
            versionId, cotizacionId, cotizacion.VersionActual, versionVigente.VersionId);
      }
      else
      {
        _logger.LogDebug("VersionId {VersionId} validado como versión vigente de cotización {CotizacionId} " +
            "(VersionActual: {VersionActual})",
            versionId, cotizacionId, cotizacion.VersionActual);
      }

      return esVersionVigente;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al validar versión vigente para cotización {CotizacionId}", cotizacionId);
      return false;
    }
  }

  /// <summary>
  /// Genera un pedido en ERP a partir de una cotización
  /// </summary>
  public async Task<GenerarPedidoResult> GenerarPedidoAsync(string cotizacionId, int versionId)
  {
    _logger.LogInformation("Iniciando generación de pedido ERP para cotización {CotizacionId}, versión {VersionId}",
        cotizacionId, versionId);

    // Validar estado de cotización
    if (!await ValidarEstadoCotizacionParaErpAsync(cotizacionId))
    {
      _logger.LogWarning("Cotización {CotizacionId} no está en estado válido para envío a ERP", cotizacionId);
      return new GenerarPedidoResult(false, Message: "Cotización no está en estado Aceptada");
    }

    // Validar que la versión corresponda a la versión vigente de la cotización
    if (!await ValidarVersionVigenteAsync(cotizacionId, versionId))
    {
      _logger.LogWarning("Versión {VersionId} no es la versión vigente de cotización {CotizacionId}", versionId, cotizacionId);
      return new GenerarPedidoResult(false, Message: "La versión especificada no corresponde a la versión vigente de la cotización");
    }

    // Validar y obtener configuración ERP
    var erpConfig = await _erpConfigService.GetErpConfigurationAsync();
    if (!erpConfig.IsValid())
    {
      _logger.LogError("Configuración ERP no válida para cotización {CotizacionId}", cotizacionId);
      return new GenerarPedidoResult(false, Message: "Configuración ERP incompleta");
    }

    // Validar esquema ERP (CIA) para seguridad
    if (!EsEsquemaSeguro(erpConfig.Cia))
    {
      _logger.LogError("Esquema ERP no seguro: {Schema}", erpConfig.Cia);
      return new GenerarPedidoResult(false, Message: "Configuración de esquema ERP no válida");
    }

    IntegracionPedidoErp integracion;
    Guid loteId;

    // FASE A: Operaciones locales en CotizacionesWeb
    using (var transactionLocal = await _contextCotizaciones.Database.BeginTransactionAsync())
    {
      try
      {
        // a) Verificar si ya existe integración para esta cotización/versión
        integracion = await ObtenerOCrearIntegracionAsync(cotizacionId, versionId);
        loteId = integracion.LoteId;

        await transactionLocal.CommitAsync();
      }
      catch (Exception ex)
      {
        await transactionLocal.RollbackAsync();
        _logger.LogError(ex, "Error en fase local inicial para cotización {CotizacionId}", cotizacionId);
        return new GenerarPedidoResult(false, Message: $"Error en configuración inicial: {ex.Message}");
      }
    }

    // b) Obtener datos de cotización (sin transacción, solo lectura)
    var datosCompletos = await ObtenerDatosCotizacionAsync(cotizacionId, versionId);
    if (datosCompletos == null)
    {
      await ActualizarIntegracionErrorAsync(integracion.IntegracionId,
          "No se encontraron datos de la cotización");
      return new GenerarPedidoResult(false, Message: "Datos de cotización no encontrados");
    }

    // FASE B: Operaciones ERP en DbContextErp
    string? mensajeErrorErp = null;
    (bool Success, string? PedidoErp, string? Message) resultadoSP = (false, null, null);

    try
    {
      // c) Insertar en tablas staging ERP (con esquema dinámico, limpiando antes)
      await InsertarEnStagingErpAsync(loteId, datosCompletos, erpConfig);

      // d) Ejecutar stored procedure en ERP (con esquema dinámico)
      resultadoSP = await EjecutarStoredProcedureAsync(loteId, erpConfig.Cia);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error en operaciones ERP para cotización {CotizacionId}", cotizacionId);
      mensajeErrorErp = $"Error en ERP: {ex.Message}";
      resultadoSP = (false, null, mensajeErrorErp);
    }

    // FASE C: Actualización final del estado en CotizacionesWeb
    using (var transactionFinal = await _contextCotizaciones.Database.BeginTransactionAsync())
    {
      try
      {
        if (resultadoSP.Success)
        {
          // e) Actualizar como procesado exitoso
          await ActualizarIntegracionExitosoAsync(integracion.IntegracionId, resultadoSP.PedidoErp!);
          await transactionFinal.CommitAsync();

          _logger.LogInformation("Pedido ERP generado exitosamente: {PedidoErp} para cotización {CotizacionId}",
              resultadoSP.PedidoErp, cotizacionId);

          return new GenerarPedidoResult(true, resultadoSP.PedidoErp, "Pedido generado exitosamente",
              integracion.IntegracionId, loteId);
        }
        else
        {
          // f) Actualizar como error
          await ActualizarIntegracionErrorAsync(integracion.IntegracionId,
              resultadoSP.Message ?? mensajeErrorErp ?? "Error desconocido en ERP");
          await transactionFinal.CommitAsync();

          _logger.LogError("Error al generar pedido ERP para cotización {CotizacionId}: {Error}",
              cotizacionId, resultadoSP.Message ?? mensajeErrorErp);

          return new GenerarPedidoResult(false, Message: resultadoSP.Message ?? mensajeErrorErp,
              IntegracionId: integracion.IntegracionId, LoteId: loteId);
        }
      }
      catch (Exception ex)
      {
        await transactionFinal.RollbackAsync();
        _logger.LogError(ex, "Error en actualización final para cotización {CotizacionId}", cotizacionId);
        return new GenerarPedidoResult(false, Message: $"Error en actualización final: {ex.Message}",
            IntegracionId: integracion.IntegracionId, LoteId: loteId);
      }
    }
  }

  /// <summary>
  /// Valida que una cotización esté en estado válido para envío a ERP
  /// </summary>
  public async Task<bool> ValidarEstadoCotizacionParaErpAsync(string cotizacionId)
  {
    try
    {
      var cotizacion = await _contextCotizaciones.Cotizaciones
          .FirstOrDefaultAsync(c => c.CotizacionId == cotizacionId);

      if (cotizacion == null)
      {
        _logger.LogWarning("Cotización {CotizacionId} no encontrada", cotizacionId);
        return false;
      }

      var esEstadoValido = cotizacion.EstadoActual == ESTADO_ACEPTADA;

      if (!esEstadoValido)
      {
        _logger.LogInformation("Cotización {CotizacionId} en estado {Estado}, se requiere estado Aceptada",
            cotizacionId, cotizacion.EstadoActual);
      }

      return esEstadoValido;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al validar estado de cotización {CotizacionId}", cotizacionId);
      return false;
    }
  }

  /// <summary>
  /// Obtiene integración existente no procesada o con error, o crea una nueva
  /// Reutiliza el mismo LoteId para reprocesos
  /// </summary>
  private async Task<IntegracionPedidoErp> ObtenerOCrearIntegracionAsync(string cotizacionId, int versionId)
  {
    // Buscar integración existente no procesada
    var integracionExistente = await _contextCotizaciones.IntegracionesPedidoErp
        .FirstOrDefaultAsync(i => i.CotizacionId == cotizacionId &&
                                i.VersionId == versionId &&
                                (i.Estado != ESTADO_PROCESADO || i.Estado != ESTADO_NO_SINCRONIZADO));

    if (integracionExistente != null)
    {
      _logger.LogInformation("Reutilizando integración existente {IntegracionId} para reproceso con LoteId {LoteId}",
          integracionExistente.IntegracionId, integracionExistente.LoteId);

      integracionExistente.Estado = ESTADO_PENDIENTE; // Resetear a pendiente
      await _contextCotizaciones.SaveChangesAsync();

      return integracionExistente;
    }

    // Crear nueva integración
    var nuevaIntegracion = new IntegracionPedidoErp
    {
      CotizacionId = cotizacionId,
      VersionId = versionId,
      LoteId = Guid.NewGuid(), // Nuevo LoteId solo para nueva integración
      Estado = ESTADO_PENDIENTE,
      Intentos = 1,
      FechaCreacion = DateTime.Now
    };

    _contextCotizaciones.IntegracionesPedidoErp.Add(nuevaIntegracion);
    await _contextCotizaciones.SaveChangesAsync();

    _logger.LogInformation("Nueva integración creada con ID {IntegracionId} y LoteId {LoteId}",
        nuevaIntegracion.IntegracionId, nuevaIntegracion.LoteId);

    return nuevaIntegracion;
  }

  /// <summary>
  /// Obtiene los datos completos de la cotización y su versión
  /// ACTUALIZADO: Incluye PorcentajeImpuesto desde DetalleCotizacionVersion
  /// </summary>
  private async Task<CotizacionCompleta?> ObtenerDatosCotizacionAsync(string cotizacionId, int versionId)
  {
    var query = from cotizacion in _contextCotizaciones.Cotizaciones
                join version in _contextCotizaciones.CotizacionesVersiones
                    on versionId equals version.VersionId
                join interesado in _contextCotizaciones.Interesados
                    on cotizacion.InteresadoId equals interesado.InteresadoId
                where cotizacion.CotizacionId == cotizacionId
                select new CotizacionCompleta
                {
                  CotizacionId = cotizacion.CotizacionId,
                  VersionId = version.VersionId,
                  NumeroVersion = version.NumeroVersion, // Para construir observaciones
                  NombreInteresado = version.NombreInteresado,
                  EmailInteresado = version.EmailInteresado,
                  EmpresaInteresado = version.EmpresaInteresado,
                  Moneda = cotizacion.Moneda,
                  TipoCambio = version.TipoCambio ?? 1,
                  SubTotal = version.SubTotal,
                  Total = version.Total,
                  Observaciones = version.Notas ?? string.Empty,
                  Interesado = interesado
                };

    var datos = await query.FirstOrDefaultAsync();

    if (datos != null)
    {
      // Obtener las líneas de detalle en orden secuencial
      // ACTUALIZADO: Mapear PorcentajeImpuesto desde DetalleCotizacionVersion
      datos.Detalles = await _contextCotizaciones.DetallesCotizacionVersion
          .Where(d => d.VersionId == versionId)
          .OrderBy(d => d.DetalleVersionId) // Mantener orden original
          .Select(d => new DetalleCotizacionCompleto
          {
            ProductoId = d.ProductoId,
            ProductoNombre = d.Descripcion,
            Cantidad = d.Cantidad,
            PrecioUnitario = d.PrecioUnitario,
            MontoDescuento = d.Descuento,
            PorcentajeImpuesto = d.PorcentajeImpuesto, // NUEVO: Mapear desde BD
            TotalLinea = d.TotalLinea
          })
          .ToListAsync();
    }

    return datos;
  }

  /// <summary>
  /// Inserta los datos en las tablas staging del ERP usando SQL seguro con esquema dinámico
  /// ACTUALIZADO: Nuevos campos MONEDA(1), NIVEL_PRECIO(12) y PORCENTAJE_IMPUESTO
  /// </summary>
  private async Task InsertarEnStagingErpAsync(Guid loteId, CotizacionCompleta datos,
      ErpIntegrationConfig erpConfig)
  {
    _logger.LogDebug("Insertando datos en staging ERP con LoteId {LoteId} y esquema {Schema}", loteId, erpConfig.Cia);

    // Limpiar staging anterior para este lote (por si es reintento)
    await LimpiarStagingAsync(loteId, erpConfig.Cia);

    // Obtener cliente ERP desde HubSpot
    var clienteErp = await ObtenerClienteErpAsync(datos);
    if (string.IsNullOrWhiteSpace(clienteErp))
    {
      throw new InvalidOperationException($"No se pudo obtener código de cliente ERP para la cotización {datos.CotizacionId}. " +
          "Verifique que el interesado tenga configurado el código de cliente ERP en HubSpot.");
    }

    // NUEVO: Resolver MONEDA y NIVEL_PRECIO según lógica solicitada
    var (monedaErp, nivelPrecio) = await ResolverMonedaYNivelPrecioAsync(datos.Moneda);

    // Construir observaciones para el pedido ERP
    var observacionesPedido = $"Creado desde sistema de cotizaciones Web para la cotización: {datos.CotizacionId} con la versión: {datos.NumeroVersion}";

    // Insertar cabecera en COTWEB_PEDIDO_STG usando SQL seguro
    // ACTUALIZADO: Incluir campos MONEDA y NIVEL_PRECIO
    var sqlPedido = FormattableStringFactory.Create(
        $"INSERT INTO [{erpConfig.Cia}].[COTWEB_PEDIDO_STG] " +
        "(LOTE_ID, TIPO_DOCUMENTO, CLIENTE, CONDICION_PAGO, BODEGA, MONEDA, NIVEL_PRECIO, TIPO_CAMBIO, " +
        "USUARIO_ERP, ACTIVIDAD_COMERCIAL, OBSERVACIONES, FECHA_CREACION) " +
        "VALUES " +
        "({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})",
        loteId, "P", clienteErp, erpConfig.CondicionPago, erpConfig.Bodega,
        monedaErp, nivelPrecio, datos.TipoCambio, erpConfig.UsuarioERP, erpConfig.ActividadComercial,
        observacionesPedido, DateTime.Now);

    await _contextErp.Database.ExecuteSqlAsync(sqlPedido);

    // Insertar líneas en COTWEB_PEDIDO_LINEA_STG usando SQL seguro
    // ACTUALIZADO: Usar PORCENTAJE_IMPUESTO en lugar de PORCENTAJE_DESCUENTO
    int numeroLinea = 1; // Comenzar numeración en 1
    foreach (var detalle in datos.Detalles)
    {
      var sqlLinea = FormattableStringFactory.Create(
          $"INSERT INTO [{erpConfig.Cia}].[COTWEB_PEDIDO_LINEA_STG] " +
          "(LOTE_ID, LINEA, PRODUCTO, DESCRIPCION, CANTIDAD, PRECIO_UNITARIO, " +
          "MONTO_DESCUENTO, PORCENTAJE_IMPUESTO, SUBTOTAL, BODEGA) " +
          "VALUES " +
          "({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})",
          loteId, numeroLinea, detalle.ProductoId, detalle.ProductoNombre,
          detalle.Cantidad, detalle.PrecioUnitario, detalle.MontoDescuento,
          detalle.PorcentajeImpuesto, detalle.TotalLinea, erpConfig.Bodega);

      await _contextErp.Database.ExecuteSqlAsync(sqlLinea);

      numeroLinea++; // Incrementar línea después del insert
    }

    _logger.LogDebug("Datos insertados en staging: 1 cabecera, {LineasCount} líneas", datos.Detalles.Count);
  }

  /// <summary>
  /// Resuelve los valores de MONEDA y NIVEL_PRECIO según la lógica solicitada
  /// NUEVO: CRC=>"L", USD=>"D" y niveles de precio desde parámetros
  /// </summary>
  private async Task<(string MonedaErp, string NivelPrecio)> ResolverMonedaYNivelPrecioAsync(string monedaCotizacion)
  {
    string monedaErp = monedaCotizacion switch
    {
      "CRC" => "L",
      "USD" => "D",
      _ => throw new InvalidOperationException($"Moneda no soportada: {monedaCotizacion}")
    };

    string? nivelPrecio = monedaCotizacion switch
    {
      "CRC" => await _parametroSistemaService.ObtenerValorParametroAsync("ERP_NIVELPRECIO_LOCAL"),
      "USD" => await _parametroSistemaService.ObtenerValorParametroAsync("ERP_NIVELPRECIO_DOLAR"),
      _ => null
    };

    if (string.IsNullOrWhiteSpace(nivelPrecio))
    {
      throw new InvalidOperationException($"Parámetro de nivel de precio no configurado para moneda {monedaCotizacion}");
    }

    _logger.LogDebug("Moneda {Original} => ERP: {MonedaErp}, NivelPrecio: {NivelPrecio}",
        monedaCotizacion, monedaErp, nivelPrecio);

    return (monedaErp, nivelPrecio);
  }

  /// <summary>
  /// Obtiene el código de cliente ERP desde HubSpot
  /// </summary>
  private async Task<string?> ObtenerClienteErpAsync(CotizacionCompleta datos)
  {
    if (datos.Interesado == null)
    {
      _logger.LogWarning("No se encontró información del interesado para cotización {CotizacionId}", datos.CotizacionId);
      return null;
    }

    try
    {
      _logger.LogDebug("Obteniendo cliente ERP desde HubSpot para interesado {InteresadoId} (Tipo: {Tipo})",
          datos.Interesado.InteresadoId, datos.Interesado.TipoInteresado);

      var clienteErp = await _hubspotClienteErpService.ObtenerClienteErpAsync(datos.Interesado);

      if (string.IsNullOrWhiteSpace(clienteErp))
      {
        _logger.LogWarning("No se pudo obtener código de cliente ERP desde HubSpot para interesado {InteresadoId}",
            datos.Interesado.InteresadoId);
        return null;
      }

      _logger.LogInformation("Cliente ERP obtenido exitosamente: {ClienteErp} para cotización {CotizacionId}",
          clienteErp, datos.CotizacionId);

      return clienteErp;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al obtener cliente ERP para cotización {CotizacionId}", datos.CotizacionId);
      return null;
    }
  }

  /// <summary>
  /// Limpia las tablas staging para un lote específico usando esquema dinámico
  /// CORREGIDO: Vulnerabilidad de SQL injection usando FormattableString
  /// </summary>
  private async Task LimpiarStagingAsync(Guid loteId, string esquemaErp)
  {
    // El esquema ya fue validado previamente con EsEsquemaSeguro()
    // Usar FormattableString para construcción segura de SQL con esquemas dinámicos
    var deleteLineas = FormattableStringFactory.Create(
        $"DELETE FROM [{esquemaErp}].[COTWEB_PEDIDO_LINEA_STG] WHERE LOTE_ID = {{0}}", loteId);

    var deleteCabecera = FormattableStringFactory.Create(
        $"DELETE FROM [{esquemaErp}].[COTWEB_PEDIDO_STG] WHERE LOTE_ID = {{0}}", loteId);

    await _contextErp.Database.ExecuteSqlAsync(deleteLineas);
    await _contextErp.Database.ExecuteSqlAsync(deleteCabecera);

    _logger.LogDebug("Staging limpiado para LoteId {LoteId} en esquema {Schema}", loteId, esquemaErp);
  }

  /// <summary>
  /// Ejecuta el stored procedure del ERP que genera el pedido usando esquema dinámico
  /// CORREGIDO: Vulnerabilidad de SQL injection usando FormattableString
  /// </summary>
  private async Task<(bool Success, string? PedidoErp, string? Message)> EjecutarStoredProcedureAsync(Guid loteId, string esquemaErp)
  {
    try
    {
      _logger.LogDebug("Ejecutando stored procedure {Schema}.COTWEB_PROCESAR_PEDIDO_STAGE con LoteId {LoteId}", esquemaErp, loteId);

      var parametroLoteId = new SqlParameter("@LoteId", loteId);
      var parametroPedidoOutput = new SqlParameter("@PedidoGenerado", System.Data.SqlDbType.VarChar, 50)
      {
        Direction = System.Data.ParameterDirection.Output
      };

      // El esquema ya fue validado previamente con EsEsquemaSeguro()
      // Usar FormattableString para construcción segura de SQL con esquemas dinámicos
      var sqlSP = FormattableStringFactory.Create(
          $"EXEC [{esquemaErp}].[COTWEB_PROCESAR_PEDIDO_STAGE] {{0}}, {{1}} OUTPUT",
          parametroLoteId, parametroPedidoOutput);

      await _contextErp.Database.ExecuteSqlAsync(sqlSP);

      var pedidoGenerado = parametroPedidoOutput.Value?.ToString();

      var success = !string.IsNullOrEmpty(pedidoGenerado);

      // Limpiar staging para evitar dejar basura, ya que no se va a requeririr ningún intento o validación datos por error
      await LimpiarStagingAsync(loteId, esquemaErp);

      _logger.LogDebug("SP ejecutado: Success={Success}, Pedido={Pedido}",
          success, pedidoGenerado);

      return (success, pedidoGenerado, success ? "Pedido generado exitosamente" : "Error desconocido");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al ejecutar stored procedure en esquema {Schema} para LoteId {LoteId}", esquemaErp, loteId);
      return (false, null, $"Error en SP: {ex.Message}");
    }
  }

  /// <summary>
  /// Actualiza la integración como exitosa
  /// </summary>
  private async Task ActualizarIntegracionExitosoAsync(int integracionId, string pedidoErp)
  {
    var integracion = await _contextCotizaciones.IntegracionesPedidoErp
        .FirstOrDefaultAsync(i => i.IntegracionId == integracionId);

    if (integracion != null)
    {
      integracion.Estado = ESTADO_PROCESADO;
      integracion.PedidoErp = pedidoErp;
      integracion.FechaProcesado = DateTime.Now; // Llenar en éxito
      integracion.MensajeError = null;
      // No modificar Intentos en éxito

      await _contextCotizaciones.SaveChangesAsync();
      _logger.LogDebug("Integración {IntegracionId} actualizada como exitosa", integracionId);
    }
  }

  /// <summary>
  /// Actualiza la integración como error
  /// </summary>
  private async Task ActualizarIntegracionErrorAsync(int integracionId, string mensajeError)
  {
    var integracion = await _contextCotizaciones.IntegracionesPedidoErp
        .FirstOrDefaultAsync(i => i.IntegracionId == integracionId);

    if (integracion != null)
    {
      integracion.Estado = ESTADO_ERROR;
      integracion.MensajeError = mensajeError;
      integracion.FechaProcesado = DateTime.Now; // Llenar también en error
                                                 // Intentos ya se incrementa en ObtenerOCrearIntegracionAsync

      await _contextCotizaciones.SaveChangesAsync();
      _logger.LogDebug("Integración {IntegracionId} actualizada con error: {Error}", integracionId, mensajeError);
    }
  }
}

/// <summary>
/// Clase auxiliar para datos completos de cotización
/// </summary>
internal class CotizacionCompleta
{
  public string CotizacionId { get; set; } = string.Empty;
  public int VersionId { get; set; }
  public decimal NumeroVersion { get; set; } // Para construir observaciones
  public string NombreInteresado { get; set; } = string.Empty;
  public string EmailInteresado { get; set; } = string.Empty;
  public string EmpresaInteresado { get; set; } = string.Empty;
  public string Moneda { get; set; } = string.Empty;
  public decimal TipoCambio { get; set; }
  public decimal SubTotal { get; set; }
  public decimal Total { get; set; }
  public string Observaciones { get; set; } = string.Empty;
  public Interesado Interesado { get; set; } = null!; // Para obtener cliente ERP
  public List<DetalleCotizacionCompleto> Detalles { get; set; } = new();
}

/// <summary>
/// Clase auxiliar para detalles de cotización
/// ACTUALIZADO: Usar PORCENTAJE_IMPUESTO en lugar de PORCENTAJE_DESCUENTO
/// </summary>
internal class DetalleCotizacionCompleto
{
  public string ProductoId { get; set; } = string.Empty;
  public string ProductoNombre { get; set; } = string.Empty;
  public decimal Cantidad { get; set; }
  public decimal PrecioUnitario { get; set; }
  public decimal MontoDescuento { get; set; }
  public decimal PorcentajeImpuesto { get; set; } // CAMBIADO: era PorcentajeDescuento
  public decimal TotalLinea { get; set; }
}