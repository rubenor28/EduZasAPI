namespace Domain.Entities.QuestionAnswers;

public record OrderingQuestionAnswer : IQuestionAnswer
{
    public required List<string> Sequence { get; init; }
}
