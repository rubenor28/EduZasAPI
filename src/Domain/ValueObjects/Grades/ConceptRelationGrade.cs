using Domain.Entities.Questions;

namespace Domain.ValueObjects.Grades;

public record ConceptRelationGrade : Grade
{
    public override uint TotalPoints => (uint)Pairs.Count;
    public required string Title { get; init; }
    public required IList<ConceptPair> Pairs { get; init; }
    public required IList<ConceptPair> AnsweredPairs { get; init; }

    public override uint Asserts()
    {
        if (ManualGrade == true)
            return (uint)Pairs.Count;

        return (uint)AnsweredPairs.Intersect(Pairs).Count();
    }
}
