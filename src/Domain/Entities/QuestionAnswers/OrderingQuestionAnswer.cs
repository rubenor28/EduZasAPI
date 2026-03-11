namespace Domain.Entities.QuestionAnswers;

/// <summary>
/// Representa la respuesta de un usuario a una pregunta de ordenamiento.
/// </summary>
public record OrderingQuestionAnswer : IQuestionAnswer
{
    /// <summary>
    /// Obtiene la secuencia de elementos ordenada por el usuario.
    /// </summary>
    public required List<string> Sequence { get; init; }
}
