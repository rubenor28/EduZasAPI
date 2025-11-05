using Application.DTOs.Tests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tests;

public sealed class TestEFQuerier(
    EduZasDotnetContext ctx,
    IMapper<Test, TestDomain> domainMapper,
    int pageSize
) : EFQuerier<TestDomain, TestCriteriaDTO, Test>(ctx, domainMapper, pageSize)
{
    public override IQueryable<Test> BuildQuery(TestCriteriaDTO criteria) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereStringQuery(criteria.Title, c => c.Title)
            .WhereStringQuery(criteria.Content, c => c.Content)
            .WhereOptional(criteria.TimeLimitMinutes, time => test => test.TimeLimitMinutes == time)
            .WhereOptional(criteria.ProfessorId, id => test => test.ProfessorId == id);
}
