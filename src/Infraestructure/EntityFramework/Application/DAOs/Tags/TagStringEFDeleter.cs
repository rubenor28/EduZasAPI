using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tags;

/// <summary>
/// Implementación de eliminación de etiquetas usando EF.
/// </summary>
public sealed class TagStringEFDeleter(EduZasDotnetContext ctx, IMapper<Tag, TagDomain> domainMapper)
    : EFDeleter<string, TagDomain, Tag>(ctx, domainMapper)
{
    /// <inheritdoc/>
    public override Task<Tag?> GetTrackedById(string id) =>
        _dbSet.AsTracking().AsQueryable().Where(t => t.Text == id).FirstOrDefaultAsync();
}
