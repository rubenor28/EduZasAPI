using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;

public class ClassProfessorMapper : IMapper<ClassProfessor, ClassProfessorDomain>
{
    public ClassProfessorDomain Map(ClassProfessor efEntity) =>
        new()
        {
            ClassId = efEntity.ClassId,
            UserId = efEntity.ProfessorId,
            IsOwner = efEntity.IsOwner ?? false,
            CreatedAt = efEntity.CreatedAt,
        };
}
