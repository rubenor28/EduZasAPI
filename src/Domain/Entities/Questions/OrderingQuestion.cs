namespace Domain.Entities.Questions;

/// <summary>
/// Representa una pregunta de ordenamiento de elementos.
/// </summary>
public record OrderingQuestion : IQuestion
{
    /// <inheritdoc />
    public required string Title { get; set; }
    /// <inheritdoc />
    public string? ImageUrl { get; set; }
    /// <summary>
    /// Obtiene la lista de elementos en la secuencia correcta.
    /// </summary>
    public required List<string> Sequence { get; set; }
    /// <inheritdoc />
    public bool RequiresManualGrade => false;
}
