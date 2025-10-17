namespace EntityFramework.Application.DTOs;

public partial class AgendaContact
{
    public ulong AgendaContactId { get; set; }
    public string Alias { get; set; } = null!;
    public string Notes { get; set; } = null!;
    public ulong AgendaOwnerId { get; set; }
    public ulong ContactId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public virtual User AgendaOwner { get; set; } = null!;
    public virtual User Contact { get; set; } = null!;
    public virtual ICollection<TagsPerUser> TagsPerUsers { get; set; } = new List<TagsPerUser>();
}
