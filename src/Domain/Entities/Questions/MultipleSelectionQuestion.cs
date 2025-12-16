namespace Domain.Entities.Questions;

public record MultipleSelectionQuestion : IQuestion
{
    public required string Title { get; set; }
    public required string? ImageUrl { get; set; }
    public required IDictionary<Guid, string> Options { get; set; }
    public required IEnumerable<Guid> CorrectOptions { get; set; }
}
