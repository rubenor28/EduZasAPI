using Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Application.DTOs.Resources;

public sealed record ResourceCriteriaMAPI : CriteriaDTO
{
    public bool? Active { get; init; }
    public StringQueryMAPI? Title { get; init; }
    public ulong? ProfessorId { get; init; }
    public string? ClassId { get; init; }
}
