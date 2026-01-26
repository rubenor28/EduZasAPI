namespace Domain.ValueObjects.Grades;

public record OpenGrade : Grade
{
    public required string? Text { get; init; }
    public override uint TotalPoints => 1;

    public override uint Asserts() => ManualGrade == true ? 1u : 0;
}
