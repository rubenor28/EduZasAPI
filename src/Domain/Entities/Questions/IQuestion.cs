namespace Domain.Entities.Questions;

public interface IQuestion
{
    public string Title { get; set; }
    public string? ImageUrl { get; set; }
}
