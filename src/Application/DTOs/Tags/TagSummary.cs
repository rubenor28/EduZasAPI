namespace Application.DTOs.Tags;

public sealed record TagSummary
{
    public required string Text { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}
