using Application.DTOs.ClassTests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassTests;

public sealed class ClassTestEFDeleter(
    EduZasDotnetContext ctx,
    IMapper<TestPerClass, ClassTestDomain> domainMapper
) : EFDeleter<ClassTestIdDTO, ClassTestDomain, TestPerClass>(ctx, domainMapper)
{
    public override async Task<TestPerClass?> GetTrackedById(ClassTestIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(ct => ct.TestId == id.TestId)
            .Where(ct => ct.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
