using CotizacionesWeb.Application.Integrations.Erp.Services;

namespace CotizacionesWeb.Infrastructure.Integrations.Erp.Services;

/// <summary>
/// Ejemplos de uso del servicio ErpPedidoService
/// </summary>
public static class ErpPedidoServiceExamples
{
    /// <summary>
    /// Ejemplo de uso típico del servicio
    /// </summary>
    /// <param name="erpPedidoService">Instancia del servicio</param>
    /// <param name="cotizacionId">ID de la cotización</param>
    /// <param name="versionId">ID de la versión</param>
    public static async Task<string> EjemploUsoBasicoAsync(
        IErpPedidoService erpPedidoService,
        string cotizacionId,
        int versionId)
    {
        /*
        // Ejemplo de uso desde un controller:
        
        // 1. Validar estado antes de intentar generar pedido
        var esEstadoValido = await erpPedidoService.ValidarEstadoCotizacionParaErpAsync(cotizacionId);
        if (!esEstadoValido)
        {
            return "Error: La cotización debe estar en estado 'Aceptada' para envío a ERP";
        }

        // 2. Generar pedido en ERP (incluye validación de versión vigente automáticamente)
        var resultado = await erpPedidoService.GenerarPedidoAsync(cotizacionId, versionId);
        
        // 3. Manejar resultado
        if (resultado.Success)
        {
            return $"Pedido generado exitosamente: {resultado.PedidoErp}";
        }
        else
        {
            // Los posibles errores incluyen:
            // - "Cotización no está en estado Aceptada"
            // - "La versión especificada no corresponde a la versión vigente de la cotización"
            // - "Configuración ERP incompleta"
            // - "Configuración de esquema ERP no válida"
            return $"Error al generar pedido: {resultado.Message}";
        }
        */

        // Para propósitos de documentación
        await Task.CompletedTask;
        return "Ver código comentado arriba para ejemplo real";
    }

    /// <summary>
    /// Flujo completo con manejo de errores
    /// </summary>
    public static async Task<string> EjemploFlujoCompletoAsync(
        IErpPedidoService erpPedidoService,
        string cotizacionId,
        int versionId)
    {
        /*
        try
        {
            // Paso 1: Validar prerrequisitos
            var esValido = await erpPedidoService.ValidarEstadoCotizacionParaErpAsync(cotizacionId);
            if (!esValido)
            {
                return Json(new { success = false, message = "Cotización no está en estado válido para ERP" });
            }

            // Paso 2: Generar pedido
            var resultado = await erpPedidoService.GenerarPedidoAsync(cotizacionId, versionId);

            // Paso 3: Procesar resultado
            if (resultado.Success)
            {
                // Opcional: Actualizar estado de cotización a "Enviado a ERP"
                // await cotizacionService.MarcarEnvioERPAsync(cotizacionId);

                return Json(new 
                { 
                    success = true, 
                    message = "Pedido generado exitosamente", 
                    pedidoErp = resultado.PedidoErp,
                    integracionId = resultado.IntegracionId
                });
            }
            else
            {
                return Json(new 
                { 
                    success = false, 
                    message = resultado.Message,
                    integracionId = resultado.IntegracionId,
                    loteId = resultado.LoteId
                });
            }
        }
        catch (Exception ex)
        {
            // Log del error
            // _logger.LogError(ex, "Error inesperado al generar pedido ERP");
            
            return Json(new { success = false, message = "Error interno del sistema" });
        }
        */

        await Task.CompletedTask;
        return "Ver código comentado arriba para ejemplo real";
    }

    /// <summary>
    /// Estados de la integración para monitoreo
    /// </summary>
    public static Dictionary<string, string> EstadosIntegracion => new()
    {
        ["Pendiente"] = "Integración creada, esperando procesamiento",
        ["Procesado"] = "Pedido generado exitosamente en ERP",
        ["Error"] = "Error al procesar, revisar mensaje de error"
    };

    /// <summary>
    /// Requisitos de configuración ERP
    /// </summary>
    public static List<string> ParametrosErpRequeridos => new()
    {
        "ERP_CIA - Compañía/Esquema en ERP (solo letras, números y underscore)",
        "ERP_CONDICION_PAGO - Condición de pago por defecto",
        "ERP_BODEGA - Bodega por defecto para pedidos",
        "ERP_USUARIO - Usuario ERP que crea los pedidos",
        "ERP_ACTIVIDAD_COMERCIAL - Actividad comercial por defecto",
        "HUBSPOT_CONTACTO_CAMPO_CLIENTE_ERP - Campo en contactos HubSpot para código ERP",
        "HUBSPOT_EMPRESA_CAMPO_CLIENTE_ERP - Campo en empresas HubSpot para código ERP"
    };

    /// <summary>
    /// Notas sobre la arquitectura del servicio
    /// </summary>
    public static Dictionary<string, string> NotasArquitectura => new()
    {
        ["Transacciones separadas"] = "No usa transacciones distribuidas. Separa operaciones locales de ERP.",
        ["Esquema dinámico"] = "Usa ERP_CIA como esquema dinámico con validación de seguridad.",
        ["Control por fases"] = "Fase A: Local, Fase B: ERP, Fase C: Actualización final.",
        ["Tabla de control"] = "IntegracionPedidoErp es la fuente de verdad del proceso.",
        ["Reintentos inteligentes"] = "Reutiliza LoteId existente para reprocesos, no genera nuevos GUIDs.",
        ["Seguridad SQL"] = "Valida esquemas para prevenir inyección SQL en nombres dinámicos.",
        ["Staging con SQL directo"] = "Usa SQL directo en lugar de DbSet para esquemas dinámicos.",
        ["Cliente ERP desde HubSpot"] = "Obtiene código de cliente ERP desde campos configurables en HubSpot.",
        ["Validación cliente obligatoria"] = "Si no se obtiene cliente ERP válido, se detiene el proceso.",
        ["FechaProcesado siempre"] = "Se llena tanto en éxito como en error para auditoría completa.",
        ["Conteo de intentos correcto"] = "Se incrementa solo en reprocesos, no en actualizaciones de estado."
    };

    /// <summary>
    /// Flujo de reproceso mejorado
    /// </summary>
    public static Dictionary<string, string> FlujoReproceso => new()
    {
        ["1. Detección"] = "Si existe integración no procesada para cotización/versión, la reutiliza",
        ["2. LoteId constante"] = "Mantiene el mismo LoteId para el reproceso",
        ["3. Limpieza staging"] = "Limpia tablas staging por LoteId antes de insertar",
        ["4. Incremento intentos"] = "Incrementa contador de intentos solo en reproceso",
        ["5. Estado reset"] = "Cambia estado a 'Pendiente' antes de reintento",
        ["6. Auditoría completa"] = "FechaProcesado se actualiza siempre para trazabilidad"
    };

    /// <summary>
    /// Validaciones implementadas en el servicio
    /// </summary>
    public static Dictionary<string, string> ValidacionesServicio => new()
    {
        ["Estado cotización"] = "Debe estar en estado 'Aceptada' para envío a ERP",
        ["Versión vigente"] = "Versión especificada debe corresponder a la versión vigente de la cotización",
        ["Configuración ERP"] = "Todos los parámetros ERP deben estar configurados y ser válidos",
        ["Esquema seguro"] = "ERP_CIA debe contener solo caracteres alfanuméricos y underscore",
        ["Datos cotización"] = "Cotización y versión deben existir con datos completos",
        ["Cliente ERP obligatorio"] = "Se debe obtener código de cliente ERP válido desde HubSpot antes de continuar",
        ["Interesado vinculado"] = "La cotización debe tener un interesado asociado con datos de HubSpot válidos"
    };

    /// <summary>
    /// Flujo de obtención de cliente ERP desde HubSpot
    /// </summary>
    public static Dictionary<string, string> FlujoClienteErp => new()
    {
        ["1. Obtener Interesado"] = "Se consulta el interesado asociado a la cotización/versión",
        ["2. Determinar tipo"] = "Se identifica si es Persona (P) o Empresa (E)",
        ["3. Campo HubSpot"] = "Se usa HUBSPOT_CONTACTO_CAMPO_CLIENTE_ERP o HUBSPOT_EMPRESA_CAMPO_CLIENTE_ERP",
        ["4. Consultar HubSpot"] = "Se obtiene el objeto específico desde la API de HubSpot",
        ["5. Extraer código"] = "Se extrae el valor del campo configurado",
        ["6. Validar resultado"] = "Si no se obtiene código válido, se detiene el proceso con error claro"
    };
}