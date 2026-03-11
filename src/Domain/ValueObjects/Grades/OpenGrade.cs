namespace Domain.ValueObjects.Grades;

/// <summary>
/// Representa la calificación de una pregunta abierta, que puede requerir calificación manual.
/// </summary>
public record OpenGrade : Grade
{
    /// <summary>
    /// Texto de la respuesta del estudiante.
    /// </summary>
    public required string? Text { get; init; }
    /// <summary>
    /// Puntos totales, siempre es 1 para este tipo de pregunta.
    /// </summary>
    public override uint TotalPoints => 1;

    /// <summary>
    /// Determina los aciertos. En las preguntas abiertas, depende de la calificación manual.
    /// </summary>
    /// <returns>1 si la calificación manual es verdadera, 0 en caso contrario.</returns>
    public override uint Asserts() => ManualGrade == true ? 1u : 0;
}
