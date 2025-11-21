using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassTests;

public sealed class ClassTestEFReader(
    EduZasDotnetContext ctx,
    IMapper<TestPerClass, ClassTestDomain> domainMapper
) : EFReader<ClassTestIdDTO, ClassTestDomain, TestPerClass>(ctx, domainMapper)
{
    public override async Task<TestPerClass?> GetTrackedById(ClassTestIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(ct => ct.ClassId == id.ClassId)
            .Where(ct => ct.TestId == id.TestId)
            .FirstOrDefaultAsync();
}
