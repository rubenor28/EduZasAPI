using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tests;

public sealed class TestEFDeleter(EduZasDotnetContext ctx, IMapper<Test, TestDomain> domainMapper)
    : EFDeleter<ulong, TestDomain, Test>(ctx, domainMapper)
{
    public override Task<Test?> GetTrackedById(ulong id) =>
        _dbSet.AsTracking().AsQueryable().Where(t => t.TestId == id).FirstOrDefaultAsync();
}
