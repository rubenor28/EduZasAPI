using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;

/// <summary>
/// Mapeador de entidad EF a dominio para profesores de clase.
/// </summary>
public class ClassProfessorMapper : IMapper<ClassProfessor, ClassProfessorDomain>
{
    /// <summary>
    /// Mapea una entidad de profesor de clase a un objeto de dominio.
    /// </summary>
    /// <param name="efEntity">Entidad de base de datos.</param>
    /// <returns>Objeto de dominio.</returns>
    public ClassProfessorDomain Map(ClassProfessor efEntity) =>
        new()
        {
            ClassId = efEntity.ClassId,
            UserId = efEntity.ProfessorId,
            IsOwner = efEntity.IsOwner ?? false,
            CreatedAt = efEntity.CreatedAt,
        };
}
