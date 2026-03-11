namespace Domain.Entities.QuestionAnswers;

/// <summary>
/// Representa la respuesta de un usuario a una pregunta de opción múltiple.
/// </summary>
public record MultipleChoiseQuestionAnswer : IQuestionAnswer
{
    /// <summary>
    /// Obtiene el ID de la opción que el usuario ha seleccionado.
    /// </summary>
    public required Guid? SelectedOption { get; init; }
}
