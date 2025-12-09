using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassStudents;

/// <summary>
/// Mapeador de creación para estudiantes de clase.
/// </summary>
public class NewClassStudentEFMapper : IMapper<UserClassRelationId, ClassStudent>
{
    /// <inheritdoc/>
    public ClassStudent Map(UserClassRelationId input) =>
        new()
        {
            ClassId = input.ClassId,
            StudentId = input.UserId,
            Hidden = false,
        };
}
