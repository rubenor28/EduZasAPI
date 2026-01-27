using Domain.Entities.QuestionAnswers;

namespace Domain.Entities;

public record AnswerMetadata
{
    public IDictionary<Guid, bool> ManualGrade { get; init; } = new Dictionary<Guid, bool>();
}

public class AnswerDomain
{
    public required ulong UserId { get; set; }
    public required Guid TestId { get; set; }
    public required string ClassId { get; set; }
    public required bool TryFinished { get; set; }
    public required IDictionary<Guid, IQuestionAnswer> Content { get; set; }
    public required AnswerMetadata Metadata { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset ModifiedAt { get; set; }
}
