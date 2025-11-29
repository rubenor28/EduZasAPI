using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record ResourceViewSession
{
    public required Guid Id { get; set; }
    public required ulong UserId { get; set; }
    public required Guid ResourceId { get; set; }
    public required string ClassId { get; set; }
    public required DateTime StartTimeUtc { get; set; }
    public required Optional<DateTime> EndTimeUtc { get; set; }
    public required Optional<int> DurationSeconds { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime ModifiedAt { get; set; }
}
