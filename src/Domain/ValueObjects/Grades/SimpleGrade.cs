namespace Domain.ValueObjects.Grades;

public record SimpleGrade
{
    public required uint Points { get; init; }
    public required uint TotalPoints { get; init; }
}
