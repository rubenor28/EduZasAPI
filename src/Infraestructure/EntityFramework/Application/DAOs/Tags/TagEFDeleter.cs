using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tags;

/// <summary>
/// Implementación de eliminación de etiquetas usando EF.
/// </summary>
public sealed class TagEFDeleter(EduZasDotnetContext ctx, IMapper<Tag, TagDomain> domainMapper)
    : EFDeleter<ulong, TagDomain, Tag>(ctx, domainMapper)
{
    /// <summary>
    /// Obtiene la entidad de etiqueta rastreada por su ID para eliminación.
    /// </summary>
    /// <param name="id">ID de la etiqueta.</param>
    /// <returns>Entidad rastreada o null.</returns>
    public override Task<Tag?> GetTrackedById(ulong id) =>
        _dbSet.AsTracking().AsQueryable().Where(t => t.TagId == id).FirstOrDefaultAsync();
}
