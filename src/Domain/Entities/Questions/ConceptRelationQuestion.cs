namespace Domain.Entities.Questions;

public record ConceptPair(string ConceptA, string ConceptB);

public record ConceptRelationQuestion : IQuestion
{
    public required string Title { get; init; }
    public required string? ImageUrl { get; init; }
    public required ISet<ConceptPair> Concepts { get; init; }
}
