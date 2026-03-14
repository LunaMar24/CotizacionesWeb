namespace CotizacionesWeb.Domain.Enums;

public static class TipoEvento
{
    // Eventos básicos
    public const string Creada = "Creada";
    public const string VersionGenerada = "Version Generada";
    
    // Eventos de flujo de aprobación
    public const string EnviadaAProbacion = "EnviadaAProbacion";
    public const string DevueltaABorrador = "DevueltaABorrador";
    public const string Aprobada = "Aprobada";
    
    // Eventos de envío al cliente
    public const string EnviadaCliente = "EnviadaCliente";
    public const string AceptadaCliente = "AceptadaCliente";
    public const string RechazadaCliente = "RechazadaCliente";
    
    // Eventos administrativos
    public const string Archivada = "Archivada";
    public const string EnviadaERP = "EnviadaERP";
    
    // Eventos legacy (mantener por compatibilidad)
    public const string Modificada = "Modificada";
    public const string Enviada = "Enviada";
    public const string Rechazada = "Rechazada";
    public const string Reactivada = "Reactivada";
    public const string Cancelada = "Cancelada";
}
