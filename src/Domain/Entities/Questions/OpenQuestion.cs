namespace Domain.Entities.Questions;

/// <summary>
/// Representa una pregunta abierta que requiere una respuesta textual.
/// </summary>
public record OpenQuestion : IQuestion
{
    /// <inheritdoc />
    public required string Title { get; set; }
    /// <inheritdoc />
    public string? ImageUrl { get; set; }
    /// <inheritdoc />
    public bool RequiresManualGrade => true;
}
