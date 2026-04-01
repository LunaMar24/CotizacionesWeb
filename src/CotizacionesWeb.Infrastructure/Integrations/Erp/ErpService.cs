using CotizacionesWeb.Application.Integrations;
using CotizacionesWeb.Domain.Entities.ERP;
using CotizacionesWeb.Infrastructure.Data;
using CotizacionesWeb.Infrastructure.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text.RegularExpressions;

namespace CotizacionesWeb.Infrastructure.Integrations.Erp;

public class ErpService : IErpService
{
  private readonly ILogger<ErpService> _logger;
  private readonly DbContextErp _dbContextErp;
  private readonly IParametroSistemaService _parametroSistemaService;

  public ErpService(ILogger<ErpService> logger, DbContextErp dbContextErp, IParametroSistemaService parametroSistemaService)
  {
    _logger = logger;
    _dbContextErp = dbContextErp;
    _parametroSistemaService = parametroSistemaService;
  }

  public async Task<List<ProductoErp>> ObtenerProductosAsync(
    string monedaCotizacion,
    string? textoBusqueda = null)
  {
    _logger.LogInformation(
        "Iniciando consulta ERP de productos. MonedaCotizacion: {Moneda}, TextoBusqueda: {Busqueda}",
        monedaCotizacion,
        textoBusqueda);

    try
    {
      // 1. Obtener parámetros
      var erpCia = await _parametroSistemaService.ObtenerValorParametroAsync("ERP_CIA");
      var nivelPrecioLocal = await _parametroSistemaService.ObtenerValorParametroAsync("ERP_NIVELPRECIO_LOCAL");
      var nivelPrecioDolar = await _parametroSistemaService.ObtenerValorParametroAsync("ERP_NIVELPRECIO_DOLAR");

      if (string.IsNullOrWhiteSpace(erpCia))
      {
        _logger.LogError("ERP_CIA no configurado.");
        throw new InvalidOperationException("Parámetro ERP_CIA no configurado.");
      }

      // 2. Validar schema
      if (!Regex.IsMatch(erpCia, @"^[A-Za-z0-9_]+$"))
      {
        _logger.LogError("Valor inválido para ERP_CIA: {ErpCia}", erpCia);
        throw new InvalidOperationException("Valor inválido para ERP_CIA.");
      }

      // 3. Resolver moneda ERP
      string monedaErp = monedaCotizacion switch
      {
        "CRC" => "CRC",
        "DOL" => "DOL",
        _ => throw new InvalidOperationException("Moneda de cotización no soportada.")
      };

      // 4. Resolver nivel de precio
      string nivelPrecio = monedaCotizacion switch
      {
        "CRC" => nivelPrecioLocal ?? throw new InvalidOperationException("ERP_NIVELPRECIO_LOCAL no configurado."),
        "DOL" => nivelPrecioDolar ?? throw new InvalidOperationException("ERP_NIVELPRECIO_DOLAR no configurado."),
        _ => throw new InvalidOperationException("Moneda no válida.")
      };

      _logger.LogInformation(
          "Consulta ERP - Schema: {Schema}, MonedaERP: {MonedaERP}, NivelPrecio: {NivelPrecio}",
          erpCia,
          monedaErp,
          nivelPrecio);

      // 5. Construir SQL
      var sql = $@"
        SELECT 
            Producto,
            Descripcion,
            CodigoImpuesto,
            Tarifa,
            Tipo,
            Porcentaje,
            Precio,
            NivelPrecio,
            Moneda
        FROM {erpCia}.vCotWebInformacionProductosERP
        WHERE NivelPrecio = @NivelPrecio
          AND Moneda = @Moneda
        ";

      if (!string.IsNullOrWhiteSpace(textoBusqueda))
      {
        sql += @"
          AND (
                Producto LIKE @TextoBusqueda
             OR Descripcion LIKE @TextoBusqueda
          )";
      }

      sql += " ORDER BY Producto";

      var resultado = new List<ProductoErp>();

      var connection = _dbContextErp.Database.GetDbConnection();

      if (connection.State != ConnectionState.Open)
        await connection.OpenAsync();

      using var command = connection.CreateCommand();
      command.CommandText = sql;
      command.CommandType = CommandType.Text;

      // Parámetros
      var paramNivel = command.CreateParameter();
      paramNivel.ParameterName = "@NivelPrecio";
      paramNivel.Value = nivelPrecio;
      command.Parameters.Add(paramNivel);

      var paramMoneda = command.CreateParameter();
      paramMoneda.ParameterName = "@Moneda";
      paramMoneda.Value = monedaErp;
      command.Parameters.Add(paramMoneda);

      if (!string.IsNullOrWhiteSpace(textoBusqueda))
      {
        var paramBusqueda = command.CreateParameter();
        paramBusqueda.ParameterName = "@TextoBusqueda";
        paramBusqueda.Value = $"%{textoBusqueda.Trim()}%";
        command.Parameters.Add(paramBusqueda);
      }

      using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync())
      {
        resultado.Add(new ProductoErp
        {
          Producto = reader["Producto"]?.ToString() ?? string.Empty,
          Descripcion = reader["Descripcion"]?.ToString() ?? string.Empty,
          CodigoImpuesto = reader["CodigoImpuesto"]?.ToString() ?? string.Empty,
          Tarifa = reader["Tarifa"]?.ToString() ?? string.Empty,
          Tipo = reader["Tipo"]?.ToString() ?? string.Empty,
          Porcentaje = reader["Porcentaje"] != DBNull.Value ? Convert.ToDecimal(reader["Porcentaje"]) : 0,
          Precio = reader["Precio"] != DBNull.Value ? Convert.ToDecimal(reader["Precio"]) : null,
          NivelPrecio = reader["NivelPrecio"]?.ToString(),
          Moneda = reader["Moneda"]?.ToString()
        });
      }

      _logger.LogInformation(
          "Consulta ERP finalizada. Registros obtenidos: {Cantidad}",
          resultado.Count);

      return resultado;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex,
          "Error al consultar productos ERP. Moneda: {Moneda}, TextoBusqueda: {Busqueda}",
          monedaCotizacion,
          textoBusqueda);

      throw;
    }
  }
}
