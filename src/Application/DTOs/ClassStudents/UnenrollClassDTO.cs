using Application.DTOs.Classes;
using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.ClassStudents;

public sealed record UnenrollClassDTO : IIdentifiable<ClassUserRelationIdDTO>
{
    public required ClassUserRelationIdDTO Id { get; set; }
    public required Executor Executor { get; set; }
}
