using Domain.Entities.Questions;

namespace Domain.Entities.QuestionAnswers;

public record ConceptRelationQuestionAnswer : IQuestionAnswer
{
    public required Guid QuestionId { get; init; }
    public required ISet<ConceptPair> AnsweredPairs { get; init; }
}
