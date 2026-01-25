namespace Domain.ValueObjects.Grades;

public record IndividualGradeError(ulong UserId, string Error);

public record IndividualGradeErrorDetail
{
    public required ulong StudentId { get; init; }
    public required string StudentName { get; init; }
    public required string Error { get; init; }
};
