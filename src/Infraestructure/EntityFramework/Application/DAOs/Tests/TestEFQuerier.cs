using Application.DTOs.Tests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tests;

public sealed class TestEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Test, TestDomain, TestCriteriaDTO> projector,
    int pageSize
) : EFQuerier<TestDomain, TestCriteriaDTO, Test>(ctx, projector, pageSize)
{
    public override IQueryable<Test> BuildQuery(TestCriteriaDTO criteria) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereStringQuery(criteria.Title, c => c.Title)
            .WhereOptional(criteria.TimeLimitMinutes, time => test => test.TimeLimitMinutes == time)
            .WhereOptional(criteria.ProfessorId, id => test => test.ProfessorId == id);
}
