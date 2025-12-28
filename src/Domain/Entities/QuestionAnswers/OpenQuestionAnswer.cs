namespace Domain.Entities.QuestionAnswers;

public record OpenQuestionAnswer : IQuestionAnswer
{
    public required Guid QuestionId { get; init; }
    public required string Text { get; init; }
}
