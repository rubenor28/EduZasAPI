namespace EntityFramework.Application.DTOs;

public partial class ResourceViewSession
{
    public Guid Id { get; set; }
    public ulong UserId { get; set; }
    public Guid ResourceId { get; set; }
    public string ClassId { get; set; } = null!;
    public DateTime StartTimeUtc { get; set; }
    public DateTime? EndTimeUtc { get; set; }
    public int? DurationSeconds { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Resource Resource { get; set; } = null!;
    public virtual Class Class { get; set; } = null!;
}
