namespace CotizacionesWeb.Domain.Enums;

public enum TipoArchivo
{
    Manual = 'M',     // Archivado manual por el usuario
    Rechazada = 'R',  // Archivado porque la cotización fue rechazada
    Concretada = 'T'  // Archivado porque se concretó en el ERP (Transaction/Trade)
}
