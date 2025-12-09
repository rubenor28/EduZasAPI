using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Resources;

/// <summary>
/// Implementación de eliminación de recursos usando EF.
/// </summary>
public sealed class ResourceEFDeleter(
    EduZasDotnetContext ctx,
    IMapper<Resource, ResourceDomain> domainMapper
) : EFDeleter<Guid, ResourceDomain, Resource>(ctx, domainMapper)
{
    /// <inheritdoc/>
    public override async Task<Resource?> GetTrackedById(Guid id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(r => r.ResourceId == id)
            .FirstOrDefaultAsync();
}
