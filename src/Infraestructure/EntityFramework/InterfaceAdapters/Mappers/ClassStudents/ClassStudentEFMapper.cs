using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassStudents;

public class ClassStudentMapper : IMapper<ClassStudent, ClassStudentDomain>
{
    public ClassStudentDomain Map(ClassStudent efEntity) =>
        new()
        {
            Id = new() { ClassId = efEntity.ClassId, UserId = efEntity.StudentId },
            Hidden = efEntity.Hidden,
            CreatedAt = efEntity.CreatedAt,
        };
}
