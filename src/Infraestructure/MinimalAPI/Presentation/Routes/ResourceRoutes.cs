using Application.DTOs.ClassResources;
using Application.DTOs.Common;
using Application.DTOs.Resources;
using Application.UseCases.ClassResource;
using Application.UseCases.Resources;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Presentation.Filters;

namespace MinimalAPI.Presentation.Routes;

/// <summary>
/// Define las rutas relacionadas con los recursos académicos.
/// </summary>
public static class ResourceRoutes
{
    /// <summary>
    /// Mapea los endpoints para la gestión de recursos académicos.
    /// </summary>
    /// <param name="app">La aplicación web.</param>
    /// <returns>El grupo de rutas configurado.</returns>
    public static RouteGroupBuilder MapResourceRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/resources").WithTags("Recursos académicos");

        group
            .MapPost("/", AddResource)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<ResourceDomain>(StatusCodes.Status201Created)
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
            .Produces<ResourceDomain>(StatusCodes.Status200OK)
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
            .Produces<PaginatedQuery<ResourceDomain, ResourceCriteriaDTO>>(StatusCodes.Status200OK)
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
            .Produces<ResourceDomain>(StatusCodes.Status200OK)
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
            .MapDelete("/association/{resourceId:guid}/{classId}", DeleteClassResource)
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
            .MapPut("/association", UpdateClassResource)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Actualizar la relacion un recurso de una clase.";
                op.Description = "Actualiza la asociación entre un recurso y una clase.";
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
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<
                PaginatedQuery<ClassResourceAssociationDTO, ClassResourceAssociationCriteriaDTO>
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

        group
            .MapGet("/{resourceId}/{classId}", PublicResourceRead)
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<ResourceDomain>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Obtener un recurso por ID de clase y recurso.";
                op.Description = "Recupera la información de un recurso académico dada la clase.";
                op.Responses["200"].Description = "Recurso obtenido exitosamente.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para leer el recurso";
                op.Responses["404"].Description =
                    "No se encontró un recurso con el ID proporcionado.";
                return op;
            });

        return group;
    }

    public static Task<IResult> AddResource(
        [FromBody] NewResourceDTO request,
        [FromServices] AddResourceUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            () => request,
            (resource) => Results.Created($"/resource/{resource.Id}", resource)
        );
    }

    public static Task<IResult> GetResources(
        [FromRoute] Guid resourceId,
        [FromServices] ReadResourceUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            () => resourceId,
            (resource) => Results.Ok(resource)
        );
    }

    public static Task<IResult> SearchResource(
        [FromBody] ResourceCriteriaDTO request,
        [FromServices] ResourceQueryUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: search => Results.Ok(search)
        );
    }

    public static Task<IResult> DeleteResource(
        [FromRoute] Guid resourceId,
        [FromServices] DeleteResourceUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            () => resourceId,
            (resource) => Results.Ok(resource)
        );
    }

    public static Task<IResult> UpdateResource(
        [FromBody] ResourceUpdateDTO request,
        [FromServices] UpdateResourceUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: (resource) => Results.Ok(resource)
        );
    }

    public static Task<IResult> AddClassResource(
        [FromBody] ClassResourceDTO request,
        [FromServices] AddClassResourceUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: _ => Results.NoContent()
        );
    }

    public static Task<IResult> UpdateClassResource(
        [FromBody] ClassResourceDTO request,
        [FromServices] UpdateClassResourceUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: _ => Results.NoContent()
        );
    }

    public static Task<IResult> DeleteClassResource(
        [FromRoute] Guid resourceId,
        [FromRoute] string classId,
        [FromServices] DeleteClassResourceUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => new ClassResourceIdDTO { ClassId = classId, ResourceId = resourceId },
            mapResponse: _ => Results.NoContent()
        );
    }

    public static Task<IResult> ReadResourceClass(
        [FromBody] ClassResourceIdDTO request,
        [FromServices] ReadClassResourceUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: res => Results.Ok(res)
        );
    }

    public static Task<IResult> GetAssignedResources(
        [FromBody] ClassResourceAssociationCriteriaDTO request,
        [FromServices] ClassResourceAssociationQueryUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: (results) => Results.Ok(results)
        );
    }

    public static Task<IResult> PublicResourceRead(
        [FromRoute] Guid resourceId,
        [FromRoute] string classId,
        [FromServices] PublicReadResourceUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () =>
                new ReadResourceDTO(
                    UserId: utils.GetExecutorFromContext(ctx).Id,
                    ClassId: classId,
                    ResourceId: resourceId
                ),
            mapResponse: t => Results.Ok(t)
        );
}
