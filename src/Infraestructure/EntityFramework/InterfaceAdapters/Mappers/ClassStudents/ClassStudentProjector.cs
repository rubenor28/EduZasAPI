using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassStudents;

public class ClassStudentProjector : IEFProjector<ClassStudent, ClassStudentDomain>
{
    public Expression<Func<ClassStudent, ClassStudentDomain>> Projection =>
        efEntity =>
            new()
            {
                Id = new() { ClassId = efEntity.ClassId, UserId = efEntity.StudentId },
                Hidden = efEntity.Hidden,
                CreatedAt = efEntity.CreatedAt,
            };

    private static readonly Lazy<Func<ClassStudent, ClassStudentDomain>> _mapFunc = new(() =>
        new ClassStudentProjector().Projection.Compile()
    );

    public ClassStudentDomain Map(ClassStudent efEntity) => _mapFunc.Value(efEntity);
}
