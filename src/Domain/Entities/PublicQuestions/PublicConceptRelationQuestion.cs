namespace Domain.Entities.PublicQuestions;

public record PublicConceptRelationQuestion : IPublicQuestion
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string? ImageUrl { get; init; }
    public required IEnumerable<string> ColumnA { get; init; }
    public required IEnumerable<string> ColumnB { get; init; }
}
