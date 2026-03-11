namespace Domain.ValueObjects.Grades;

/// <summary>
/// Representa la calificación de una pregunta de ordenar elementos.
/// </summary>
public record OrderingGrade : Grade
{
    /// <summary>
    /// Puntos totales, basados en el número de elementos a ordenar.
    /// </summary>
    public override uint TotalPoints => (uint)Sequence.Count;
    /// <summary>
    /// Secuencia correcta de elementos.
    /// </summary>
    public required List<string> Sequence { get; set; }
    /// <summary>
    /// Secuencia respondida por el estudiante.
    /// </summary>
    public required List<string> AnsweredSequence { get; set; }

    /// <summary>
    /// Calcula el número de aciertos en la secuencia.
    /// </summary>
    /// <returns>El número de elementos correctamente ordenados desde el inicio.</returns>
    public override uint Asserts()
    {
        if (Sequence.Count != AnsweredSequence.Count)
            throw new InvalidOperationException("Ambas listas deberían tener el mismo tamaño");

        var points = 0u;
        for (var i = 0; i < Sequence.Count; i++)
        {
            if (Sequence[i] != AnsweredSequence[i])
                break;

            points++;
        }

        return points;
    }
}
