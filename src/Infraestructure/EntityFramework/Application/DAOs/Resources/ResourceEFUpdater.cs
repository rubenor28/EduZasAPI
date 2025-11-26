using Application.DTOs.Resources;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Resources;

public sealed class ResourceEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<Resource, ResourceDomain> domainMapper,
    IUpdateMapper<ResourceUpdateDTO, Resource> updateMapper
) : EFUpdater<ResourceDomain, ResourceUpdateDTO, Resource>(ctx, domainMapper, updateMapper)
{
    protected override Task<Resource?> GetTrackedByDTO(ResourceUpdateDTO value) =>
        _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(r => r.ResourceId == value.Id)
            .FirstOrDefaultAsync();
}
