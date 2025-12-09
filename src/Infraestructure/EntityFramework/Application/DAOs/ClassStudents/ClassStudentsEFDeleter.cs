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
    /// <inheritdoc/>
    public override async Task<ClassStudent?> GetTrackedById(UserClassRelationId id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(cs => cs.StudentId == id.UserId)
            .Where(cs => cs.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
