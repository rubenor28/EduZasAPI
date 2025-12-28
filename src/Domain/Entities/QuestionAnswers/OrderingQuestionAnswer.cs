namespace Domain.Entities.QuestionAnswers;

public record OrderingQuestionAnswer : IQuestionAnswer
{
    public required Guid QuestionId { get; init; }
    public required List<string> Sequence { get; init; }
}
