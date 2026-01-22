namespace Domain.ValueObjects.Grades;

public record MultipleSelectionGrade : Grade
{
    public override uint TotalPoints => (uint)Options.Count;
    public required string Title { get; init; }
    public required IDictionary<Guid, string> Options;
    public required ISet<Guid> CorrectOptions;
    public required ISet<Guid> AnsweredOptions;

    public override uint Asserts() => (uint)Options.Count(opt => CorrectOptions.Contains(opt.Key));
}
