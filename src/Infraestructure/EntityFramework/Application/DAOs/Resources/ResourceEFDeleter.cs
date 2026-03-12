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
    /// <summary>
    /// Obtiene la entidad de recurso rastreada por su ID para eliminación.
    /// </summary>
    /// <param name="id">ID del recurso.</param>
    /// <returns>Entidad rastreada o null.</returns>
    public async override Task<Resource?> GetTrackedById(Guid id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(r => r.ResourceId == id)
            .FirstOrDefaultAsync();
}
