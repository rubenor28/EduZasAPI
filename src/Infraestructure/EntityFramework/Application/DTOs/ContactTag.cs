namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de relación Contacto-Etiqueta.
/// </summary>
public partial class ContactTag
{
    public ulong TagId { get; set; }
    public ulong AgendaOwnerId { get; set; }
    public ulong UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual AgendaContact AgendaContact { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}
