using Domain.Entities.QuestionAnswers;

public record AnswerUpdateStudentDTO
{
    public required ulong UserId { get; set; }
    public required Guid TestId { get; set; }
    public required string ClassId { get; set; }
    public required IDictionary<Guid, IQuestionAnswer> Content { get; set; }
}
