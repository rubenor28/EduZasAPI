namespace Domain.Entities.QuestionAnswers;

/// <summary>
/// Representa la respuesta de un usuario a una pregunta abierta.
/// </summary>
public record OpenQuestionAnswer : IQuestionAnswer
{
    /// <summary>
    /// Obtiene el texto de la respuesta proporcionado por el usuario.
    /// </summary>
    public required string? Text { get; init; }
}
