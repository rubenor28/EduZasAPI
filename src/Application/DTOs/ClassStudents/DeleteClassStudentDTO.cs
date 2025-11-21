using Application.DTOs.Common;
using Domain.Entities;

namespace Application.DTOs.ClassStudents;

public sealed record DeleteClassStudentDTO
{
    public required UserClassRelationId Id { get; set; }
    public required Executor Executor { get; set; }
}
