using Application.DTOs.ClassTests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassTests;

public sealed class ClassTestEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<TestPerClass, ClassTestDomain> domainMapper,
    IUpdateMapper<ClassTestUpdateDTO, TestPerClass> updateMapper
)
    : CompositeKeyEFUpdater<ClassTestIdDTO, ClassTestDomain, ClassTestUpdateDTO, TestPerClass>(
        ctx,
        domainMapper,
        updateMapper
    )
{
    protected override async Task<TestPerClass?> GetTrackedById(ClassTestIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(ct => ct.TestId == id.TestId)
            .Where(ct => ct.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
