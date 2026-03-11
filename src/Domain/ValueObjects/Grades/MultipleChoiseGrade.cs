namespace Domain.ValueObjects.Grades;

/// <summary>
/// Representa la calificación de una pregunta de opción múltiple (una sola respuesta correcta).
/// </summary>
public record MultipleChoiseGrade : Grade
{
    /// <summary>
    /// Puntos totales, siempre es 1 para este tipo de pregunta.
    /// </summary>
    public override uint TotalPoints => 1;
    /// <summary>
    /// Opciones disponibles.
    /// </summary>
    public required IDictionary<Guid, string> Options { get; init; }
    /// <summary>
    /// Opción correcta.
    /// </summary>
    public required Guid CorrectOption { get; init; }
    /// <summary>
    /// Opción seleccionada por el estudiante.
    /// </summary>
    public required Guid? SelectedOption { get; init; }

    /// <summary>
    /// Verifica si la opción seleccionada es la correcta.
    /// </summary>
    /// <returns>1 si la respuesta es correcta, 0 en caso contrario.</returns>
    public override uint Asserts() => CorrectOption == SelectedOption ? 1u : 0u;
}
