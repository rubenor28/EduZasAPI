namespace Domain.Entities.PublicQuestions;

/// <summary>
/// Representa una opción de respuesta en una pregunta pública.
/// </summary>
public record PublicOption
{
    /// <summary>
    /// Obtiene o establece el identificador único de la opción.
    /// </summary>
    public required Guid Id { get; set; }
    /// <summary>
    /// Obtiene o establece el texto de la opción.
    /// </summary>
    public required string Text { get; set; }
}
