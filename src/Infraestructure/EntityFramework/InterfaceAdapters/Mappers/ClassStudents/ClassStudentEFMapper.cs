using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassStudents;

/// <summary>
/// Mapeador de entidad EF a dominio para estudiantes de clase.
/// </summary>
public class ClassStudentMapper : IMapper<ClassStudent, ClassStudentDomain>
{
    /// <inheritdoc/>
    public ClassStudentDomain Map(ClassStudent efEntity) =>
        new()
        {
            Id = new() { ClassId = efEntity.ClassId, UserId = efEntity.StudentId },
            Hidden = efEntity.Hidden,
            CreatedAt = efEntity.CreatedAt,
        };
}
