namespace Domain.Entities.PublicQuestions;

public interface IPublicQuestion
{
    string Title { get; }
    string? ImageUrl { get; }
}
