namespace Domain.Entities.Questions;

public record OpenQuestion : IQuestion
{
    public required string Title { get; set; }
    public required string? ImageUrl { get; set; }
}
