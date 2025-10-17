namespace EntityFramework.Application.DTOs;

public partial class Tag
{
    public ulong TagId { get; set; }
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TagsPerUser> TagsPerUsers { get; set; } = new List<TagsPerUser>();
}
