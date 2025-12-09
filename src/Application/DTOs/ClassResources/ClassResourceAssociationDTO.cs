namespace Application.DTOs.ClassResources;

public sealed record ClassResourceAssociationDTO
{
    public required string ClassId { get; init; }
    public required string ClassName { get; init; }
    public required bool IsAssociated { get; init; }
    public required bool IsHidden { get; init; }
}