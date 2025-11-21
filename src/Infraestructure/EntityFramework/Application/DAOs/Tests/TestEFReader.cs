using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tests;

public sealed class TestEFReader(EduZasDotnetContext ctx, IMapper<Test, TestDomain> domainMapper)
    : EFReader<ulong, TestDomain, Test>(ctx, domainMapper)
{
    public override Task<Test?> GetTrackedById(ulong id) =>
        _dbSet.AsTracking().AsQueryable().Where(t => t.TestId == id).FirstOrDefaultAsync();
}
