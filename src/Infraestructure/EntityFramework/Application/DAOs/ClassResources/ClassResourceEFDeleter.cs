using Application.DTOs.ClassResources;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassResources;

/// <summary>
/// Implementación de eliminación de relaciones Clase-Recurso usando EF.
/// </summary>
public sealed class ClassResourceEFDeleter(
    EduZasDotnetContext ctx,
    IMapper<ClassResource, ClassResourceDomain> domainMapper
) : EFDeleter<ClassResourceIdDTO, ClassResourceDomain, ClassResource>(ctx, domainMapper)
{
    /// <summary>
    /// Obtiene la entidad de asociación recurso-clase rastreada por su ID compuesto para eliminación.
    /// </summary>
    /// <param name="id">ID compuesto de la asociación.</param>
    /// <returns>Entidad rastreada o null.</returns>
    public async override Task<ClassResource?> GetTrackedById(ClassResourceIdDTO id) =>
        await _dbSet
            .AsTracking()
            .Where(r => r.ResourceId == id.ResourceId && r.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
