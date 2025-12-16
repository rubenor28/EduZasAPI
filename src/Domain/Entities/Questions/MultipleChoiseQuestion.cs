namespace Domain.Entities.Questions;

public record MultipleChoiseQuestion : IQuestion
{
    public required string Title { get; set; }
    public required string? ImageUrl { get; set; }
    public required IDictionary<Guid, string> Options { get; set; }
    public required Guid CorrectOption { get; set; }
}
