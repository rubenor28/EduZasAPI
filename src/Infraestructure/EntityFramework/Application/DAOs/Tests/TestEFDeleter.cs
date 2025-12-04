using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tests;

public sealed class TestEFDeleter(EduZasDotnetContext ctx, IMapper<Test, TestDomain> domainMapper)
    : EFDeleter<Guid, TestDomain, Test>(ctx, domainMapper)
{
    public override Task<Test?> GetTrackedById(Guid id) =>
        _dbSet.AsTracking().AsQueryable().Where(t => t.TestId == id).FirstOrDefaultAsync();
}
