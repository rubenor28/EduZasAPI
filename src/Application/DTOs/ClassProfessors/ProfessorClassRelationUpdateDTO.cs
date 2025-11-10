using Application.DTOs.Classes;
using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.ClassProfessors;

public sealed record ProfessorClassRelationUpdateDTO : IIdentifiable<ClassUserRelationIdDTO>
{
    public required ClassUserRelationIdDTO Id { get; set; }
    public required bool IsOwner { get; set; }
    public required Executor Executor { get; set; }
}
