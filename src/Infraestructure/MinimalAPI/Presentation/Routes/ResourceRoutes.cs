using Application.DTOs.Common;
using Application.DTOs.Resources;
using Application.UseCases.Resources;
using Domain.Entities;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Resources;

namespace MinimalAPI.Presentation.Routes;

public static class ResourceRoutes
{
    public static RouteGroupBuilder MapResourceRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/").WithTags("Recursos académicos");

        group.MapPost("/resource", AddResource).RequireAuthorization("ProfessorOrAdmin");

        group
            .MapGet("/resources/{resourceId:ulong}", GetResources)
            .RequireAuthorization("ProfessorOrAdmin");

        group.MapPost("/resources", SearchResource).RequireAuthorization("ProfessorOrAdmin");

        group
            .MapDelete("/resources/{resourceId:ulong}", DeleteResource)
            .RequireAuthorization("ProfessorOrAdmin");

        group.MapGet("/resources", UpdateResource).RequireAuthorization("ProfessorOrAdmin");

        return group;
    }

    public static Task<IResult> AddResource(
        NewResourceMAPI request,
        AddResourceUseCase useCase,
        IMapper<NewResourceMAPI, Executor, NewResourceDTO> reqMapper,
        IMapper<ResourceDomain, PublicResourceMAPI> resMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            () => reqMapper.Map(request, utils.GetExecutorFromContext(ctx)),
            (resource) => Results.Created($"/resource/{resource.Id}", resMapper.Map(resource))
        );
    }

    public static Task<IResult> GetResources(
        ulong resourceId,
        ReadResourceUseCase useCase,
        IMapper<ResourceDomain, PublicResourceMAPI> resMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            () => resourceId,
            (resource) => Results.Ok(resMapper.Map(resource))
        );
    }

    public static Task<IResult> SearchResource(
        ResourceCriteriaMAPI request,
        ResourceQueryUseCase useCase,
        IMapper<ResourceCriteriaMAPI, ResourceCriteriaDTO> reqMapper,
        IMapper<
            PaginatedQuery<ResourceDomain, ResourceCriteriaDTO>,
            PaginatedQuery<PublicResourceMAPI, ResourceCriteriaMAPI>
        > resMapper,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(request),
            mapResponse: (search) => Results.Ok(resMapper.Map(search))
        );
    }

    public static Task<IResult> DeleteResource(
        ulong resourceId,
        DeleteResourceUseCase useCase,
        IMapper<ulong, Executor, DeleteResourceDTO> reqMapper,
        IMapper<ResourceDomain, PublicResourceMAPI> resMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            () => reqMapper.Map(resourceId, utils.GetExecutorFromContext(ctx)),
            (resource) => Results.Ok(resMapper.Map(resource))
        );
    }

    public static Task<IResult> UpdateResource(
        ResourceUpdateMAPI request,
        UpdateResourceUseCase useCase,
        IMapper<ResourceUpdateMAPI, Executor, ResourceUpdateDTO> reqMapper,
        IMapper<ResourceDomain, PublicResourceMAPI> resMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(request, utils.GetExecutorFromContext(ctx)),
            mapResponse: (resource) => Results.Ok(resMapper.Map(resource))
        );
    }
}
