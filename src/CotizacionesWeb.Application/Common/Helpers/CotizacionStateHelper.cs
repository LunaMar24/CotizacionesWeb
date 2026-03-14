using CotizacionesWeb.Domain.Entities;
using CotizacionesWeb.Domain.Enums;

namespace CotizacionesWeb.Application.Common.Helpers;

/// <summary>
/// Helper para actualizar campos de una cotización según el cambio de estado
/// </summary>
public static class CotizacionStateHelper
{
    /// <summary>
    /// Actualiza los campos de estado de una cotización
    /// </summary>
    /// <param name="cotizacion">Cotización a actualizar</param>
    /// <param name="newState">Nuevo estado</param>
    public static void UpdateStateFields(Cotizacion cotizacion, char newState)
    {
        cotizacion.EstadoActual = newState;

        switch (newState)
        {
            case 'T': // Aceptada
                cotizacion.FechaAceptacion = DateTime.Now;
                cotizacion.FechaRechazo = null; // Limpiar fecha de rechazo si existía
                break;

            case 'R': // Rechazada
                cotizacion.FechaRechazo = DateTime.Now;
                cotizacion.FechaAceptacion = null; // Limpiar fecha de aceptación si existía
                break;

            default:
                // Otros estados no afectan FechaAceptacion ni FechaRechazo
                break;
        }
    }

    /// <summary>
    /// Marca una cotización como enviada al ERP
    /// </summary>
    /// <param name="cotizacion">Cotización a marcar</param>
    public static void MarkAsSentToERP(Cotizacion cotizacion)
    {
        cotizacion.EnviadoERP = 'S';
        cotizacion.FechaEnvioERP = DateTime.Now;
    }

    /// <summary>
    /// Valida la consistencia de los campos de estado
    /// </summary>
    /// <param name="cotizacion">Cotización a validar</param>
    /// <returns>Lista de errores de validación</returns>
    public static List<string> ValidateStateConsistency(Cotizacion cotizacion)
    {
        var errors = new List<string>();

        // Si EstadoActual = T, entonces FechaAceptacion debe tener valor
        if (cotizacion.EstadoActual == 'T' && !cotizacion.FechaAceptacion.HasValue)
        {
            errors.Add("Una cotización aceptada debe tener fecha de aceptación");
        }

        // Si EstadoActual = R, entonces FechaRechazo debe tener valor
        if (cotizacion.EstadoActual == 'R' && !cotizacion.FechaRechazo.HasValue)
        {
            errors.Add("Una cotización rechazada debe tener fecha de rechazo");
        }

        // Si EnviadoERP = S, entonces FechaEnvioERP debe tener valor
        if (cotizacion.EnviadoERP == 'S' && !cotizacion.FechaEnvioERP.HasValue)
        {
            errors.Add("Una cotización enviada al ERP debe tener fecha de envío al ERP");
        }

        // Si EnviadoERP = N, entonces FechaEnvioERP debe ser null
        if (cotizacion.EnviadoERP == 'N' && cotizacion.FechaEnvioERP.HasValue)
        {
            errors.Add("Una cotización no enviada al ERP no debe tener fecha de envío al ERP");
        }

        // Una cotización aceptada y rechazada al mismo tiempo no es válida
        if (cotizacion.FechaAceptacion.HasValue && cotizacion.FechaRechazo.HasValue)
        {
            errors.Add("Una cotización no puede estar aceptada y rechazada simultáneamente");
        }

        return errors;
    }

    /// <summary>
    /// Obtiene el evento de historial correspondiente a un cambio de estado
    /// </summary>
    /// <param name="fromState">Estado anterior</param>
    /// <param name="toState">Estado nuevo</param>
    /// <returns>Tipo de evento para el historial</returns>
    public static string GetEventType(char fromState, char toState)
    {
        return (fromState, toState) switch
        {
            (_, 'B') when fromState == 'P' => TipoEvento.DevueltaABorrador,
            (_, 'P') => TipoEvento.EnviadaAProbacion,
            (_, 'A') => TipoEvento.Aprobada,
            (_, 'E') => TipoEvento.EnviadaCliente,
            (_, 'T') => TipoEvento.AceptadaCliente,
            (_, 'R') => TipoEvento.RechazadaCliente,
            (_, 'X') => TipoEvento.Archivada,
            _ => "CambioEstado"
        };
    }
}