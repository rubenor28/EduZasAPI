namespace Domain.Entities.Questions;

public record ConceptPair(string ConceptA, string ConceptB);

public record ConceptRelationQuestion : IQuestion
{
    public required string Title { get; set; }
    public required string? ImageUrl { get; set; }
    public required IDictionary<Guid, ConceptPair> Concepts { get; set; }
}
