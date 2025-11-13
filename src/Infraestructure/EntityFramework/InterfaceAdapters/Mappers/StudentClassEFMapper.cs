using Application.DTOs.ClassStudents;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public class ClassStudentEFMapper
    : IMapper<ClassStudent, ClassStudentDomain>,
        IMapper<NewClassStudentDTO, ClassStudent>,
        IUpdateMapper<ClassStudentUpdateDTO, ClassStudent>
{
    public ClassStudentDomain Map(ClassStudent efEntity) =>
        new()
        {
            Id = new() { ClassId = efEntity.ClassId, UserId = efEntity.StudentId },
            Hidden = efEntity.Hidden,
            CreatedAt = efEntity.CreatedAt,
        };

    public void Map(ClassStudentUpdateDTO uProps, ClassStudent entity)
    {
        entity.ClassId = uProps.Id.ClassId;
        entity.StudentId = uProps.Id.UserId;
        entity.Hidden = uProps.Hidden;
    }

    public ClassStudent Map(NewClassStudentDTO input) =>
        new()
        {
            ClassId = input.ClassId,
            StudentId = input.UserId,
            Hidden = false,
        };
}
