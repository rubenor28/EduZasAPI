using Domain.Entities;
using Domain.Entities.QuestionAnswers;

/// <summary>
/// DTO para la actualización de una respuesta, permitiendo modificar su estado y contenido.
/// </summary>
public record AnswerUpdateDTO
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
    /// Obtiene o establece un valor opcional para indicar si el intento de respuesta ha finalizado.
    /// </summary>
    public bool? TryFinished { get; set; }
    /// <summary>
    /// Obtiene o establece un valor opcional para indicar si la respuesta ha sido calificada.
    /// </summary>
    public bool? Graded { get; set; }
    /// <summary>
    /// Obtiene o establece el contenido actualizado de la respuesta, con las respuestas a cada pregunta.
    /// </summary>
    public IDictionary<Guid, IQuestionAnswer>? Content { get; set; }
    /// <summary>
    /// Obtiene o establece los metadatos actualizados de la respuesta.
    /// </summary>
    public AnswerMetadata? Metadata { get; set; }
}
