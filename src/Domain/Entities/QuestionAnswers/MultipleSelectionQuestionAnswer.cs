namespace Domain.Entities.QuestionAnswers;

/// <summary>
/// Representa la respuesta de un usuario a una pregunta de selección múltiple.
/// </summary>
public record MultipleSelectionQuestionAnswer : IQuestionAnswer
{
    /// <summary>
    /// Obtiene los IDs de las opciones que el usuario ha seleccionado.
    /// </summary>
    public required ISet<Guid> SelectedOptions { get; init; }
}
