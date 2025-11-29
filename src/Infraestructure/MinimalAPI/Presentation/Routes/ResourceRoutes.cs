using Application.DTOs.ClassResources;
using Application.DTOs.Common;
using Application.DTOs.Resources;
using Application.UseCases.ClassResource;
using Application.UseCases.Resources;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Application.DTOs.ClassResources;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Resources;
using MinimalAPI.Presentation.Filters;

namespace MinimalAPI.Presentation.Routes;

public static class ResourceRoutes
{
    public static RouteGroupBuilder MapResourceRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/resources").WithTags("Recursos académicos");

        group
            .MapPost("/", AddResource)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PublicResourceMAPI>(StatusCodes.Status201Created)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Crear un nuevo recurso académico.";
                op.Description = "Añade un nuevo recurso.";
                op.Responses["201"].Description = "Recurso creado exitosamente.";
                op.Responses["400"].Description =
                    "El ID del profesor no es válido o no se encontró.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para crear recursos para el profesor especificado.";
                return op;
            });

        group
            .MapGet("/{resourceId:guid}", GetResources)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PublicResourceMAPI>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Obtener un recurso por ID.";
                op.Description = "Recupera la información de un recurso académico específico.";
                op.Responses["200"].Description = "Recurso obtenido exitosamente.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["404"].Description =
                    "No se encontró un recurso con el ID proporcionado.";
                return op;
            });

        group
            .MapPost("/search", SearchResource)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PaginatedQuery<PublicResourceMAPI, ResourceCriteriaMAPI>>(
                StatusCodes.Status200OK
            )
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi(op =>
            {
                op.Summary = "Buscar recursos académicos.";
                op.Description = "Realiza una búsqueda paginada de recursos con filtros.";
                op.Responses["200"].Description = "Búsqueda completada exitosamente.";
                op.Responses["400"].Description = "Los criterios de búsqueda son inválidos.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                return op;
            });

        group
            .MapDelete("/{resourceId:guid}", DeleteResource)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Eliminar un recurso académico.";
                op.Description = "Elimina un recurso específico por su ID.";
                op.Responses["204"].Description = "Recurso eliminado exitosamente.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para eliminar este recurso.";
                op.Responses["404"].Description =
                    "No se encontró un recurso con el ID proporcionado.";
                return op;
            });

        group
            .MapPut("/", UpdateResource)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PublicResourceMAPI>(StatusCodes.Status200OK)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Actualizar un recurso académico.";
                op.Description = "Modifica los datos de un recurso existente.";
                op.Responses["200"].Description = "Recurso actualizado exitosamente.";
                op.Responses["400"].Description = "Los datos para la actualización son inválidos.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para modificar este recurso.";
                op.Responses["404"].Description =
                    "No se encontró un recurso con el ID proporcionado.";
                return op;
            });

        group
            .MapPost("/association", AddClassResource)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Asignar un recurso académico a una clase.";
                op.Description = "Crea una asociación entre un recurso y una clase.";
                op.Responses["204"].Description = "Recurso asignado exitosamente.";
                op.Responses["400"].Description = "Los datos de la asociación son inválidos.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para realizar esta acción.";

                return op;
            });

        group
            .MapDelete("/association", DeleteClassResource)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Desasociar un recurso de una clase.";
                op.Description = "Elimina la asociación entre un recurso y una clase.";
                op.Responses["204"].Description = "Recurso desasociado exitosamente.";
                op.Responses["400"].Description = "Los datos de la asociación son inválidos.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para realizar esta acción.";

                return op;
            });

        group
            .MapPost("/assigned", GetAssignedResources)
            .RequireAuthorization("ProfessorOrAdmin")
            .Produces<
                PaginatedQuery<ClassResourceAssosiationDTO, ClassResourceAssosiationCriteriaDTO>
            >(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Busqueda de recursos asociados a las clases de un profesor";
                op.Description =
                    "Busca las clases a las cuales pertenece el profesor determinando si el recurso seleccionado se encuentra asignado a la clase.";
                op.Responses["200"].Description = "Recurso actualizado exitosamente.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para modificar este recurso.";

                return op;
            });

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
        Guid resourceId,
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
        IMapper<
            ResourceCriteriaMAPI,
            Result<ResourceCriteriaDTO, IEnumerable<FieldErrorDTO>>
        > reqMapper,
        IMapper<
            PaginatedQuery<ResourceSummary, ResourceCriteriaDTO>,
            PaginatedQuery<ResourceSummary, ResourceCriteriaMAPI>
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
        Guid resourceId,
        DeleteResourceUseCase useCase,
        IMapper<Guid, Executor, DeleteResourceDTO> reqMapper,
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

    public static Task<IResult> AddClassResource(
        [FromBody] NewClassResourceMAPI request,
        [FromServices] AddClassResourceUseCase useCase,
        [FromServices] IMapper<NewClassResourceMAPI, Executor, NewClassResourceDTO> reqMapper,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(request, utils.GetExecutorFromContext(ctx)),
            mapResponse: _ => Results.NoContent()
        );
    }

    public static Task<IResult> DeleteClassResource(
        [FromBody] ClassResourceIdDTO request,
        [FromServices] DeleteClassResourceUseCase useCase,
        [FromServices] IMapper<ClassResourceIdDTO, Executor, DeleteClassResourceDTO> reqMapper,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(request, utils.GetExecutorFromContext(ctx)),
            mapResponse: _ => Results.NoContent()
        );
    }

    public static Task<IResult> ReadResourceClass(
        ClassResourceIdDTO request,
        ReadClassResourceUseCase useCase,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => request,
            mapResponse: res => Results.Ok(res)
        );
    }

    public static Task<IResult> GetAssignedResources(
        [FromBody] ClassResourceAssosiationCriteriaDTO request,
        [FromServices] ClassResourceAssosiationQueryUseCase useCase,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => request,
            mapResponse: (results) => Results.Ok(results)
        );
    }
}
