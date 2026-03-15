namespace CotizacionesWeb.Domain.Common;

/// <summary>
/// Clase base para entidades con ID genérico y auditoría basada en IDs de usuario.
/// FASE 1: Mantener Id genérico, cambiar solo la auditoría a IDs.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public int? ModifiedBy { get; set; }
}
