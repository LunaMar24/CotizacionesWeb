namespace CotizacionesWeb.Domain.Entities;

public class Parametros
{
    public int ParametroId { get; set; }  // FASE 3: Llave primaria específica (sin BaseEntity según modelo)
    public string Descripcion { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public char Tipo { get; set; }
}
