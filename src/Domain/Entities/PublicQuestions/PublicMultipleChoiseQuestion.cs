namespace Domain.Entities.PublicQuestions;

/// <summary>
/// Representa una pregunta de opción múltiple en su versión pública.
/// </summary>
public record PublicMultipleChoiseQuestion : IPublicQuestion
{
    /// <inheritdoc />
    public required Guid Id { get; init; }
    /// <inheritdoc />
    public required string Title { get; init; }
    /// <inheritdoc />
    public required string? ImageUrl { get; init; }
    /// <summary>
    /// Obtiene las opciones disponibles para la pregunta.
    /// </summary>
    public required IEnumerable<PublicOption> Options { get; init; }
}
