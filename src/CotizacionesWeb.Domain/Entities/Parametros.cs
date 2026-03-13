using CotizacionesWeb.Domain.Common;

namespace CotizacionesWeb.Domain.Entities;

public class Parametros : BaseEntity
{
    public string Descripcion { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public char Tipo { get; set; }
}
