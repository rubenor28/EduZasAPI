using Application.DTOs.Common;

namespace Application.DTOs.ClassProfessors;

public sealed record ClassProfessorCriteriaDTO : CriteriaDTO
{
    public ulong? UserId { get; init; } 
    public string? ClassId { get; init; } 
    public bool? IsOwner { get; init; } 
}
