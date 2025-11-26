using Application.DTOs.ClassStudents;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassStudents;

public class NewClassStudentEFMapper : IMapper<NewClassStudentDTO, ClassStudent>
{
    public ClassStudent Map(NewClassStudentDTO input) =>
        new()
        {
            ClassId = input.ClassId,
            StudentId = input.UserId,
            Hidden = false,
        };
}
