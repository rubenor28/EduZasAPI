namespace Application.DTOs.ClassResources;

public sealed record ClassResourceAssosiationDTO
{
    public required Guid ResourceId { get; init; }
    public required string ClassId { get; init; }
    public required string ClassName { get; init; }
    public bool IsAssosiated { get; init; }
}
