using System.Linq.Expressions;
using Application.DTOs.Resources;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Resources;

/// <summary>
/// Proyector de consultas para recursos.
/// </summary>
public class ResourceProjector : IEFProjector<Resource, ResourceDomain, ResourceCriteriaDTO>
{
    /// <inheritdoc/>
    public Expression<Func<Resource, ResourceDomain>> GetProjection(ResourceCriteriaDTO criteria) =>
        input =>
            new()
            {
                Id = input.ResourceId,
                Active = input.Active ?? true,
                Color = input.Color,
                Content = input.Content,
                ProfessorId = input.ProfessorId,
                Title = input.Title,
                CreatedAt = input.CreatedAt,
                ModifiedAt = input.ModifiedAt,
            };
}

/// <summary>
/// Proyector de consultas para resúmenes de recursos.
/// </summary>
public class ResourceSummaryProjector : IEFProjector<Resource, ResourceSummary, ResourceCriteriaDTO>
{
    /// <inheritdoc/>
    public Expression<Func<Resource, ResourceSummary>> GetProjection(
        ResourceCriteriaDTO criteria
    ) =>
        input =>
            new()
            {
                Id = input.ResourceId,
                Color = input.Color,
                Active = input.Active ?? false,
                ProfessorId = input.ProfessorId,
                Title = input.Title,
            };
}
