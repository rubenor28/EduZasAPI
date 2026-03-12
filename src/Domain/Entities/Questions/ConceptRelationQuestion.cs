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
    /// <summary>
    /// Título o enunciado de la pregunta.
    /// </summary>
    public required string Title { get; init; }
    /// <summary>
    /// URL de la imagen asociada (opcional).
    /// </summary>
    public string? ImageUrl { get; init; }
    /// <summary>
    /// Conjunto de pares de conceptos correctos.
    /// </summary>
    public required ISet<ConceptPair> Concepts { get; init; }
    /// <summary>
    /// Indica si la pregunta requiere calificación manual.
    /// </summary>
    public bool RequiresManualGrade => false;
}
