namespace Domain.Entities.PublicQuestions;

public record PublicOpenQuestion : IPublicQuestion
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string? ImageUrl { get; init; }
}
