namespace Domain.Entities.Questions;

/// <summary>
/// Representa una pregunta de selección múltiple.
/// </summary>
public record MultipleSelectionQuestion : IQuestion
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
    /// IDs de las opciones correctas.
    /// </summary>
    public required ISet<Guid> CorrectOptions { get; set; }
    /// <summary>
    /// Indica si la pregunta requiere calificación manual.
    /// </summary>
    public bool RequiresManualGrade => false;
}
