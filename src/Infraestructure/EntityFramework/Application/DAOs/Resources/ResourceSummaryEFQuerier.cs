using Application.DTOs.Resources;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Resources;

public class ResourceSummaryEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Resource, ResourceSummary> projector,
    int pageSize
) : EFQuerier<ResourceSummary, ResourceCriteriaDTO, Resource>(ctx, projector, pageSize)
{
    public override IQueryable<Resource> BuildQuery(ResourceCriteriaDTO criteria) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereOptional(criteria.Active, active => r => r.Active == active)
            .WhereOptional(criteria.ProfessorId, id => r => r.ProfessorId == id)
            .WhereStringQuery(criteria.Title, t => t.Title)
            .WhereOptional(
                criteria.ClassId,
                id => r => r.ClassResources.Any(rpc => rpc.ClassId == id)
            );
}
