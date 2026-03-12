using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassStudents;

/// <summary>
/// Mapeador de creación para estudiantes de clase.
/// </summary>
public class NewClassStudentEFMapper : IMapper<UserClassRelationId, ClassStudent>
{
    /// <summary>
    /// Mapea un DTO de nueva relación estudiante-clase a una entidad de base de datos.
    /// </summary>
    /// <param name="id">DTO de identificación de la relación.</param>
    /// <returns>Entidad de base de datos.</returns>
    public ClassStudent Map(UserClassRelationId id) =>
        new()
        {
            ClassId = id.ClassId,
            StudentId = id.UserId,
            Hidden = false,
        };
}
