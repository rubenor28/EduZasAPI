namespace Domain.Entities.QuestionAnswers;

public record OpenQuestionAnswer : IQuestionAnswer
{
    public required string Text { get; init; }
}
