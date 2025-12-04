using Application.DTOs.Common;

namespace Application.DTOs.ClassResources;

public sealed record ClassResourceAssosiationCriteriaDTO : CriteriaDTO
{
    public required ulong ProfessorId { get; init; }
    public required Guid ResourceId { get; init; }
}
