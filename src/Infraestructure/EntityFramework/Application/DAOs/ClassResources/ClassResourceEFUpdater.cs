using Application.DTOs.ClassResources;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassResources;

public sealed class ClassResourceEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<ClassResource, ClassResourceDomain> domainMapper,
    IUpdateMapper<ClassResourceDTO, ClassResource> updateMapper
)
    : EFUpdater<ClassResourceDomain, ClassResourceDTO, ClassResource>(
        ctx,
        domainMapper,
        updateMapper
    )
{
    protected override Task<ClassResource?> GetTrackedByDTO(ClassResourceDTO value) =>
        _dbSet
            .AsTracking()
            .Where(cr => cr.ResourceId == value.ResourceId && cr.ClassId == value.ClassId)
            .FirstOrDefaultAsync();
}
