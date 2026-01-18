namespace Domain.Entities.Questions;

public interface IQuestion
{
    public string Title { get; }
    public string? ImageUrl { get; }
    public bool RequiresManualGrade { get; }
}
