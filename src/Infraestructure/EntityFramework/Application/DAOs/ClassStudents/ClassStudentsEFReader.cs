using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassStudents;

/// <summary>
/// Implementación de lectura de relaciones Clase-Estudiante por ID usando EF.
/// </summary>
public class ClassStudentsEFReader(
    EduZasDotnetContext ctx,
    IMapper<ClassStudent, ClassStudentDomain> mapper
) : EFReader<UserClassRelationId, ClassStudentDomain, ClassStudent>(ctx, mapper)
{
    /// <inheritdoc/>
    protected override Expression<Func<ClassStudent, bool>> GetIdPredicate(
        UserClassRelationId id
    ) => cs => cs.ClassId == id.ClassId && cs.StudentId == id.UserId;
}
