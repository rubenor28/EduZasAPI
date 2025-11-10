using Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Application.DTOs.Resources;

public sealed record ResourceCriteriaMAPI : CriteriaDTO
{
    public required StringQueryMAPI? Title { get; set; }
    public required bool? Active { get; set; }
    public required ulong? ProfessorId { get; set; }
    public required string? ClassId { get; set; }
}
