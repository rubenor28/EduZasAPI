using Domain.Entities.QuestionAnswers;

public record AnswerStudentDTO
{
    public required ulong UserId { get; set; }
    public required Guid TestId { get; set; }
    public required string ClassId { get; set; }
    public required IDictionary<Guid, IQuestionAnswer> Content { get; set; }
}
