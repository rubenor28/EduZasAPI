using Application.DTOs.Common;

namespace Application.DTOs.Resources;

public sealed record ResourceCriteriaDTO : CriteriaDTO
{
    public bool? Active { get; init; } 
    public StringQueryDTO? Title { get; init; } 
    public ulong? ProfessorId { get; init; } 
    public string? ClassId { get; init; } 
}
