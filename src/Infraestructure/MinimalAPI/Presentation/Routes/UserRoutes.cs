using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.UseCases.Users;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Users;

namespace MinimalAPI.Presentation.Routes;

public static class UserRoutes
{
    public static RouteGroupBuilder MapUserRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/users").WithTags("Usuarios");

        group.MapPost("/", SearchUsers)
            .RequireAuthorization("Admin")
            .Produces<PaginatedQuery<PublicUserMAPI, UserCriteriaMAPI>>(StatusCodes.Status200OK)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Buscar usuarios por criterios.";
                op.Description = "Realiza una búsqueda paginada de usuarios con filtros. Requiere privilegios de administrador.";
                op.Responses["200"].Description = "Búsqueda completada exitosamente.";
                op.Responses["400"].Description = "Los criterios de búsqueda son inválidos.";
                op.Responses["401"].Description = "El usuario no está autenticado.";
                op.Responses["403"].Description = "El usuario no tiene permisos de administrador.";
                return op;
            });

        group.MapPut("/", UpdateUser)
            .RequireAuthorization("Admin")
            .Produces<PublicUserMAPI>(StatusCodes.Status200OK)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Actualizar un usuario.";
                op.Description = "Modifica los datos de un usuario existente. Requiere privilegios de administrador.";
                op.Responses["200"].Description = "El usuario fue actualizado exitosamente.";
                op.Responses["400"].Description = "Los datos proporcionados para la actualización son inválidos.";
                op.Responses["401"].Description = "El usuario no está autenticado.";
                op.Responses["403"].Description = "El usuario no tiene permisos de administrador.";
                op.Responses["404"].Description = "No se encontró un usuario con el ID proporcionado.";
                return op;
            });

        group.MapDelete("/{userId:ulong}", DeleteUser)
            .RequireAuthorization("Admin")
            .Produces<PublicUserMAPI>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Eliminar un usuario.";
                op.Description = "Elimina un usuario de forma lógica (soft delete). Requiere privilegios de administrador.";
                op.Responses["200"].Description = "El usuario fue marcado como inactivo exitosamente.";
                op.Responses["401"].Description = "El usuario no está autenticado.";
                op.Responses["403"].Description = "El usuario no tiene permisos de administrador.";
                op.Responses["404"].Description = "No se encontró un usuario con el ID proporcionado.";
                return op;
            });

        group.MapGet("/{email}", GetUserByEmail)
          .RequireAuthorization("ProfessorOrAdmin");

        return group;
    }

    public static Task<IResult> SearchUsers(
        UserCriteriaMAPI criteria,
        UserQueryUseCase useCase,
        IMapper<UserCriteriaMAPI, Result<UserCriteriaDTO, IEnumerable<FieldErrorDTO>>> reqMapper,
        IMapper<
            PaginatedQuery<UserDomain, UserCriteriaDTO>,
            PaginatedQuery<PublicUserMAPI, UserCriteriaMAPI>
        > resMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(criteria),
            mapResponse: (search) => Results.Ok(resMapper.Map(search))
        );
    }

    public static Task<IResult> UpdateUser(
        UserUpdateMAPI request,
        UpdateUserUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils,
        IMapper<UserUpdateMAPI, Executor, Result<UserUpdateDTO, IEnumerable<FieldErrorDTO>>> reqMapper,
        IMapper<UserDomain, PublicUserMAPI> resMapper
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(request, utils.GetExecutorFromContext(ctx)),
            mapResponse: (user) => Results.Ok(resMapper.Map(user))
        );
    }

    public static Task<IResult> DeleteUser(
        ulong userId,
        [FromServices] DeleteUserUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils,
        IMapper<ulong, Executor, DeleteUserDTO> reqMapper,
        IMapper<UserDomain, PublicUserMAPI> resMapper
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(userId, utils.GetExecutorFromContext(ctx)),
            mapResponse: (user) => Results.Ok(resMapper.Map(user))
        );
    }
}
