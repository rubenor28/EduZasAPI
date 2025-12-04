namespace Domain.Entities;

public sealed record ResourceViewSession
{
    public required Guid Id { get; set; }
    public required ulong UserId { get; set; }
    public required Guid ResourceId { get; set; }
    public required string ClassId { get; set; }
    public required DateTime StartTimeUtc { get; set; }
    public required DateTime? EndTimeUtc { get; set; }
    public required int? DurationSeconds { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime ModifiedAt { get; set; }
}
