using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;

/// <summary>
/// Mapeador de entidad EF a dominio para profesores de clase.
/// </summary>
public class ClassProfessorMapper : IMapper<ClassProfessor, ClassProfessorDomain>
{
    /// <inheritdoc/>
    public ClassProfessorDomain Map(ClassProfessor efEntity) =>
        new()
        {
            ClassId = efEntity.ClassId,
            UserId = efEntity.ProfessorId,
            IsOwner = efEntity.IsOwner ?? false,
            CreatedAt = efEntity.CreatedAt,
        };
}
