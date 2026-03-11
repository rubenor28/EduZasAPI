namespace Domain.Entities.PublicQuestions;

/// <summary>
/// Representa una pregunta de ordenamiento en su versión pública.
/// </summary>
public record PublicOrderingQuestion : IPublicQuestion
{
    /// <inheritdoc />
    public required Guid Id { get; init; }
    /// <inheritdoc />
    public required string Title { get; init; }
    /// <inheritdoc />
    public required string? ImageUrl { get; init; }
    /// <summary>
    /// Obtiene los elementos a ordenar.
    /// </summary>
    public required IEnumerable<string> Items { get; init; }
}
