namespace Domain.Entities.PublicQuestions;

/// <summary>
/// Define la estructura de una pregunta en su versión pública, sin revelar las respuestas correctas.
/// </summary>
public interface IPublicQuestion
{
    /// <summary>
    /// Obtiene el identificador único de la pregunta.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Obtiene el título o enunciado de la pregunta.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Obtiene la URL de una imagen asociada a la pregunta (opcional).
    /// </summary>
    string? ImageUrl { get; }
}
