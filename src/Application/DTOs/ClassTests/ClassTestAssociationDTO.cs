namespace Application.DTOs.ClassTests;

public sealed record ClassTestAssociationDTO
{
    public required string ClassId { get; init; }
    public required string ClassName { get; init; }
    public required bool IsAssociated { get; init; }
    public required bool IsVisible { get; init; }
}
