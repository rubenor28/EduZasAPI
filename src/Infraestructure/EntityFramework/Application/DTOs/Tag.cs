namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de etiqueta.
/// </summary>
public partial class Tag
{
    public ulong TagId { get; set; }
    public string Text { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }

    public virtual ICollection<ContactTag> ContactTags { get; set; } = new List<ContactTag>();
}
