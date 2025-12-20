namespace Domain.Entities.PublicQuestions;

public record PublicMultipleChoiseQuestion : IPublicQuestion
{
    public required string Title { get; init; }
    public required string? ImageUrl { get; init; }
    public required IEnumerable<PublicOption> Options { get; init; }
}
