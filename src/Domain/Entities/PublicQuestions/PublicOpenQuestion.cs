namespace Domain.Entities.PublicQuestions;

/// <summary>
/// Representa una pregunta abierta en su versión pública.
/// </summary>
public record PublicOpenQuestion : IPublicQuestion
{
    /// <inheritdoc />
    public required Guid Id { get; init; }
    /// <inheritdoc />
    public required string Title { get; init; }
    /// <inheritdoc />
    public required string? ImageUrl { get; init; }
}
