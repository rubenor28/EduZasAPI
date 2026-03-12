using Application.DTOs.ClassStudents;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassStudents;

/// <summary>
/// Mapeador de actualización para estudiantes de clase.
/// </summary>
public class UpdateClassStudentEFMapper : IUpdateMapper<ClassStudentUpdateDTO, ClassStudent>
{
    /// <summary>
    /// Actualiza una entidad de relación estudiante-clase con los datos del DTO.
    /// </summary>
    /// <param name="uProps">DTO de actualización.</param>
    /// <param name="entity">Entidad de base de datos.</param>
    public void Map(ClassStudentUpdateDTO uProps, ClassStudent entity)
    {
        entity.ClassId = uProps.ClassId;
        entity.StudentId = uProps.UserId;
        entity.Hidden = uProps.Hidden;
    }
}
