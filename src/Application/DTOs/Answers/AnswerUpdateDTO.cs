using Domain.Entities;
using Domain.Entities.QuestionAnswers;

public record AnswerUpdateDTO
{
    public required ulong UserId { get; set; }
    public required Guid TestId { get; set; }
    public required string ClassId { get; set; }
    public bool? TryFinished { get; set; }
    public bool? Graded { get; set; }
    public IDictionary<Guid, IQuestionAnswer>? Content { get; set; }
    public AnswerMetadata? Metadata { get; set; }
}
