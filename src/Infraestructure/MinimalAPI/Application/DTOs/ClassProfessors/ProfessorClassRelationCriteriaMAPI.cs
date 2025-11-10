namespace MinimalAPI.Application.DTOs.ClassProfessors;

public sealed record ProfessorClassRelationCriteriaMAPI
{
    public ulong? ProfessorId { get; set; }
    public string? ClassId { get; set; }
    public bool? IsOwner { get; set; }
}
