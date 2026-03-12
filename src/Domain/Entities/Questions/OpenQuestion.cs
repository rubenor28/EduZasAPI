namespace Domain.Entities.Questions;

/// <summary>
/// Representa una pregunta abierta que requiere una respuesta textual.
/// </summary>
public record OpenQuestion : IQuestion
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
    /// Indica si la pregunta requiere calificación manual.
    /// </summary>
    public bool RequiresManualGrade => true;
}
