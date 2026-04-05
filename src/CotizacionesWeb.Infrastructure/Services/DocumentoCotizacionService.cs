using CotizacionesWeb.Application.Cotizaciones;
using CotizacionesWeb.Application.Documents;
using CotizacionesWeb.Infrastructure.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace CotizacionesWeb.Infrastructure.Services;

/// <summary>
/// Motor de generación de documentos Word para cotizaciones
/// Usa OpenXML para manipular plantillas .docx
/// </summary>
public class DocumentoCotizacionService : IDocumentoCotizacionService
{
  private readonly ICotizacionService _cotizacionService;
  private readonly IParametroSistemaService _parametroService;
  private readonly ILogger<DocumentoCotizacionService> _logger;

  private const string PLACEHOLDER_DETALLE = "{{DETALLE_COTIZACION}}";
  private const string PLANTILLA_PATH = "Templates/PlantillaCotizacion.docx";

  public DocumentoCotizacionService(
      ICotizacionService cotizacionService,
      IParametroSistemaService parametroService,
      ILogger<DocumentoCotizacionService> logger)
  {
    _cotizacionService = cotizacionService;
    _parametroService = parametroService;
    _logger = logger;
  }

  public async Task<GenerarDocumentoResult> GenerarDocumentoCotizacionAsync(GenerarDocumentoCotizacionRequest request)
  {
    try
    {
      _logger.LogInformation("Iniciando generación de documento para cotización {CotizacionId}", request.CotizacionId);

      // 1. Obtener datos de la cotización
      var datosCotizacion = await ObtenerDatosCotizacionAsync(request);
      if (datosCotizacion == null)
      {
        return new GenerarDocumentoResult(false, "Cotización no encontrada", null, null);
      }

      // 2. Verificar que existe la plantilla
      var rutaPlantilla = GetRutaPlantilla();
      if (!File.Exists(rutaPlantilla))
      {
        _logger.LogError("Plantilla no encontrada en: {RutaPlantilla}", rutaPlantilla);
        return new GenerarDocumentoResult(false, "Plantilla de documento no encontrada", null, null);
      }

      // 3. Generar el documento
      var documentoGenerado = await GenerarDocumentoDesdeePlantillaAsync(rutaPlantilla, datosCotizacion);

      var fileName = $"Cotizacion_{datosCotizacion.CotizacionId}_v{datosCotizacion.Version}.docx";

      _logger.LogInformation("Documento generado exitosamente para cotización {CotizacionId}", request.CotizacionId);

      return new GenerarDocumentoResult(true, null, documentoGenerado, fileName);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al generar documento para cotización {CotizacionId}", request.CotizacionId);
      return new GenerarDocumentoResult(false, $"Error interno: {ex.Message}", null, null);
    }
  }

  public async Task<bool> ValidarPlantillaAsync()
  {
    try
    {
      var rutaPlantilla = GetRutaPlantilla();
      if (!File.Exists(rutaPlantilla))
      {
        _logger.LogWarning("Plantilla no encontrada en: {RutaPlantilla}", rutaPlantilla);
        return false;
      }

      // Validar que la plantilla se puede abrir
      using var stream = new FileStream(rutaPlantilla, FileMode.Open, FileAccess.Read);
      using var document = WordprocessingDocument.Open(stream, false);

      var body = document.MainDocumentPart?.Document?.Body;
      if (body == null)
      {
        _logger.LogError("La plantilla no tiene un cuerpo de documento válido");
        return false;
      }

      _logger.LogInformation("Plantilla validada exitosamente");
      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al validar plantilla");
      return false;
    }
  }

  public async Task<List<string>> ObtenerPlaceholdersDisponiblesAsync()
  {
    var placeholders = new List<string>();

    try
    {
      var rutaPlantilla = GetRutaPlantilla();
      if (!File.Exists(rutaPlantilla))
      {
        return placeholders;
      }

      using var stream = new FileStream(rutaPlantilla, FileMode.Open, FileAccess.Read);
      using var document = WordprocessingDocument.Open(stream, false);

      var body = document.MainDocumentPart?.Document?.Body;
      if (body != null)
      {
        var textos = body.Descendants<Text>();
        foreach (var texto in textos)
        {
          if (!string.IsNullOrEmpty(texto.Text))
          {
            var placeholdersEncontrados = ExtraerPlaceholders(texto.Text);
            placeholders.AddRange(placeholdersEncontrados);
          }
        }
      }

      return placeholders.Distinct().ToList();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al obtener placeholders");
      return placeholders;
    }
  }

  private async Task<CotizacionDocumentDto?> ObtenerDatosCotizacionAsync(GenerarDocumentoCotizacionRequest request)
  {
    try
    {
      // Obtener detalle de la cotización
      CotizacionVersionDetalleDto? detalleCotizacion;

      if (request.VersionId.HasValue)
      {
        detalleCotizacion = await _cotizacionService.GetCotizacionVersionDetailAsync(request.VersionId.Value);
      }
      else
      {
        detalleCotizacion = await _cotizacionService.GetCotizacionCurrentVersionDetailAsync(request.CotizacionId);
      }

      if (detalleCotizacion == null)
      {
        return null;
      }

      // Obtener parámetros del sistema
      var vigencia = await _parametroService.ObtenerValorParametroAsync("COT_VIGENCIA") ?? "30 días";
      var condicionesPago = await _parametroService.ObtenerValorParametroAsync("COT_CONDICIONES_PAGO") ?? "Contado";
      var notasComerciales = await _parametroService.ObtenerValorParametroAsync("COT_NOTAS_COMERCIALES") ?? "";
      var tituloDetalle = await _parametroService.ObtenerValorParametroAsync("COT_TITULO_DETALLE") ?? "";
      var tituloResumen = await _parametroService.ObtenerValorParametroAsync("COT_TITULO_RESUMEN") ?? "";

      // Convertir datos
      var version = detalleCotizacion.Version;
      var detalles = detalleCotizacion.Detalles.Select(d => new DetalleDocumentDto(
          d.ProductoId,
          d.Descripcion,
          d.Cantidad,
          d.PrecioUnitario,
          d.Cantidad * d.PrecioUnitario - d.Descuento, // Subtotal línea
          d.PorcentajeImpuesto,
          d.TotalLinea
      )).ToList();

      return new CotizacionDocumentDto(
          version.CotizacionId,
          version.NumeroVersion.ToString("0.0", CultureInfo.InvariantCulture),
          version.FechaVersion,
          version.FechaVersion.Year,
          version.NombreInteresado,
          version.EmpresaInteresado,
          version.EmailInteresado,
          version.Moneda,
          version.TipoCambio?.ToString("N2", CultureInfo.InvariantCulture) ?? "1.00",
          version.SubTotal,
          version.Impuesto,
          version.Total,
          vigencia,
          condicionesPago,
          notasComerciales,
          tituloDetalle,
          tituloResumen,
          detalles
      );
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al obtener datos de cotización {CotizacionId}", request.CotizacionId);
      return null;
    }
  }

  private async Task<byte[]> GenerarDocumentoDesdeePlantillaAsync(string rutaPlantilla, CotizacionDocumentDto datos)
  {
    using var memoryStream = new MemoryStream();

    // Copiar plantilla a memoria
    using (var plantillaStream = new FileStream(rutaPlantilla, FileMode.Open, FileAccess.Read))
    {
      await plantillaStream.CopyToAsync(memoryStream);
    }

    memoryStream.Position = 0;

    // Abrir documento en memoria para edición
    using (var document = WordprocessingDocument.Open(memoryStream, true))
    {
      var body = document.MainDocumentPart?.Document?.Body;
      if (body == null)
      {
        throw new InvalidOperationException("El documento no tiene un cuerpo válido");
      }

      // 1. Reemplazar placeholders simples
      await ReemplazarPlaceholdersSimples(body, datos);

      // 2. Generar y reemplazar bloque de detalle
      await ReemplazarBloqueDetalle(body, datos);

      // Guardar cambios
      document.MainDocumentPart.Document.Save();
    }

    return memoryStream.ToArray();
  }

  private async Task ReemplazarPlaceholdersSimples(Body body, CotizacionDocumentDto datos)
  {
    var placeholders = new Dictionary<string, string>
        {
            { "{{COTIZACION_ID}}", datos.CotizacionId },
            { "{{VERSION}}", $"v{datos.Version}" },
            { "{{FECHA_COTIZACION}}", datos.FechaCotizacion.ToString("dd/MM/yyyy") },
            { "{{ANO}}", datos.ano.ToString() },
            { "{{CLIENTE}}", datos.Cliente },
            { "{{EMPRESA}}", datos.Empresa },
            { "{{EMAIL}}", datos.Email??"N/A" },
            { "{{MONEDA}}", datos.Moneda },
            { "{{TIPO_CAMBIO}}", datos.TipoCambio },
            { "{{VIGENCIA}}", datos.Vigencia },
            { "{{CONDICIONES_PAGO}}", datos.CondicionesPago },
            { "{{NOTAS_COMERCIALES}}", datos.NotasComerciales },
            { "{{TITULO_DETALLE}}", datos.TituloDetalle },
            { "{{TITULO_RESUMEN}}", datos.TituloResumen },
            { "{{SUBTOTAL}}", FormatearMoneda(datos.SubTotal, datos.Moneda) },
            { "{{IMPUESTO_TOTAL}}", FormatearMoneda(datos.ImpuestoTotal, datos.Moneda) },
            { "{{TOTAL}}", FormatearMoneda(datos.Total, datos.Moneda) }
        };

    foreach (var placeholder in placeholders)
    {
      ReemplazarTextoEnDocumento(body, placeholder.Key, placeholder.Value);
    }

    _logger.LogDebug("Placeholders simples reemplazados: {Count}", placeholders.Count);
  }

  private async Task ReemplazarBloqueDetalle(Body body, CotizacionDocumentDto datos)
  {
    _logger.LogDebug("Iniciando reemplazo de bloque de detalle");

    // Buscar párrafos que contengan el placeholder de detalle
    var parrafoDetalle = EncontrarParrafoConTexto(body, PLACEHOLDER_DETALLE);
    if (parrafoDetalle == null)
    {
      _logger.LogWarning("Placeholder {Placeholder} no encontrado en el documento", PLACEHOLDER_DETALLE);
      return;
    }

    _logger.LogDebug("Placeholder de detalle encontrado, eliminando párrafo original");

    // Obtener título de detalle desde parámetros
    var tituloDetalle = datos.TituloDetalle ?? "DETALLE DE PRODUCTOS Y SERVICIOS";

    // Crear nuevos párrafos para el detalle
    var nuevosElementos = new List<Paragraph>();

    // Título de la sección de detalle
    nuevosElementos.Add(CrearParrafoTitulo(tituloDetalle));
    //nuevosElementos.Add(CrearParrafoVacio()); // Espacio

    // Generar bloque por cada línea de detalle
    for (int i = 0; i < datos.Detalles.Count; i++)
    {
      var detalle = datos.Detalles[i];
      nuevosElementos.AddRange(GenerarBloqueLineaDetalle(detalle, datos.Moneda));

      // Agregar separación visual entre bloques (excepto el último)
      if (i < datos.Detalles.Count - 1)
      {
        nuevosElementos.Add(CrearLineDivisor());
        nuevosElementos.Add(CrearParrafoVacio());
      }
    }

    // Reemplazar el párrafo original con los nuevos elementos
    var parent = parrafoDetalle.Parent;
    if (parent != null)
    {
      _logger.LogDebug("Insertando {Count} nuevos párrafos de detalle", nuevosElementos.Count);

      foreach (var elemento in nuevosElementos)
      {
        parent.InsertBefore(elemento, parrafoDetalle);
      }

      // Eliminar completamente el párrafo original
      parent.RemoveChild(parrafoDetalle);

      _logger.LogDebug("Párrafo original eliminado exitosamente");
    }

    _logger.LogDebug("Bloque de detalle generado con {Count} líneas", datos.Detalles.Count);
  }

  /// <summary>
  /// Buscar párrafo que contenga el texto especificado (maneja texto fragmentado)
  /// </summary>
  private Paragraph? EncontrarParrafoConTexto(Body body, string textoBuscado)
  {
    var parrafos = body.Descendants<Paragraph>().ToList();

    foreach (var parrafo in parrafos)
    {
      var textoCompleto = string.Concat(parrafo.Descendants<Text>().Select(t => t.Text ?? ""));

      if (textoCompleto.Contains(textoBuscado))
      {
        _logger.LogDebug("Párrafo con texto encontrado: {Texto}", textoCompleto);
        return parrafo;
      }
    }

    return null;
  }

  private List<Paragraph> GenerarBloqueLineaDetalle(DetalleDocumentDto detalle, string moneda)
  {
    var elementos = new List<Paragraph>();

    // 1. Producto (en negrita con tamaño mayor)
    elementos.Add(CrearParrafoProducto(detalle.Descripcion));

    // 2. Descripción (en cursiva, con indentación)
    if (!string.IsNullOrWhiteSpace(detalle.Producto))
    {
      elementos.Add(CrearParrafoDescripcion(detalle.Producto));
    }

    // 3. Información comercial en formato tabular visual
    elementos.Add(CrearParrafoInfoComercial("Cantidad:", detalle.Cantidad.ToString("N2")));
    elementos.Add(CrearParrafoInfoComercial("Precio unitario:", FormatearMoneda(detalle.PrecioUnitario, moneda)));
    elementos.Add(CrearParrafoInfoComercial("Subtotal:", FormatearMoneda(detalle.SubtotalLinea, moneda)));

    if (detalle.PorcentajeImpuesto > 0)
    {
      var impuestoLinea = detalle.SubtotalLinea * detalle.PorcentajeImpuesto / 100;
      elementos.Add(CrearParrafoInfoComercial($"Impuesto ({detalle.PorcentajeImpuesto:N1}%):", FormatearMoneda(impuestoLinea, moneda)));
    }

    // Total en negrita
    elementos.Add(CrearParrafoTotal("Total línea:", FormatearMoneda(detalle.TotalLinea, moneda)));

    return elementos;
  }

  /// <summary>
  /// Crear párrafo para nombre de producto con formato destacado
  /// </summary>
  private Paragraph CrearParrafoProducto(string producto)
  {
    return new Paragraph(
        new ParagraphProperties(
            new SpacingBetweenLines() { After = "100" }
        ),
        new Run(
            new RunProperties(
                new Bold(),
                new FontSize { Val = "28" }, // 14pt
                new Color { Val = "1F497D" } // Azul oscuro
            ),
            new Text(producto)
        )
    );
  }

  /// <summary>
  /// Crear párrafo para descripción con formato sutil
  /// </summary>
  private Paragraph CrearParrafoDescripcion(string descripcion)
  {
    return new Paragraph(
        new ParagraphProperties(
            new Indentation { Left = "360" }, // Indentación de 0.25"
            new SpacingBetweenLines() { After = "80" }
        ),
        new Run(
            new RunProperties(
                new Italic(),
                new Color { Val = "666666" } // Gris
            ),
            new Text(descripcion)
        )
    );
  }

  /// <summary>
  /// Crear párrafo para información comercial con formato tabular
  /// </summary>
  private Paragraph CrearParrafoInfoComercial(string etiqueta, string valor)
  {
    return new Paragraph(
        new ParagraphProperties(
            new Indentation { Left = "720" }, // Indentación de 0.5"
            new SpacingBetweenLines() { After = "60" }
        ),
        new Run(
            new RunProperties(new FontSize { Val = "20" }), // 10pt
            new Text(etiqueta)
        ),
        new Run(
            new RunProperties(
                new FontSize { Val = "20" },
                new Bold()
            ),
            new Text($" {valor}")
        )
    );
  }

  /// <summary>
  /// Crear párrafo para total con formato destacado
  /// </summary>
  private Paragraph CrearParrafoTotal(string etiqueta, string valor)
  {
    return new Paragraph(
        new ParagraphProperties(
            new Indentation { Left = "720" },
            new SpacingBetweenLines() { After = "120" }
        ),
        new Run(
            new RunProperties(
                new Bold(),
                new FontSize { Val = "22" },
                new Color { Val = "1F497D" }
            ),
            new Text($"{etiqueta} {valor}")
        )
    );
  }

  private Paragraph CrearParrafoTitulo(string texto)
  {
    return new Paragraph(
        new ParagraphProperties(
            new ParagraphStyleId { Val = "Heading1" },
            new SpacingBetweenLines() { After = "240" } // Espacio adicional después del título
        ),
        new Run(
            new RunProperties(
                new Bold(),
                new FontSize { Val = "36" }, // 14pt
                new Color { Val = "1F497D" } // Azul corporativo
            ),
            new Text(texto)
        )
    );
  }

  private Paragraph CrearParrafoConFormato(string texto, bool bold = false, bool italic = false)
  {
    var runProperties = new RunProperties();

    if (bold) runProperties.Append(new Bold());
    if (italic) runProperties.Append(new Italic());

    return new Paragraph(
        new Run(runProperties, new Text(texto))
    );
  }

  private Paragraph CrearParrafoVacio()
  {
    return new Paragraph(
        new ParagraphProperties(
            new SpacingBetweenLines() { After = "120" } // Espacio pequeño
        ),
        new Run(new Text(""))
    );
  }

  private void ReemplazarTextoEnDocumento(Body body, string placeholder, string reemplazo)
  {
    _logger.LogDebug("Reemplazando placeholder: {Placeholder} por {Reemplazo}", placeholder, reemplazo);

    // Estrategia híbrida: primero intentar reemplazo preservando formato, luego fallback robusto
    bool reemplazadoExitoso = ReemplazarTextoConservandoFormato(body, placeholder, reemplazo);

    if (!reemplazadoExitoso)
    {
      _logger.LogDebug("Reemplazo simple falló, usando método robusto como fallback");
      ReemplazarTextoRobustoFallback(body, placeholder, reemplazo);
    }
  }

  /// <summary>
  /// Intenta reemplazar placeholders conservando el formato original de la plantilla
  /// Recorre runs individuales para mantener estilos, negritas, colores, etc.
  /// </summary>
  private bool ReemplazarTextoConservandoFormato(OpenXmlElement elemento, string placeholder, string reemplazo)
  {
    bool reemplazadoAlMenosUno = false;
    var textsElements = elemento.Descendants<Text>().ToList();

    foreach (var textElement in textsElements)
    {
      if (!string.IsNullOrEmpty(textElement.Text) && textElement.Text.Contains(placeholder))
      {
        _logger.LogDebug("Placeholder encontrado en Text element: {Texto}", textElement.Text);

        // Reemplazar SOLO el contenido del Text, manteniendo el Run y su formato
        textElement.Text = textElement.Text.Replace(placeholder, reemplazo);
        reemplazadoAlMenosUno = true;

        _logger.LogDebug("Texto reemplazado conservando formato: {NuevoTexto}", textElement.Text);
      }
    }

    // Si no se encontró en runs individuales, intentar buscar en párrafos completos
    // para casos de placeholders fragmentados
    if (!reemplazadoAlMenosUno)
    {
      reemplazadoAlMenosUno = ReemplazarTextoFragmentado(elemento, placeholder, reemplazo);
    }

    return reemplazadoAlMenosUno;
  }

  /// <summary>
  /// Intenta reemplazar texto que puede estar fragmentado entre múltiples runs
  /// conservando el formato del primer run donde aparece el placeholder
  /// </summary>
  private bool ReemplazarTextoFragmentado(OpenXmlElement elemento, string placeholder, string reemplazo)
  {
    var parrafos = elemento.Descendants<Paragraph>().ToList();
    bool reemplazadoAlMenosUno = false;

    foreach (var parrafo in parrafos)
    {
      var textoCompleto = string.Concat(parrafo.Descendants<Text>().Select(t => t.Text ?? ""));

      if (textoCompleto.Contains(placeholder))
      {
        _logger.LogDebug("Placeholder fragmentado encontrado en párrafo: {Texto}", textoCompleto);

        // Buscar el run que contiene el inicio del placeholder
        var runs = parrafo.Elements<Run>().ToList();
        var runFormato = EncontrarRunConFormatoPlaceholder(runs, placeholder);

        if (runFormato != null)
        {
          // Crear nuevo run con el texto reemplazado usando el formato del run original
          var nuevoTexto = textoCompleto.Replace(placeholder, reemplazo);
          var runProperties = runFormato.Elements<RunProperties>().FirstOrDefault()?.CloneNode(true) as RunProperties;

          // Limpiar todos los runs existentes
          var runsExistentes = parrafo.Elements<Run>().ToList();
          foreach (var run in runsExistentes)
          {
            run.Remove();
          }

          // Crear nuevo run con formato preservado
          var nuevoRun = new Run();
          if (runProperties != null)
          {
            nuevoRun.Append(runProperties);
          }
          nuevoRun.Append(new Text(nuevoTexto));
          parrafo.Append(nuevoRun);

          _logger.LogDebug("Placeholder fragmentado reemplazado conservando formato: {NuevoTexto}", nuevoTexto);
          reemplazadoAlMenosUno = true;
        }
      }
    }

    return reemplazadoAlMenosUno;
  }

  /// <summary>
  /// Encuentra el run que contiene el formato a preservar para un placeholder
  /// </summary>
  private Run? EncontrarRunConFormatoPlaceholder(List<Run> runs, string placeholder)
  {
    // Buscar el run que contiene el inicio del placeholder
    foreach (var run in runs)
    {
      var textoRun = string.Concat(run.Descendants<Text>().Select(t => t.Text ?? ""));
      if (textoRun.Contains("{{") || textoRun.Contains(placeholder.Substring(0, Math.Min(placeholder.Length, 5))))
      {
        return run;
      }
    }

    // Si no se encuentra, retornar el primer run con formato
    return runs.FirstOrDefault(r => r.Elements<RunProperties>().Any()) ?? runs.FirstOrDefault();
  }

  /// <summary>
  /// Método robusto como fallback cuando el reemplazo conservando formato falla
  /// </summary>
  private void ReemplazarTextoRobustoFallback(OpenXmlElement elemento, string placeholder, string reemplazo)
  {
    var parrafos = elemento.Descendants<Paragraph>().ToList();

    foreach (var parrafo in parrafos)
    {
      var textoCompleto = string.Concat(parrafo.Descendants<Text>().Select(t => t.Text ?? ""));

      if (textoCompleto.Contains(placeholder))
      {
        _logger.LogDebug("Usando fallback robusto para párrafo: {Texto}", textoCompleto);

        var nuevoTexto = textoCompleto.Replace(placeholder, reemplazo);

        // Intentar preservar al menos las propiedades del párrafo
        var paragraphProperties = parrafo.Elements<ParagraphProperties>().FirstOrDefault()?.CloneNode(true) as ParagraphProperties;

        // Limpiar runs existentes
        var runsExistentes = parrafo.Elements<Run>().ToList();
        foreach (var run in runsExistentes)
        {
          run.Remove();
        }

        // Crear nuevo run básico
        var nuevoRun = new Run(new Text(nuevoTexto));
        parrafo.Append(nuevoRun);

        _logger.LogDebug("Fallback aplicado - formato básico preservado: {NuevoTexto}", nuevoTexto);
      }
    }
  }

  private Text? EncontrarElementoConTexto(Body body, string textoBuscado)
  {
    // Buscar en párrafos completos para manejar texto fragmentado
    var parrafos = body.Descendants<Paragraph>().ToList();

    foreach (var parrafo in parrafos)
    {
      var textoCompleto = string.Concat(parrafo.Descendants<Text>().Select(t => t.Text ?? ""));

      if (textoCompleto.Contains(textoBuscado))
      {
        _logger.LogDebug("Texto buscado encontrado en párrafo: {Texto}", textoCompleto);
        // Retornar el primer Text element del párrafo para referencia
        return parrafo.Descendants<Text>().FirstOrDefault();
      }
    }

    _logger.LogWarning("Texto buscado no encontrado: {TextoBuscado}", textoBuscado);
    return null;
  }

  private List<string> ExtraerPlaceholders(string texto)
  {
    var placeholders = new List<string>();

    // Buscar patrones {{TEXTO}}
    var inicio = texto.IndexOf("{{");
    while (inicio >= 0)
    {
      var fin = texto.IndexOf("}}", inicio);
      if (fin > inicio)
      {
        var placeholder = texto.Substring(inicio, fin - inicio + 2);
        placeholders.Add(placeholder);
        inicio = texto.IndexOf("{{", fin);
      }
      else
      {
        break;
      }
    }

    return placeholders;
  }

  private string FormatearMoneda(decimal valor, string moneda)
  {
    // Configurar formato según moneda
    var cultura = moneda switch
    {
      "CRC" => new CultureInfo("es-CR"), // Costa Rica
      "USD" => new CultureInfo("en-US"), // Estados Unidos
      "EUR" => new CultureInfo("es-ES"), // España/Euro
      _ => CultureInfo.InvariantCulture
    };

    var simbolo = moneda switch
    {
      "CRC" => "¢",
      "USD" => "$",
      "EUR" => "€",
      "GBP" => "£",
      _ => moneda + " "
    };

    // Formatear con la cultura apropiada
    var valorFormateado = valor.ToString("N2", cultura);

    return $"{simbolo}{valorFormateado}";
  }

  /// <summary>
  /// Crear un párrafo con línea divisoria para separar bloques
  /// </summary>
  private Paragraph CrearLineDivisor()
  {
    return new Paragraph(
        new ParagraphProperties(
            new ParagraphBorders(
                new BottomBorder()
                {
                  Val = BorderValues.Single,
                  Size = 4,
                  Space = 1,
                  Color = "CCCCCC"
                }
            )
        ),
        new Run(new Text(""))
    );
  }

  /// <summary>
  /// Crear párrafo vacío con espacio adicional
  /// </summary>
  private Paragraph CrearParrafoEspaciado()
  {
    return new Paragraph(
        new ParagraphProperties(
            new SpacingBetweenLines() { After = "200" } // 10pt de espacio adicional
        ),
        new Run(new Text(""))
    );
  }

  private string GetRutaPlantilla()
  {
    // Buscar la plantilla relativa al directorio de la aplicación
    var baseDirectory = AppContext.BaseDirectory;
    var templatePath = Path.Combine(baseDirectory, PLANTILLA_PATH);

    // Si no existe en BaseDirectory, buscar en directorio de contenido web
    if (!File.Exists(templatePath))
    {
      var contentRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "PlantillaCotizacion.docx");
      if (File.Exists(contentRoot))
      {
        return contentRoot;
      }
    }

    return templatePath;
  }
}