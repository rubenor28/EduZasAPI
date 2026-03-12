namespace Domain.Entities.PublicQuestions;

/// <summary>
/// Representa una pregunta de opción múltiple en su versión pública.
/// </summary>
public record PublicMultipleChoiseQuestion : IPublicQuestion
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
    /// Obtiene las opciones disponibles para la pregunta.
    /// </summary>
    public required IEnumerable<PublicOption> Options { get; init; }
}
