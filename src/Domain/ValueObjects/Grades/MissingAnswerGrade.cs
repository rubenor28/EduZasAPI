namespace Domain.ValueObjects.Grades;

/// <summary>
/// Representa una pregunta no respondida por el estudiante.
/// </summary>
public record MissingAnswerGrade : Grade
{
    /// <summary>
    /// Siempre devuelve 0 aciertos.
    /// </summary>
    public override uint Asserts() => 0;
    /// <summary>
    /// Puntos totales de la pregunta, basados en su peso.
    /// </summary>
    public override uint TotalPoints => QuestionWeight;

    /// <summary>
    /// El peso o valor total de la pregunta que no fue respondida.
    /// </summary>
    public required uint QuestionWeight { get; init; }
}
