using Domain.ValueObjects.Grades;

public record StudentResult
{
    public required ulong StudentId { get; init; }
    public required double Grade { get; init; }
}

public record ClassTestReport
{
    public required string ClassName { get; init; }
    public required string ProfessorName { get; init; }
    public required double PassThreshold { get; init; }
    public required DateTimeOffset TestDate { get; init; }

    public required double AveragePercentage { get; init; }
    public required double MedianPercentage { get; init; }
    public required double PassPercentage { get; init; }
    public required double StandardDeviation { get; init; }
    public required double MaxScore { get; init; }
    public required double MinScore { get; init; }
    public required int TotalStudents { get; init; }

    public required IEnumerable<StudentResult> Results { get; init; }
    public required IEnumerable<IndividualGradeError> Errors { get; init; }
}
