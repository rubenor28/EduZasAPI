using Domain.Entities.Questions;

namespace Domain.ValueObjects.Grades;

/// <summary>
/// Representa la calificación de una pregunta de relación de conceptos.
/// </summary>
public record ConceptRelationGrade : Grade
{
    /// <summary>
    /// Puntos totales, basados en el número de pares a relacionar.
    /// </summary>
    public override uint TotalPoints => (uint)Pairs.Count;
    /// <summary>
    /// Lista de los pares correctos.
    /// </summary>
    public required IList<ConceptPair> Pairs { get; init; }
    /// <summary>
    /// Lista de los pares respondidos por el estudiante.
    /// </summary>
    public required IList<ConceptPair> AnsweredPairs { get; init; }

    /// <summary>
    /// Calcula el número de aciertos contando las intersecciones entre los pares correctos y los respondidos.
    /// </summary>
    /// <returns>El número de pares correctamente relacionados.</returns>
    public override uint Asserts() => (uint)AnsweredPairs.Intersect(Pairs).Count();
}
