using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassProfessors;

/// <summary>
/// Implementación de eliminación de relaciones Clase-Profesor usando EF.
/// </summary>
public class ClassProfessorsEFDeleter(
    EduZasDotnetContext ctx,
    IMapper<ClassProfessor, ClassProfessorDomain> domainMapper
) : EFDeleter<UserClassRelationId, ClassProfessorDomain, ClassProfessor>(ctx, domainMapper)
{
    /// <inheritdoc/>
    public override async Task<ClassProfessor?> GetTrackedById(UserClassRelationId id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(cp => cp.ProfessorId == id.UserId)
            .Where(cp => cp.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
