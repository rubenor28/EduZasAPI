using System.Diagnostics.CodeAnalysis;

namespace Domain.ValueObjects.Grades;

public record SimpleGrade
{
    public required string StudentName { get; init; }
    public required uint Points { get; init; }
    public required uint TotalPoints { get; init; }

    [SetsRequiredMembers]
    public SimpleGrade(AnswerGrade grade, string studentName)
    {
        StudentName = studentName;
        Points = grade.Points;
        TotalPoints = grade.TotalPoints;
    }
}
