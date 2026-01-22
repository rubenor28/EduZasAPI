namespace Domain.ValueObjects.Grades;

public record OpenGrade : Grade
{
    public override uint TotalPoints => 1;
    public new bool ManualGrade
    {
        get => base.ManualGrade.GetValueOrDefault();
        init => base.ManualGrade = value;
    }

    public override uint Asserts() => ManualGrade ? 1u : 0;
}
