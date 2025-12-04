namespace Application.DTOs.ClassProfessors;

public sealed record ClassProfessorUpdateDTO
{
    public required ulong UserId { get; init; }
    public required string ClassId { get; init; }
    public required bool IsOwner { get; init; }
}
