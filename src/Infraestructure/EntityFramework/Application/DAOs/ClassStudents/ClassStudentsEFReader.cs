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
    /// <summary>
    /// Obtiene el predicado para filtrar la relación estudiante-clase por su ID compuesto.
    /// </summary>
    /// <param name="id">ID compuesto de la relación.</param>
    /// <returns>Expresión de predicado.</returns>
    protected override Expression<Func<ClassStudent, bool>> GetIdPredicate(UserClassRelationId id) => cs => cs.ClassId == id.ClassId && cs.StudentId == id.UserId;
}
