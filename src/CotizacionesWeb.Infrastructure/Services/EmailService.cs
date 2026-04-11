using CotizacionesWeb.Application.Common.Email;
using CotizacionesWeb.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CotizacionesWeb.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de email usando SMTP
/// Compatible con desarrollo local y Azure App Service
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly EmailOptions _emailOptions;

    public EmailService(
        ILogger<EmailService> logger,
        IOptions<EmailOptions> emailOptions)
    {
        _logger = logger;
        _emailOptions = emailOptions.Value;
    }

    /// <summary>
    /// Envía un email usando SMTP
    /// </summary>
    public async Task<bool> EnviarEmailAsync(string destinatario, string asunto, string cuerpo, bool esHtml = true)
    {
        try
        {
            // Validar configuración
            if (!_emailOptions.IsValid())
            {
                if (!_emailOptions.Enabled)
                {
                    return await EnviarEmailSimuladoAsync(destinatario, asunto, cuerpo);
                }
                else
                {
                    _logger.LogError("Configuración de email inválida. Revise EmailOptions en configuración");
                    return false;
                }
            }

            // Validar destinatario
            if (string.IsNullOrWhiteSpace(destinatario) || !EsEmailValido(destinatario))
            {
                _logger.LogWarning("Dirección de email destinatario inválida: {Destinatario}", destinatario);
                return false;
            }

            // Crear el mensaje de email
            using var message = new MailMessage();
            message.From = new MailAddress(_emailOptions.FromAddress, _emailOptions.FromName);
            message.To.Add(destinatario);
            message.Subject = asunto ?? "";
            message.Body = cuerpo ?? "";
            message.IsBodyHtml = esHtml;
            message.BodyEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;

            // Configurar cliente SMTP
            using var smtpClient = new SmtpClient(_emailOptions.SmtpHost, _emailOptions.SmtpPort);
            smtpClient.EnableSsl = _emailOptions.EnableSsl;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(_emailOptions.SmtpUser, _emailOptions.SmtpPassword);
            smtpClient.Timeout = _emailOptions.TimeoutMs;

            // Configuración adicional para entornos cloud
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            _logger.LogInformation("Enviando email a: {Destinatario} - Asunto: {Asunto}", destinatario, asunto);

            // Enviar email
            await smtpClient.SendMailAsync(message);

            _logger.LogInformation("? Email enviado exitosamente a {Destinatario}", destinatario);
            return true;
        }
        catch (SmtpException smtpEx)
        {
            _logger.LogError(smtpEx, "? Error SMTP al enviar email a {Destinatario}: {StatusCode} - {Message}",
                destinatario, smtpEx.StatusCode, smtpEx.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Error general al enviar email a {Destinatario}: {Message}",
                destinatario, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Verifica si el servicio está disponible
    /// </summary>
    public async Task<bool> EstaDisponibleAsync()
    {
        try
        {
            if (!_emailOptions.Enabled)
            {
                _logger.LogInformation("?? Servicio de email en modo simulación");
                return true;
            }

            if (!_emailOptions.IsValid())
            {
                _logger.LogWarning("?? Configuración de email inválida");
                return false;
            }

            // Verificar conectividad básica con el servidor SMTP
            using var tcpClient = new System.Net.Sockets.TcpClient();
            var connectTask = tcpClient.ConnectAsync(_emailOptions.SmtpHost, _emailOptions.SmtpPort);
            var timeoutTask = Task.Delay(5000); // 5 segundos timeout

            var completedTask = await Task.WhenAny(connectTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                _logger.LogWarning("? Timeout al verificar conectividad SMTP con {Host}:{Port}", 
                    _emailOptions.SmtpHost, _emailOptions.SmtpPort);
                return false;
            }

            if (connectTask.IsCompletedSuccessfully)
            {
                _logger.LogDebug("? Conectividad SMTP verificada con {Host}:{Port}", 
                    _emailOptions.SmtpHost, _emailOptions.SmtpPort);
                return true;
            }
            else
            {
                _logger.LogWarning("? No se pudo conectar al servidor SMTP {Host}:{Port}", 
                    _emailOptions.SmtpHost, _emailOptions.SmtpPort);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Error al verificar disponibilidad del servicio de email: {Message}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Envío simulado para desarrollo o cuando está deshabilitado
    /// </summary>
    private async Task<bool> EnviarEmailSimuladoAsync(string destinatario, string asunto, string cuerpo)
    {
        _logger.LogInformation("?? [SIMULADO] Enviando email a: {Destinatario}", destinatario);
        _logger.LogInformation("?? [SIMULADO] Asunto: {Asunto}", asunto);
        _logger.LogDebug("?? [SIMULADO] Cuerpo: {Cuerpo}", cuerpo.Length > 100 ? cuerpo.Substring(0, 100) + "..." : cuerpo);
        
        // Simular tiempo de envío
        await Task.Delay(200);
        
        // Simular éxito del 98% de las veces (más realista para desarrollo)
        var exito = Random.Shared.NextDouble() > 0.02;
        
        if (exito)
        {
            _logger.LogInformation("? [SIMULADO] Email enviado exitosamente a {Destinatario}", destinatario);
        }
        else
        {
            _logger.LogWarning("? [SIMULADO] Fallo simulado en envío de email a {Destinatario}", destinatario);
        }
        
        return exito;
    }

    /// <summary>
    /// Validación básica de formato de email
    /// </summary>
    private static bool EsEmailValido(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}