using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

public class ClassTestEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<TestPerClass, ClassTestDomain> domainMapper,
    IUpdateMapper<ClassTestUpdateDTO, TestPerClass> updateMapper
) : EFUpdater<ClassTestDomain, ClassTestUpdateDTO, TestPerClass>(ctx, domainMapper, updateMapper)
{
    protected override Task<TestPerClass?> GetTrackedByDTO(ClassTestUpdateDTO value) =>
        _dbSet
            .AsTracking()
            .Where(tpc => tpc.TestId == value.TestId && tpc.ClassId == value.ClassId)
            .FirstOrDefaultAsync();
}
