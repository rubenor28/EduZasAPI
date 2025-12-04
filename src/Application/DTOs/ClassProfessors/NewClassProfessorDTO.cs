namespace Application.DTOs.ClassProfessors;

public sealed record NewClassProfessorDTO
{
    public required ulong UserId { get; init; }
    public required string ClassId { get; init; }
    public required bool IsOwner { get; init; }
}
