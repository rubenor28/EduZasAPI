namespace Domain.Entities.PublicQuestions;

public record PublicOption
{
    public required Guid Id { get; set; }
    public required string Text { get; set; }
}
