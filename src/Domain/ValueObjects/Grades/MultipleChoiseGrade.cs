namespace Domain.ValueObjects.Grades;

public record MultipleChoiseGrade : Grade
{
    public override uint TotalPoints => 1;
    public required string Title { get; init; }
    public required IDictionary<Guid, string> Options;
    public required Guid CorrectOption { get; init; }
    public required Guid? SelectedOption { get; init; }

    public override uint Asserts() => CorrectOption == SelectedOption ? 1u : 0u;
}
