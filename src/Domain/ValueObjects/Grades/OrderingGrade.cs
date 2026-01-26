namespace Domain.ValueObjects.Grades;

public record OrderingGrade : Grade
{
    public override uint TotalPoints => (uint)Sequence.Count;
    public required List<string> Sequence { get; set; }
    public required List<string> AnsweredSequence { get; set; }

    public override uint Asserts()
    {
        if (Sequence.Count != AnsweredSequence.Count)
            throw new InvalidOperationException("Ambas listas deberían tener el mismo tamaño");

        var points = 0u;
        for (var i = 0; i < Sequence.Count; i++)
        {
            if (Sequence[i] != AnsweredSequence[i])
                break;

            points++;
        }

        return points;
    }
}
