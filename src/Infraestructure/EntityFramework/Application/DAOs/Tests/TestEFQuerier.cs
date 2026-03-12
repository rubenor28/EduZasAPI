using Application.DTOs.Tests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tests;

/// <summary>
/// Implementación de consulta de exámenes usando EF.
/// </summary>
public sealed class TestEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Test, TestDomain, TestCriteriaDTO> projector,
    int pageSize
) : EFQuerier<TestDomain, TestCriteriaDTO, Test>(ctx, projector, pageSize)
{
    /// <summary>
    /// Construye la consulta de exámenes a partir de los criterios de búsqueda.
    /// </summary>
    /// <param name="c">Criterios de consulta.</param>
    /// <returns>IQueryable de exámenes.</returns>
    public override IQueryable<Test> BuildQuery(TestCriteriaDTO c) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereStringQuery(criteria.Title, c => c.Title)
            .WhereOptional(criteria.Active, active => c => c.Active == active)
            .WhereOptional(criteria.TimeLimitMinutes, time => test => test.TimeLimitMinutes == time)
            .WhereOptional(criteria.ProfessorId, id => test => test.ProfessorId == id)
            .WhereOptional(
                criteria.AssignedInClass,
                c => test => test.TestsPerClasses.Any(tc => tc.ClassId == c)
            );
}
