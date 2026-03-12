namespace Domain.Entities.PublicQuestions;

/// <summary>
/// Representa una pregunta de ordenamiento en su versión pública.
/// </summary>
public record PublicOrderingQuestion : IPublicQuestion
{
    /// <summary>
    /// Identificador único de la pregunta.
    /// </summary>
    public required Guid Id { get; init; }
    /// <summary>
    /// Título o enunciado de la pregunta.
    /// </summary>
    public required string Title { get; init; }
    /// <summary>
    /// URL de la imagen asociada (opcional).
    /// </summary>
    public required string? ImageUrl { get; init; }
    /// <summary>
    /// Obtiene los elementos a ordenar.
    /// </summary>
    public required IEnumerable<string> Items { get; init; }
}
