namespace CotizacionesWeb.Application.Common.Email;

/// <summary>
/// Servicio para envío de emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envía un email simple
    /// </summary>
    /// <param name="destinatario">Email del destinatario</param>
    /// <param name="asunto">Asunto del email</param>
    /// <param name="cuerpo">Cuerpo del email (puede ser HTML)</param>
    /// <param name="esHtml">Indica si el cuerpo es HTML</param>
    /// <returns>True si se envió correctamente</returns>
    Task<bool> EnviarEmailAsync(string destinatario, string asunto, string cuerpo, bool esHtml = true);
    
    /// <summary>
    /// Verifica si el servicio de email está configurado y disponible
    /// </summary>
    /// <returns>True si está disponible</returns>
    Task<bool> EstaDisponibleAsync();
}