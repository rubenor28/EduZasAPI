using Application.DTOs.ClassStudents;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassStudents;

/// <summary>
/// Mapeador de actualización para estudiantes de clase.
/// </summary>
public class UpdateClassStudentEFMapper : IUpdateMapper<ClassStudentUpdateDTO, ClassStudent>
{
    /// <inheritdoc/>
    public void Map(ClassStudentUpdateDTO uProps, ClassStudent entity)
    {
        entity.ClassId = uProps.ClassId;
        entity.StudentId = uProps.UserId;
        entity.Hidden = uProps.Hidden;
    }
}
