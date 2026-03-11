using Domain.Entities;

/// <summary>
/// DTO para la actualización de una respuesta por parte de un profesor.
/// </summary>
public record AnswerUpdateProfessorDTO
{
    /// <summary>
    /// Obtiene o establece el ID del usuario cuya respuesta se va a actualizar.
    /// </summary>
    public required ulong UserId { get; set; }
    /// <summary>
    /// Obtiene o establece el ID de la evaluación a la que pertenece la respuesta.
    /// </summary>
    public required Guid TestId { get; set; }
    /// <summary>
    /// Obtiene o establece el ID de la clase asociada a la respuesta.
    /// </summary>
    public required string ClassId { get; set; }
    /// <summary>
    /// Obtiene o establece los metadatos actualizados de la respuesta, incluyendo la calificación manual.
    /// </summary>
    public required AnswerMetadata Metadata { get; set; }
}
