namespace MinimalAPI.Application.DTOs.Classes;

public sealed record ProfessorClassRelationMAPI
{
    public required ulong ProfessorId { get; set; }
    public required string ClassId { get; set; }
    public required bool IsOwner { get; set; }
}
