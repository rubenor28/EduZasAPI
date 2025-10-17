using Application.DTOs.ClassStudents;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public class StudentClassEFMapper
    : IMapper<ClassStudent, StudentClassRelationDTO>,
        IMapper<StudentClassRelationDTO, ClassStudent>,
        IUpdateMapper<StudentClassRelationDTO, ClassStudent>
{
    public ClassStudent Map(StudentClassRelationDTO r) =>
        new()
        {
            ClassId = r.Id.ClassId,
            StudentId = r.Id.UserId,
            Hidden = false,
        };

    public StudentClassRelationDTO Map(ClassStudent efEntity) =>
        new()
        {
            Id = new() { ClassId = efEntity.ClassId, UserId = efEntity.StudentId },
            Hidden = efEntity.Hidden,
        };

    public void Map(StudentClassRelationDTO uProps, ClassStudent entity)
    {
        entity.ClassId = uProps.Id.ClassId;
        entity.StudentId = uProps.Id.UserId;
        entity.Hidden = uProps.Hidden;
    }
}
