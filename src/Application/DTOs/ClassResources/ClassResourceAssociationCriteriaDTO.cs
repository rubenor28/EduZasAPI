using Application.DTOs.Common;

namespace Application.DTOs.ClassResources;

public sealed record ClassResourceAssociationCriteriaDTO : CriteriaDTO
{
    public required ulong ProfessorId { get; init; }
    public required Guid ResourceId { get; init; }
}