namespace Application.DTOs.ClassTests;

public sealed record ClassTestIdDTO
{
    public required Guid TestId { get; init; }
    public required string ClassId { get; init; }
}
