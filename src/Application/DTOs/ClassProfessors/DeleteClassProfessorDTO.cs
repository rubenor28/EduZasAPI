using Application.DTOs.Common;
using Domain.Entities;

namespace Application.DTOs.ClassProfessors;

public sealed record DeleteClassProfessorDTO
{
    public required UserClassRelationId Id { get; set; }
    public required Executor Executor { get; set; }
}
