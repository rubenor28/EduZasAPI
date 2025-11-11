namespace MinimalAPI.Application.DTOs.ClassProfessors;

public sealed record ClassProfessorMAPI
{
    public required ulong UserId { get; set; }
    public required string ClassId { get; set; }
    public required bool IsOwner { get; set; }
}
