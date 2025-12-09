using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassProfessors;

/// <summary>
/// Implementación de lectura de relaciones Clase-Profesor por ID usando EF.
/// </summary>
public class ClassProfessorsEFReader(
    EduZasDotnetContext ctx,
    IMapper<ClassProfessor, ClassProfessorDomain> mapper
) : EFReader<UserClassRelationId, ClassProfessorDomain, ClassProfessor>(ctx, mapper)
{
    /// <inheritdoc/>
    protected override Expression<Func<ClassProfessor, bool>> GetIdPredicate(
        UserClassRelationId id
    ) => cs => cs.ProfessorId == id.UserId && cs.ClassId == id.ClassId;
}
