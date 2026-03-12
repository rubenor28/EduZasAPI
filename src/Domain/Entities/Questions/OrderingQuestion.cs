namespace Domain.Entities.Questions;

/// <summary>
/// Representa una pregunta de ordenamiento de elementos.
/// </summary>
public record OrderingQuestion : IQuestion
{
    /// <summary>
    /// Título o enunciado de la pregunta.
    /// </summary>
    public required string Title { get; set; }
    /// <summary>
    /// URL de la imagen asociada (opcional).
    /// </summary>
    public string? ImageUrl { get; set; }
    /// <summary>
    /// Lista de elementos en la secuencia correcta.
    /// </summary>
    public required List<string> Sequence { get; set; }
    /// <summary>
    /// Indica si la pregunta requiere calificación manual.
    /// </summary>
    public bool RequiresManualGrade => false;
}
