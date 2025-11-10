using Application.DTOs.Classes;
using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public class StudentClassEFMapper
    : IMapper<ClassStudent, StudentClassRelationDTO>,
        IMapper<EnrollClassDTO, ClassStudent>,
        IMapper<string, ulong, Executor, ToggleClassVisibilityDTO>,
        IUpdateMapper<StudentClassRelationDTO, ClassStudent>
{
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

    public ClassStudent Map(EnrollClassDTO input) =>
        new()
        {
            ClassId = input.ClassId,
            StudentId = input.UserId,
            Hidden = false,
        };

    public ToggleClassVisibilityDTO Map(string classId, ulong studentId, Executor ex)
=>
                new ()
                {
                    ClassId = classId,
                    UserId = studentId,
                    Executor = ex,
                };
}
