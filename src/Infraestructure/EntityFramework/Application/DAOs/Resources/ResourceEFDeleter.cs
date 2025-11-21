using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Resources;

public sealed class ResourceEFDeleter(
    EduZasDotnetContext ctx,
    IMapper<Resource, ResourceDomain> domainMapper
) : EFDeleter<ulong, ResourceDomain, Resource>(ctx, domainMapper)
{
    public override async Task<Resource?> GetTrackedById(ulong id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(r => r.ResourceId == id)
            .FirstOrDefaultAsync();
}
