namespace Application.Configuration;

public record GradeSettings
{
    public double PassThresholdPercentage { get; init; }
    public double PassThreshold => PassThresholdPercentage * 100;
}
