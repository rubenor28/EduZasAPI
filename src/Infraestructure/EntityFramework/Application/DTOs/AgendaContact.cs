namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de contacto en agenda.
/// </summary>
public partial class AgendaContact
{
    
    public string Alias { get; set; } = null!;
    public string? Notes { get; set; }
    public ulong AgendaOwnerId { get; set; }
    public ulong UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ModifiedAt { get; set; }

    public virtual User AgendaOwner { get; set; } = null!;
    public virtual User Contact { get; set; } = null!;
    public virtual ICollection<ContactTag> ContactTags { get; set; } = new List<ContactTag>();
}
