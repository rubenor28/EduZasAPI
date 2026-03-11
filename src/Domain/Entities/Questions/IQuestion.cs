namespace Domain.Entities.Questions;

/// <summary>
/// Define la estructura base para una pregunta de examen, incluyendo su contenido y metadatos.
/// </summary>
public interface IQuestion
{
    /// <summary>
    /// Obtiene el título o enunciado de la pregunta.
    /// </summary>
    public string Title { get; }
    /// <summary>
    /// Obtiene la URL de una imagen asociada a la pregunta (opcional).
    /// </summary>
    public string? ImageUrl { get; }
    /// <summary>
    /// Obtiene un valor que indica si la pregunta requiere calificación manual.
    /// </summary>
    public bool RequiresManualGrade { get; }
}
