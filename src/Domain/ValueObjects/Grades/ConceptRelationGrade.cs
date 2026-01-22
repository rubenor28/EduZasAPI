using Domain.Entities.Questions;

namespace Domain.ValueObjects.Grades;

public record ConceptRelationGrade : Grade
{
    public required string Title { get; init; }
    public required IList<ConceptPair> Pairs { get; init; }
    public required IList<ConceptPair> AnsweredPairs { get; init; }

    public override uint CalculateAsserts
    {
        get
        {
            if (ManualGrade == true)
                return (uint)Pairs.Count;

            if (Pairs.Count != AnsweredPairs.Count)
                throw new InvalidOperationException("Los tamaños de las listas no coinciden");

            var points = 0u;
            for (var i = 0; i < Pairs.Count; i++)
            {
                if (Pairs[i] == AnsweredPairs[i])
                    break;

                points += 1;
            }

            return points;
        }
    }

    public override uint TotalPoints => (uint)Pairs.Count;
}
