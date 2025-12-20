namespace Domain.Entities.Questions;

public record OrderingQuestion : IQuestion
{
    public required string Title { get; set; }
    public required string? ImageUrl { get; set; }
    public required List<string> Sequence { get; set; }
}
