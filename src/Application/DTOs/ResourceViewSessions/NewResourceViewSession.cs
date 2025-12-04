namespace Application.DTOs.ResourceViewSessions;

public sealed record ResourceViewSession
{
    public required ulong UserId { get; init; }
    public required Guid ResourceId { get; init; }
    public required string ClassId { get; init; }
    public required DateTime StartTimeUtc { get; init; }
}
