namespace Domain.ValueObjects.Grades;

public abstract record GradeError;

public static class GradeErrors
{
    public static GradeError MissingManualGrade(IEnumerable<Guid> questionsId) =>
        new MissingManualGrade(questionsId);
}

public sealed record MissingManualGrade(IEnumerable<Guid> QuestionId) : GradeError;
