namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de contacto en agenda.
/// </summary>
public partial class AgendaContact
{
    
    /// <summary>
    /// Alias del contacto.
    /// </summary>
    public string Alias { get; set; } = null!;
    
    /// <summary>
    /// Notas sobre el contacto.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// ID del propietario de la agenda.
    /// </summary>
    public ulong AgendaOwnerId { get; set; }
    
    /// <summary>
    /// ID del usuario de contacto.
    /// </summary>
    public ulong UserId { get; set; }
    
    /// <summary>
    /// Fecha de creación del contacto.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// Fecha de modificación del contacto.
    /// </summary>
    public DateTimeOffset ModifiedAt { get; set; }

    /// <summary>
    /// Propietario de la agenda.
    /// </summary>
    public virtual User AgendaOwner { get; set; } = null!;
    
    /// <summary>
    /// Usuario de contacto.
    /// </summary>
    public virtual User Contact { get; set; } = null!;
    
    /// <summary>
    /// Etiquetas del contacto.
    /// </summary>
    public virtual ICollection<ContactTag> ContactTags { get; set; } = new List<ContactTag>();
}
