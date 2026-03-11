namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de relación Contacto-Etiqueta.
/// </summary>
public partial class ContactTag
{
    /// <summary>
    /// ID de la etiqueta.
    /// </summary>
    public ulong TagId { get; set; }
    /// <summary>
    /// ID del propietario de la agenda.
    /// </summary>
    public ulong AgendaOwnerId { get; set; }
    /// <summary>
    /// ID del usuario de contacto.
    /// </summary>
    public ulong UserId { get; set; }
    /// <summary>
    /// Fecha de asignación de la etiqueta al contacto.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Contacto de la agenda.
    /// </summary>
    public virtual AgendaContact AgendaContact { get; set; } = null!;
    /// <summary>
    /// Etiqueta asociada al contacto.
    /// </summary>
    public virtual Tag Tag { get; set; } = null!;
}
