namespace Domain.ValueObjects.Grades;

/// <summary>
/// Representa la calificación de una pregunta de selección múltiple.
/// </summary>
public record MultipleSelectionGrade : Grade
{
    /// <summary>
    /// Puntos totales, basados en el número de opciones.
    /// </summary>
    public override uint TotalPoints => (uint)Options.Count;
    /// <summary>
    /// Opciones disponibles en la pregunta.
    /// </summary>
    public required IDictionary<Guid, string> Options { get; init; }
    /// <summary>
    /// Conjunto de opciones correctas.
    /// </summary>
    public required ISet<Guid> CorrectOptions { get; init; }
    /// <summary>
    /// Conjunto de opciones respondidas por el estudiante.
    /// </summary>
    public required ISet<Guid> AnsweredOptions { get; init; }

    /// <summary>
    /// Calcula los aciertos comparando las opciones respondidas con las correctas.
    /// </summary>
    /// <returns>Los puntos totales si la respuesta es correcta, 0 en caso contrario.</returns>
    public override uint Asserts() => AnsweredOptions.SetEquals(CorrectOptions) ? TotalPoints : 0;
}
