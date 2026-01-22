using Domain.ValueObjects.Grades;

public record TestGrade
{
    public required uint Points { get; init; }
    public required uint TotalPoints { get; init; }
    public required IEnumerable<Grade> GradeDetails { get; init; }
}
