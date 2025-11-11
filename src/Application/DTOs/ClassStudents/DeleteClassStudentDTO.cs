using Application.DTOs.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.DTOs.ClassStudents;

public sealed record DeleteClassStudentDTO : IIdentifiable<UserClassRelationId>
{
    public required UserClassRelationId Id { get; set; }
    public required Executor Executor { get; set; }
}
