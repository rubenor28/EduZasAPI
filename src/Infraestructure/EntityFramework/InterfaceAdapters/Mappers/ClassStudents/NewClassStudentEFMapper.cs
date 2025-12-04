using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassStudents;

public class NewClassStudentEFMapper : IMapper<UserClassRelationId, ClassStudent>
{
    public ClassStudent Map(UserClassRelationId input) =>
        new()
        {
            ClassId = input.ClassId,
            StudentId = input.UserId,
            Hidden = false,
        };
}
