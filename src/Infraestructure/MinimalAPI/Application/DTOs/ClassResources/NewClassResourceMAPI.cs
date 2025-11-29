namespace MinimalAPI.Application.DTOs.ClassResources;

public sealed record NewClassResourceMAPI
{
    public required string ClassId { get; init; }
    public required Guid ResourceId { get; init; }
    public required bool Hidden { get; init; }
}
