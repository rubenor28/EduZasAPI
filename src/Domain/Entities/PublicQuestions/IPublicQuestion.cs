namespace Domain.Entities.PublicQuestions;

public interface IPublicQuestion
{
    Guid Id { get; }
    string Title { get; }
    string? ImageUrl { get; }
}
