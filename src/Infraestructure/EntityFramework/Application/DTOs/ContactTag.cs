namespace EntityFramework.Application.DTOs;

public partial class ContactTag
{
    public string TagText { get; set; } = null!;
    public ulong AgendaOwnerId { get; set; }
    public ulong UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual AgendaContact AgendaContact { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}
