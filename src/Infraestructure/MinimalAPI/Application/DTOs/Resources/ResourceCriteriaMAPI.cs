using Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Application.DTOs.Resources;

public sealed record ResourceCriteriaMAPI : CriteriaDTO
{
    public StringQueryMAPI? Title { get; set; }
    public bool? Active { get; set; }
    public ulong? ProfessorId { get; set; }
    public string? ClassId { get; set; }
}
