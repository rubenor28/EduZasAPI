using System.Linq.Expressions;
using Application.DTOs.Resources;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Resources;

public class ResourceProjector : IEFProjector<Resource, ResourceDomain, ResourceCriteriaDTO>
{
    public Expression<Func<Resource, ResourceDomain>> GetProjection(ResourceCriteriaDTO criteria) =>
        input =>
            new()
            {
                Id = input.ResourceId,
                Active = input.Active ?? true,
                Content = input.Content,
                ProfessorId = input.ProfessorId,
                Title = input.Title,
                CreatedAt = input.CreatedAt,
                ModifiedAt = input.ModifiedAt,
            };
}

public class ResourceSummaryProjector : IEFProjector<Resource, ResourceSummary, ResourceCriteriaDTO>
{
    public Expression<Func<Resource, ResourceSummary>> GetProjection(
        ResourceCriteriaDTO criteria
    ) =>
        input =>
            new()
            {
                Id = input.ResourceId,
                Active = input.Active ?? false,
                ProfessorId = input.ProfessorId,
                Title = input.Title,
            };
}
