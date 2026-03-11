namespace Domain.Entities.Questions;

/// <summary>
/// Representa un par de conceptos que deben ser relacionados.
/// </summary>
/// <param name="ConceptA">El primer concepto.</param>
/// <param name="ConceptB">El segundo concepto.</param>
public record ConceptPair(string ConceptA, string ConceptB);

/// <summary>
/// Representa una pregunta de examen donde el usuario debe relacionar conceptos.
/// </summary>
public record ConceptRelationQuestion : IQuestion
{
    /// <inheritdoc />
    public required string Title { get; init; }
    /// <inheritdoc />
    public string? ImageUrl { get; init; }
    /// <summary>
    /// Obtiene el conjunto de pares de conceptos correctos.
    /// </summary>
    public required ISet<ConceptPair> Concepts { get; init; }
    /// <inheritdoc />
    public bool RequiresManualGrade => false;
}
