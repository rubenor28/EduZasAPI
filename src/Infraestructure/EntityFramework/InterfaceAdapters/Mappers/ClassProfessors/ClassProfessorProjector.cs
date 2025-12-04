using System.Linq.Expressions;
using Application.DTOs.ClassProfessors;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;

public class ClassProfessorProjector
    : IEFProjector<ClassProfessor, ClassProfessorDomain, ClassProfessorCriteriaDTO>
{
    public Expression<Func<ClassProfessor, ClassProfessorDomain>> GetProjection(
        ClassProfessorCriteriaDTO _
    ) =>
        efEntity =>
            new()
            {
                ClassId = efEntity.ClassId,
                UserId = efEntity.ProfessorId,
                IsOwner = efEntity.IsOwner ?? false,
                CreatedAt = efEntity.CreatedAt,
            };
}
