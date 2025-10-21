using Application.DTOs.Common;

namespace Application.DTOs.ClassProfessors;

public sealed record class AddProfessorToClassDTO
{
    public required string ClassId { get; set; }
    public required ulong UserId { get; set; }
    public required bool IsOwner { get; set; }
    public required Executor Executor { get; set; }
}
