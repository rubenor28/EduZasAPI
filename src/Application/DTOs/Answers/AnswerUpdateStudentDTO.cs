using Domain.Entities.QuestionAnswers;

/// <summary>
/// DTO para la actualización de una respuesta por parte de un estudiante.
/// </summary>
public record AnswerUpdateStudentDTO
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
    /// Obtiene o establece el contenido actualizado de la respuesta del estudiante.
    /// </summary>
    public required IDictionary<Guid, IQuestionAnswer> Content { get; set; }
}
