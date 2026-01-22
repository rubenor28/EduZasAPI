namespace Domain.ValueObjects.Grades;

public record OpenGrade : Grade
{
    public new bool ManualGrade
    {
        get => base.ManualGrade.GetValueOrDefault();
        init => base.ManualGrade = value;
    }

    public override uint TotalPoints => 1;
    public override uint CalculateAsserts => ManualGrade ? 1u : 0;
}
