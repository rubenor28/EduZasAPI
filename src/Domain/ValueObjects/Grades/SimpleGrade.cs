namespace Domain.ValueObjects.Grades;

public record SimpleGrade
{
    public required ulong StudentId { get; init; }
    public required uint Points { get; init; }
    public required uint TotalPoints { get; init; }
}
