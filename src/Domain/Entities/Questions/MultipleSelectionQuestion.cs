namespace Domain.Entities.Questions;

public record MultipleSelectionQuestion : IQuestion
{
    public required string Title { get; set; }
    public string? ImageUrl { get; set; }
    public required IDictionary<Guid, string> Options { get; set; }
    public required ISet<Guid> CorrectOptions { get; set; }
}
