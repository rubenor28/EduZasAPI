using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Resources;

public class ResourceProjector : IEFProjector<Resource, ResourceDomain>
{
    public Expression<Func<Resource, ResourceDomain>> Projection =>
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

    private static readonly Lazy<Func<Resource, ResourceDomain>> _mapFunc = new(() =>
        new ResourceProjector().Projection.Compile()
    );

    public ResourceDomain Map(Resource input) => _mapFunc.Value(input);
}

public class ResourceSummaryProjector : IEFProjector<Resource, ResourceSummary>
{
    public Expression<Func<Resource, ResourceSummary>> Projection =>
        input =>
            new()
            {
                Id = input.ResourceId,
                Active = input.Active ?? false,
                ProfessorId = input.ProfessorId,
                Title = input.Title,
            };

    private static readonly Lazy<Func<Resource, ResourceSummary>> _mapFunc = new(() =>
        new ResourceSummaryProjector().Projection.Compile()
    );

    public ResourceSummary Map(Resource input) => _mapFunc.Value(input);
}
