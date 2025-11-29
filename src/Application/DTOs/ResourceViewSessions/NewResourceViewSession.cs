namespace Application.DTOs.ResourceViewSessions;

public sealed record ResourceViewSession
{
    public required ulong UserId { get; set; }
    public required Guid ResourceId { get; set; }
    public required string ClassId { get; set; }
    public required DateTime StartTimeUtc { get; set; }
}
