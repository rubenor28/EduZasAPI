namespace Domain.Entities.Questions;

/// <summary>
/// Representa una pregunta de opción múltiple.
/// </summary>
public record MultipleChoiseQuestion : IQuestion
{
    /// <inheritdoc />
    public required string Title { get; set; }
    /// <inheritdoc />
    public string? ImageUrl { get; set; }
    /// <summary>
    /// Obtiene las opciones de respuesta, donde la clave es el ID y el valor es el texto.
    /// </summary>
    public required IDictionary<Guid, string> Options { get; set; }
    /// <summary>
    /// Obtiene el ID de la opción correcta.
    /// </summary>
    public required Guid CorrectOption { get; set; }
    /// <inheritdoc />
    public bool RequiresManualGrade => false;
}
