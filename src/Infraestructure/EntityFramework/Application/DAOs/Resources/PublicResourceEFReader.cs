using System.Linq.Expressions;
using Application.DTOs.Resources;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Resources;

/// <summary>
/// Implementación de lectura de recursos por ID usando EF.
/// </summary>
public sealed class PublicResourceEFReader(
    EduZasDotnetContext ctx,
    IMapper<Resource, ResourceDomain> mapper
) : EFReader<ReadResourceDTO, ResourceDomain, Resource>(ctx, mapper)
{
    /// <inheritdoc/>
    protected override Expression<Func<Resource, bool>> GetIdPredicate(ReadResourceDTO dto) =>
        r => r.ResourceId == dto.ResourceId;
}
