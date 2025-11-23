using Application.DTOs.Common;

namespace MinimalAPI.Application.DTOs.ClassProfessors;

public sealed record ClassProfessorCriteriaMAPI : CriteriaDTO
{
    public ulong? UserId { get; set; }
    public string? ClassId { get; set; }
    public bool? IsOwner { get; set; }
}
