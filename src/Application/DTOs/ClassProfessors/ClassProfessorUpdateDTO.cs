using Application.DTOs.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.DTOs.ClassProfessors;

public sealed record ClassProfessorUpdateDTO : IIdentifiable<UserClassRelationId>
{
    public required UserClassRelationId Id { get; set; }
    public required bool IsOwner { get; set; }
    public required Executor Executor { get; set; }
}
