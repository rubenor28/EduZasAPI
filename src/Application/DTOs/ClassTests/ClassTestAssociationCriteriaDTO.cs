using Application.DTOs.Common;

namespace Application.DTOs.ClassTests;

public sealed record ClassTestAssociationCriteriaDTO : CriteriaDTO
{
    public required ulong ProfessorId { get; init; }
    public required Guid TestId { get; init; }
}
