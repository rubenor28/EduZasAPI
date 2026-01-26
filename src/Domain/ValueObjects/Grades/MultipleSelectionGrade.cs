namespace Domain.ValueObjects.Grades;

public record MultipleSelectionGrade : Grade
{
    public override uint TotalPoints => (uint)Options.Count;
    public required IDictionary<Guid, string> Options { get; init; }
    public required ISet<Guid> CorrectOptions { get; init; }
    public required ISet<Guid> AnsweredOptions { get; init; }

    public override uint Asserts() => AnsweredOptions.SetEquals(CorrectOptions) ? TotalPoints : 0;
}
