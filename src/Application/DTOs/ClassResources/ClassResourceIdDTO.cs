namespace Application.DTOs.ClassResources;

public sealed record ClassResourceIdDTO
{
    public required string ClassId { get; init; }
    public required Guid ResourceId { get; init; }
}
