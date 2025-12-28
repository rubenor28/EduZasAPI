namespace Domain.Entities.QuestionAnswers;

public record MultipleSelectionQuestionAnswer : IQuestionAnswer
{
    public required Guid QuestionId { get; init; }
    public required IEnumerable<Guid> SelectedOptions { get; init; }
}
