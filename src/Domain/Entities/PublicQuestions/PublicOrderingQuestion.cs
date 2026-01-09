namespace Domain.Entities.PublicQuestions;

public record PublicOrderingQuestion : IPublicQuestion
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string? ImageUrl { get; init; }
    public required IEnumerable<string> Items { get; init; }
}
