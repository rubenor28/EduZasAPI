using Application.DTOs.Classes;
using Domain.ValueObjects;

namespace Application.DTOs.ClassStudents;

public sealed record StudentClassRelationDTO : IIdentifiable<ClassUserRelationIdDTO>
{
    public required ClassUserRelationIdDTO Id { get; set; }
    public required bool Hidden { get; set; }
}
