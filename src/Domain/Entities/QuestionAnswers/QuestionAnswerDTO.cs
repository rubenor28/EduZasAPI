using Domain.Entities.QuestionAnswers;

public sealed record QuestionAnswerDTO
{
    public required Guid QuestionId { get; init; }
    public required IQuestionAnswer Data { get; init; }
}
