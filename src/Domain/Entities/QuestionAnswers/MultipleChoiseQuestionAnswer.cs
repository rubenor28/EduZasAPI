namespace Domain.Entities.QuestionAnswers;

public record MultipleChoiseQuestionAnswer : IQuestionAnswer
{
    public required Guid QuestionId { get; init; }
    public required Guid SelectedOption { get; init; }
}
