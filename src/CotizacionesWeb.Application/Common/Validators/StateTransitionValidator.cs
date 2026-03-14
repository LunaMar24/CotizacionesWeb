using CotizacionesWeb.Domain.Enums;

namespace CotizacionesWeb.Application.Common.Validators;

/// <summary>
/// Validador de transiciones de estado según lineamientos funcionales
/// </summary>
public class StateTransitionValidator
{
    private static readonly Dictionary<char, char[]> AllowedTransitions = new()
    {
        { 'B', new[] { 'P', 'X' } },           // Borrador ? PendienteAprobacion, Archivada
        { 'P', new[] { 'A', 'B' } },           // PendienteAprobacion ? Aprobada, Borrador
        { 'A', new[] { 'E' } },                // Aprobada ? Enviada
        { 'E', new[] { 'T', 'R' } },           // Enviada ? Aceptada, Rechazada
        { 'T', new[] { 'X' } },                // Aceptada ? Archivada
        { 'R', new[] { 'X' } },                // Rechazada ? Archivada
        { 'X', Array.Empty<char>() }           // Archivada ? Ninguno
    };

    /// <summary>
    /// Valida si una transición de estado es permitida
    /// </summary>
    /// <param name="fromState">Estado actual</param>
    /// <param name="toState">Estado destino</param>
    /// <returns>True si la transición es válida</returns>
    public static bool IsTransitionAllowed(char fromState, char toState)
    {
        return AllowedTransitions.ContainsKey(fromState) && 
               AllowedTransitions[fromState].Contains(toState);
    }

    /// <summary>
    /// Obtiene los estados permitidos desde un estado específico
    /// </summary>
    /// <param name="fromState">Estado actual</param>
    /// <returns>Array de estados permitidos</returns>
    public static char[] GetAllowedStates(char fromState)
    {
        return AllowedTransitions.TryGetValue(fromState, out var allowedStates) 
            ? allowedStates 
            : Array.Empty<char>();
    }

    /// <summary>
    /// Valida si se requiere nota obligatoria para la transición
    /// </summary>
    /// <param name="fromState">Estado actual</param>
    /// <param name="toState">Estado destino</param>
    /// <returns>True si requiere nota obligatoria</returns>
    public static bool RequiresNote(char fromState, char toState)
    {
        // PendienteAprobacion ? Borrador requiere nota obligatoria
        return fromState == 'P' && toState == 'B';
    }

    /// <summary>
    /// Obtiene el nombre del estado para mostrar en UI
    /// </summary>
    /// <param name="state">Código del estado</param>
    /// <returns>Nombre del estado</returns>
    public static string GetStateName(char state)
    {
        return state switch
        {
            'B' => "Borrador",
            'P' => "Pendiente Aprobación",
            'A' => "Aprobada",
            'E' => "Enviada",
            'T' => "Aceptada",
            'R' => "Rechazada",
            'X' => "Archivada",
            _ => "Desconocido"
        };
    }

    /// <summary>
    /// Valida si un estado puede ser enviado al ERP
    /// </summary>
    /// <param name="state">Estado actual</param>
    /// <returns>True si puede enviarse al ERP</returns>
    public static bool CanSendToERP(char state)
    {
        // Solo estados Aprobada, Enviada, Aceptada pueden ir al ERP
        return state is 'A' or 'E' or 'T';
    }
}