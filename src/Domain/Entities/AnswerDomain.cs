using Domain.Entities.QuestionAnswers;

namespace Domain.Entities;

public record AnswerMetadata
{
    public ISet<Guid> ManualMarkAsCorrect { get; init; } = new HashSet<Guid>();
}

public class AnswerDomain
{
    public required ulong UserId { get; set; }
    public required Guid TestId { get; set; }
    public required string ClassId { get; set; }
    public required IDictionary<Guid, IQuestionAnswer> Content { get; set; }
    public required AnswerMetadata Metadata { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset ModifiedAt { get; set; }
}
