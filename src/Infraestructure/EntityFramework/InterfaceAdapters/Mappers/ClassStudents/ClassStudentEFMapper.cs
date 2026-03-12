using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassStudents;

/// <summary>
/// Mapeador de entidad EF a dominio para estudiantes de clase.
/// </summary>
public class ClassStudentMapper : IMapper<ClassStudent, ClassStudentDomain>
{
    /// <summary>
    /// Mapea una entidad de estudiante de clase a un objeto de dominio.
    /// </summary>
    /// <param name="efEntity">Entidad de base de datos.</param>
    /// <returns>Objeto de dominio.</returns>
    public ClassStudentDomain Map(ClassStudent efEntity) =>
        new()
        {
             ClassId = efEntity.ClassId, UserId = efEntity.StudentId ,
            Hidden = efEntity.Hidden,
            CreatedAt = efEntity.CreatedAt,
        };
}
