using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Resources;

public sealed class ResourceEFReader(
    EduZasDotnetContext ctx,
    IEFProjector<Resource, ResourceDomain> projector
) : EFReader<Guid, ResourceDomain, Resource>(ctx, projector)
{
    protected override Expression<Func<Resource, bool>> GetIdPredicate(Guid id) =>
        r => r.ResourceId == id;
}
