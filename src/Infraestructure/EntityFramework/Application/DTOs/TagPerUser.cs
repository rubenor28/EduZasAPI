namespace EntityFramework.Application.DTOs;

public partial class TagsPerUser
{
    public ulong TagId { get; set; }
    public ulong AgendaContactId { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual AgendaContact Contact { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}
