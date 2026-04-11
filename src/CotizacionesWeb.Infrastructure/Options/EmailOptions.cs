namespace CotizacionesWeb.Infrastructure.Options;

/// <summary>
/// Configuración para el servicio de email
/// </summary>
public class EmailOptions
{
    public const string SectionName = "Email";

    /// <summary>
    /// Servidor SMTP (ej: smtp.gmail.com, smtp.office365.com)
    /// </summary>
    public string SmtpHost { get; set; } = string.Empty;

    /// <summary>
    /// Puerto SMTP (ej: 587, 465, 25)
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// Usuario para autenticación SMTP
    /// </summary>
    public string SmtpUser { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña para autenticación SMTP
    /// </summary>
    public string SmtpPassword { get; set; } = string.Empty;

    /// <summary>
    /// Dirección del remitente
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del remitente
    /// </summary>
    public string FromName { get; set; } = string.Empty;

    /// <summary>
    /// Habilitar SSL/TLS
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Timeout en milisegundos para el envío
    /// </summary>
    public int TimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Indica si el servicio está habilitado
    /// Si es false, se usará simulación
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Validar que la configuración sea válida
    /// </summary>
    public bool IsValid()
    {
        return !Enabled || (
            !string.IsNullOrWhiteSpace(SmtpHost) &&
            SmtpPort > 0 &&
            !string.IsNullOrWhiteSpace(SmtpUser) &&
            !string.IsNullOrWhiteSpace(SmtpPassword) &&
            !string.IsNullOrWhiteSpace(FromAddress)
        );
    }
}