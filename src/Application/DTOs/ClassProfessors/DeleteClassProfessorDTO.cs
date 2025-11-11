using Application.DTOs.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.DTOs.ClassProfessors;

public sealed record DeleteClassProfessorDTO : IIdentifiable<UserClassRelationId>
{
    public required UserClassRelationId Id { get; set; }
    public required Executor Executor { get; set; }
}
