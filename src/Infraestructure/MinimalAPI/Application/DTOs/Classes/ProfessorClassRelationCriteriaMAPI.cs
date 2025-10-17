namespace MinimalAPI.Application.DTOs.Classes;

public sealed record ProfessorClassRelationCriteriaMAPI
{
    public ulong? ProfessorId { get; set; }
    public string? ClassId { get; set; }
    public bool? IsOwner { get; set; }
}
