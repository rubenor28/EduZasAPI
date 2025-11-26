using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;

public class ClassProfessorProjector : IEFProjector<ClassProfessor, ClassProfessorDomain>
{
    public Expression<Func<ClassProfessor, ClassProfessorDomain>> Projection =>
        efEntity =>
            new()
            {
                Id = new() { ClassId = efEntity.ClassId, UserId = efEntity.ProfessorId },
                IsOwner = efEntity.IsOwner ?? false,
                CreatedAt = efEntity.CreatedAt,
            };

    private static readonly Lazy<Func<ClassProfessor, ClassProfessorDomain>> _mapFunc = new(() =>
        new ClassProfessorProjector().Projection.Compile()
    );

    public ClassProfessorDomain Map(ClassProfessor efEntity) => _mapFunc.Value(efEntity);
}
