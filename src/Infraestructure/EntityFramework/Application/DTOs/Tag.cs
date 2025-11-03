namespace EntityFramework.Application.DTOs;

public partial class Tag
{
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ContactTag> ContactTags { get; set; } = new List<ContactTag>();
}
