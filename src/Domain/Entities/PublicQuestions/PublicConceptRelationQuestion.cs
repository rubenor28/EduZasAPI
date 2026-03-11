namespace Domain.Entities.PublicQuestions;

/// <summary>
/// Representa una pregunta de relación de conceptos en su versión pública.
/// </summary>
public record PublicConceptRelationQuestion : IPublicQuestion
{
    /// <inheritdoc />
    public required Guid Id { get; init; }
    /// <inheritdoc />
    public required string Title { get; init; }
    /// <inheritdoc />
    public required string? ImageUrl { get; init; }
    /// <summary>
    /// Obtiene la colección de conceptos de la primera columna.
    /// </summary>
    public required IEnumerable<string> ColumnA { get; init; }
    /// <summary>
    /// Obtiene la colección de conceptos de la segunda columna.
    /// </summary>
    public required IEnumerable<string> ColumnB { get; init; }
}
