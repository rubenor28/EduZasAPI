using Application.DTOs.ClassTests;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassTests;

/// <summary>
/// Implementación de consulta de asociaciones Clase-Examen usando EF.
/// </summary>
public sealed class ClassTestAssociationEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Class, ClassTestAssociationDTO, ClassTestAssociationCriteriaDTO> projector,
    int maxPageSize
) : EFQuerier<ClassTestAssociationDTO, ClassTestAssociationCriteriaDTO, Class>(ctx, projector, maxPageSize)
{
    /// <inheritdoc/>
    public override IQueryable<Class> BuildQuery(ClassTestAssociationCriteriaDTO criteria) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .Where(c => c.ClassProfessors.Any(cp => cp.ProfessorId == criteria.ProfessorId));
}
