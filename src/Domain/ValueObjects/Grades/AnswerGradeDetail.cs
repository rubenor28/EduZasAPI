namespace Domain.ValueObjects.Grades;

public record AnswerGradeDetail : AnswerGrade
{
    public required Guid TestId { get; init; }
    public required string ClassName { get; init; }
    public required string TestTitle { get; init; }
    public required string ProfessorName { get; init; }
    public required string StudentName { get; init; }
    public required double Score { get; init; }
    public required bool Approved { get; init; }
    public required DateTimeOffset Date { get; init; }
}

