namespace Domain.Entities.Questions;

/// <summary>
/// Representa una pregunta de selección múltiple.
/// </summary>
public record MultipleSelectionQuestion : IQuestion
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
    /// Obtiene el conjunto de IDs de las opciones correctas.
    /// </summary>
    public required ISet<Guid> CorrectOptions { get; set; }
    /// <inheritdoc />
    public bool RequiresManualGrade => false;
}
