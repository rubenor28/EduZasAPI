namespace Domain.Entities.Questions;

/// <summary>
/// Representa una pregunta de opción múltiple.
/// </summary>
public record MultipleChoiseQuestion : IQuestion
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
    /// Opciones de respuesta (ID y texto).
    /// </summary>
    public required IDictionary<Guid, string> Options { get; set; }
    /// <summary>
    /// ID de la opción correcta.
    /// </summary>
    public required Guid CorrectOption { get; set; }
    /// <summary>
    /// Indica si la pregunta requiere calificación manual.
    /// </summary>
    public bool RequiresManualGrade => false;
}
