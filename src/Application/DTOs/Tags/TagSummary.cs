namespace Application.DTOs.Tags;

public sealed record TagSummary
{
    public required string Text { get; set; }
    public required DateTime CreatedAt { get; set; }
}
