using Application.DTOs.Tests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tests;

public sealed class TestEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<Test, TestDomain> domainMapper,
    IUpdateMapper<TestUpdateDTO, Test> updateMapper
) : EFUpdater<TestDomain, TestUpdateDTO, Test>(ctx, domainMapper, updateMapper)
{
    protected override Task<Test?> GetTrackedByDTO(TestUpdateDTO value) =>
        _dbSet.AsTracking().AsQueryable().Where(t => t.TestId == value.Id).FirstOrDefaultAsync();
}
