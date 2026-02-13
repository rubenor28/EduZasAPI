using Application.DTOs.Tests;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tests;

/// <summary>
/// Implementación de consulta de resumen de exámenes usando EF.
/// </summary>
public sealed class TestSummaryEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Test, TestSummary, TestCriteriaDTO> projector,
    int maxPageSize
) : EFQuerier<TestSummary, TestCriteriaDTO, Test>(ctx, projector, maxPageSize)
{
    /// <inheritdoc/>
    public override IQueryable<Test> BuildQuery(TestCriteriaDTO criteria) =>
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
            )
            .OrderByDescending(t => t.CreatedAt);
}
