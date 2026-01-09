namespace Domain.Entities.PublicQuestions;

public record PublicMultipleSelectionQuestion : IPublicQuestion
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string? ImageUrl { get; init; }
    public required IEnumerable<PublicOption> Options { get; init; }
}
