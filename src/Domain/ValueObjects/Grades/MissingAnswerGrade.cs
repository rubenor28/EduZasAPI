namespace Domain.ValueObjects.Grades;

public record MissingAnswerGrade : Grade
{
    public override uint Asserts() => 0;
    public override uint TotalPoints => QuestionWeight;

    // Puntos valía la pregunta
    public required uint QuestionWeight { get; init; }
}
