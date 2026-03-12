using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassStudents;

/// <summary>
/// Implementación de eliminación de relaciones Clase-Estudiante usando EF.
/// </summary>
public class ClassStudentsEFDeleter(
    EduZasDotnetContext ctx,
    IMapper<ClassStudent, ClassStudentDomain> domainMapper
) : EFDeleter<UserClassRelationId, ClassStudentDomain, ClassStudent>(ctx, domainMapper)
{
    /// <summary>
    /// Obtiene la entidad de relación estudiante-clase rastreada por su ID compuesto para eliminación.
    /// </summary>
    /// <param name="id">ID compuesto de la relación.</param>
    /// <returns>Entidad rastreada o null.</returns>
    public async override Task<ClassStudent?> GetTrackedById(UserClassRelationId id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(cs => cs.StudentId == id.UserId)
            .Where(cs => cs.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
