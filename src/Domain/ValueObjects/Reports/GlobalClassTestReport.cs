using Domain.ValueObjects.Grades;

public record GlobalClassTestReport
{
    public required string ClassName { get; init; }
    public required string ProfessorName { get; init; }
    public required double PassThreshold { get; init; }
    public required DateTimeOffset TestDate { get; init; }

    public required double AveragePercentage { get; init; }
    public required double MedianPercentage { get; init; }
    public required double PassRate { get; init; }
    public required double StandardDeviation { get; init; }
    public required uint MaxPoints { get; init; }
    public required uint MinPoints { get; init; }
    public required int TotalStudents { get; init; }

    public required IEnumerable<IndividualGradeError> Errors { get; init; }
}
