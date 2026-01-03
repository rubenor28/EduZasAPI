namespace Domain.Entities.QuestionAnswers;

public record MultipleSelectionQuestionAnswer : IQuestionAnswer
{
    public required ISet<Guid> SelectedOptions { get; init; }
}
