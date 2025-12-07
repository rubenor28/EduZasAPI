using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.UseCases.Auth;
using Application.UseCases.Users;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Presentation.Filters;

namespace MinimalAPI.Presentation.Routes;

public static class UserRoutes
{
    public static RouteGroupBuilder MapUserRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/users").WithTags("Usuarios");

        group
            .MapPost("/", AddUser)
            .WithName("Agregar usuario")
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PublicUserDTO>(StatusCodes.Status201Created)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<MessageResponse>(StatusCodes.Status409Conflict)
            .WithOpenApi(op =>
            {
                op.Summary = "Registrar un nuevo usuario en el sistema.";
                op.Description = "Crea una cuenta de usuario con la información proporcionada.";
                op.Responses["201"].Description = "El usuario fue creado exitosamente.";
                op.Responses["400"].Description =
                    "Los datos proporcionados tienen un formato incorrecto.";
                op.Responses["409"].Description = "El email proporcionado ya está registrado.";
                return op;
            });

        group
            .MapPost("/all", SearchUsers)
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PaginatedQuery<PublicUserDTO, UserCriteriaDTO>>(StatusCodes.Status200OK)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Buscar usuarios por criterios.";
                op.Description =
                    "Realiza una búsqueda paginada de usuarios con filtros. Requiere privilegios de administrador.";
                op.Responses["200"].Description = "Búsqueda completada exitosamente.";
                op.Responses["400"].Description = "Los criterios de búsqueda son inválidos.";
                op.Responses["401"].Description = "El usuario no está autenticado.";
                op.Responses["403"].Description = "El usuario no tiene permisos de administrador.";
                return op;
            });

        group
            .MapPut("/", UpdateUser)
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PublicUserDTO>(StatusCodes.Status200OK)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Actualizar un usuario.";
                op.Description =
                    "Modifica los datos de un usuario existente. Requiere privilegios de administrador.";
                op.Responses["200"].Description = "El usuario fue actualizado exitosamente.";
                op.Responses["400"].Description =
                    "Los datos proporcionados para la actualización son inválidos.";
                op.Responses["401"].Description = "El usuario no está autenticado.";
                op.Responses["403"].Description = "El usuario no tiene permisos de administrador.";
                op.Responses["404"].Description =
                    "No se encontró un usuario con el ID proporcionado.";
                return op;
            });

        group
            .MapDelete("/{userId:ulong}", DeleteUser)
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PublicUserDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Eliminar un usuario.";
                op.Description =
                    "Elimina un usuario de forma lógica (soft delete). Requiere privilegios de administrador.";
                op.Responses["200"].Description =
                    "El usuario fue marcado como inactivo exitosamente.";
                op.Responses["401"].Description = "El usuario no está autenticado.";
                op.Responses["403"].Description = "El usuario no tiene permisos de administrador.";
                op.Responses["404"].Description =
                    "No se encontró un usuario con el ID proporcionado.";
                return op;
            });

        group
            .MapGet("/{email}", GetUserByEmail)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PublicUserDTO>()
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Obtiene un usuario mediante un email.";
                op.Description = "Busca un usuario mediante el email proporcionado";
                op.Responses["200"].Description = "El usuario.";
                op.Responses["400"].Description = "El el formato del email no es apropiado.";
                op.Responses["401"].Description = "El usuario no está autenticado.";
                op.Responses["403"].Description = "El usuario no tiene permisos de administrador.";
                op.Responses["404"].Description =
                    "No se encontró un usuario con el email proporcionado.";

                return op;
            });

        group
            .MapGet("/{userId:ulong}", GetUserById)
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PublicUserDTO>()
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Obtiene un usuario mediante un ID.";
                op.Description = "Busca un usuario mediante el ID proporcionado";
                op.Responses["200"].Description = "El usuario.";
                op.Responses["400"].Description = "El el formato del ID no es apropiado.";
                op.Responses["401"].Description = "El usuario no está autenticado.";
                op.Responses["403"].Description = "El usuario no tiene permisos de administrador.";
                op.Responses["404"].Description =
                    "No se encontró un usuario con el ID proporcionado.";

                return op;
            });

        return group;
    }

    public static Task<IResult> SearchUsers(
        [FromBody] UserCriteriaDTO criteria,
        [FromServices] UserQueryUseCase useCase,
        [FromServices] IMapper<PaginatedQuery<UserDomain, UserCriteriaDTO>, PaginatedQuery<PublicUserDTO, UserCriteriaDTO>> resMapper,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => criteria,
            mapResponse: (search) => Results.Ok(resMapper.Map(search))
        );
    }

    public static Task<IResult> UpdateUser(
        [FromBody] UserUpdateDTO request,
        [FromServices] UpdateUserUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils,
        [FromServices] IMapper<UserDomain, PublicUserDTO> resMapper
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: (user) => Results.Ok(resMapper.Map(user))
        );
    }

    public static Task<IResult> DeleteUser(
        [FromRoute] ulong userId,
        [FromServices] DeleteUserUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils,
        [FromServices] IMapper<UserDomain, PublicUserDTO> resMapper
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => userId,
            mapResponse: (user) => Results.Ok(resMapper.Map(user))
        );
    }

    public static Task<IResult> GetUserByEmail(
        [FromRoute] string email,
        [FromServices] ReadUserEmailUseCase useCase,
        [FromServices] IMapper<UserDomain, PublicUserDTO> resMapper,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => email,
            mapResponse: (user) => Results.Ok(resMapper.Map(user))
        );
    }

    public static Task<IResult> GetUserById(
        [FromRoute] ulong userId,
        [FromServices] ReadUserUseCase useCase,
        [FromServices] IMapper<UserDomain, PublicUserDTO> resMapper,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => userId,
            mapResponse: (user) => Results.Ok(resMapper.Map(user))
        );
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="newUser">DTO con la información del nuevo usuario.</param>
    /// <param name="useCase">Caso de uso para registrar al usuario.</param>
    /// <param name="utils">Utilidad para manejar respuestas y excepciones.</param>
    /// <returns>Un <see cref="IResult"/> con el resultado de la operación.</returns>
    public static async Task<IResult> AddUser(
        [FromBody] NewUserDTO request,
        [FromServices] AddUserUseCase useCase,
        [FromServices] RoutesUtils utils,
        [FromServices] IMapper<UserDomain, PublicUserDTO> userMapper,
        HttpContext ctx
    )
    {
        return await utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: (user) => Results.Created($"/users/{user.Id}", userMapper.Map(user))
        );
    }
}
