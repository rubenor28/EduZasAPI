namespace Application.DTOs.ClassTests;

public sealed record ClassTestDTO
{
    public required Guid TestId { get; init; }
    public required string ClassId { get; init; }
    public required bool Visible { get; init; }
}
