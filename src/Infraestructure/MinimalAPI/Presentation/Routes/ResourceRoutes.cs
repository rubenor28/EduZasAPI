using Application.DTOs.Common;
using Application.DTOs.Resources;
using Application.UseCases.Resources;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Resources;
using MinimalAPI.Presentation.Mappers;

namespace MinimalAPI.Presentation.Routes;

public static class ResourceRoutes
{
    public static RouteGroupBuilder MapUserRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/").WithTags("Recursos académicos");

        group.MapPost("/resource", AddResource).RequireAuthorization("ProfessorOrAdmin");
        // group.MapPost("/resources", GetResources).RequireAuthorization("ProfessorOrAdmin");

        return group;
    }

    public static Task<IResult> AddResource(
        NewResourceMAPI request,
        AddResourceUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils,
        IMapper<UseCaseError, IResult> useCaseErrorMapper
    )
    {
        return utils.HandleResponseAsync(async () =>
        {
            var executor = utils.GetExecutorFromContext(ctx);

            var result = await useCase.ExecuteAsync(
                new()
                {
                    Title = request.Title,
                    Content = request.Content,
                    ProfessorId = request.ProfessorId,
                    Executor = executor,
                }
            );

            if (result.IsErr)
                return useCaseErrorMapper.Map(result.UnwrapErr());

            var created = result.Unwrap();
            return Results.Created($"/resource/{created.Id}", created);
        });
    }
}
