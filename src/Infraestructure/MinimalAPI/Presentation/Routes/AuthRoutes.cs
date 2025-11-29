using Application.DTOs.Users;
using Application.UseCases.Auth;
using Domain.Entities;
using InterfaceAdapters.Mappers.Common;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Users;
using MinimalAPI.Application.Services;
using MinimalAPI.Presentation.Filters;

namespace MinimalAPI.Presentation.Routes;

/// <summary>
/// Define las rutas relacionadas con la autenticación de usuarios.
/// Contiene endpoints para registro, inicio/cierre de sesión y verificación de identidad.
/// </summary>
public static class AuthRoutes
{
    /// <summary>
    /// Mapea las rutas del grupo <c>/auth</c> y asocia cada endpoint
    /// con su respectivo manejador.
    /// </summary>
    /// <param name="app">Instancia de <see cref="WebApplication"/>.</param>
    /// <returns><see cref="RouteGroupBuilder"/> con las rutas configuradas.</returns>
    public static RouteGroupBuilder MapAuthRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/auth").WithTags("Autenticacion");

        group
            .MapPost("/sign-in", AddUser)
            .WithName("Registrar usuario")
            .Produces<PublicUserMAPI>(StatusCodes.Status201Created)
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
            .MapPost("/login", Login)
            .WithName("Iniciar sesión")
            .Produces<PublicUserMAPI>(StatusCodes.Status200OK)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .WithOpenApi(op =>
            {
                op.Summary = "Iniciar sesión en el sistema.";
                op.Description =
                    "Autentica a un usuario y, si tiene éxito, establece una cookie de sesión.";
                op.Responses["200"].Description = "Inicio de sesión exitoso.";
                op.Responses["400"].Description =
                    "Credenciales inválidas (email o contraseña incorrectos).";
                return op;
            });

        group
            .MapDelete("/logout", Logout)
            .WithName("Cerrar sesión")
            .RequireAuthorization("RequireAuthenticated")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi(op =>
            {
                op.Summary = "Cerrar la sesión del usuario actual.";
                op.Description =
                    "Invalida la sesión del usuario eliminando la cookie de autenticación.";
                op.Responses["204"].Description = "La sesión se cerró exitosamente.";
                op.Responses["401"].Description = "El usuario no está autenticado.";
                return op;
            });

        group
            .MapGet("/me", UserData)
            .WithName("Verificar autenticado")
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<UserIdFilter>()
            .Produces<PublicUserMAPI>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Obtener los datos del usuario autenticado.";
                op.Description =
                    "Devuelve la información del usuario correspondiente a la sesión activa.";
                op.Responses["200"].Description = "Datos del usuario obtenidos exitosamente.";
                op.Responses["401"].Description = "El usuario no está autenticado.";
                op.Responses["404"].Description =
                    "El usuario asociado al token de sesión no fue encontrado.";
                return op;
            });

        group
            .MapGet("/antiforgery/token", (IAntiforgery antiforgery, HttpContext context) =>
            {
                var tokens = antiforgery.GetAndStoreTokens(context);
                return Results.Ok(new { tokens.HeaderName, tokens.RequestToken });
            })
            .WithName("Obtener token antifalsificación")
            .ExcludeFromDescription();


        return group;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="newUser">DTO con la información del nuevo usuario.</param>
    /// <param name="useCase">Caso de uso para registrar al usuario.</param>
    /// <param name="utils">Utilidad para manejar respuestas y excepciones.</param>
    /// <returns>Un <see cref="IResult"/> con el resultado de la operación.</returns>
    public static async Task<IResult> AddUser(
        NewUserMAPI request,
        AddUserUseCase useCase,
        RoutesUtils utils,
        IMapper<NewUserMAPI, NewUserDTO> newUserMapper,
        IMapper<UserDomain, PublicUserMAPI> userMapper,
        HttpContext ctx
    )
    {
        return await utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => newUserMapper.Map(request),
            mapResponse: (user) => Results.Created($"/users/{user.Id}", userMapper.Map(user))
        );
    }

    /// <summary>
    /// Autentica a un usuario con sus credenciales.
    /// Si es válido, genera un JWT y lo guarda como cookie HttpOnly.
    /// </summary>
    /// <param name="credentials">Credenciales del usuario.</param>
    /// <param name="useCase">Caso de uso para validar credenciales.</param>
    /// <param name="utils">Utilidad para manejar respuestas y excepciones.</param>
    /// <param name="jwtSettings">Configuración del JWT.</param>
    /// <param name="jwtService">Servicio para generar el JWT.</param>
    /// <param name="httpContext">Contexto HTTP actual.</param>
    /// <param name="env">Entorno de ejecución (dev/prod).</param>
    /// <returns>Un <see cref="IResult"/> con el usuario con todos sus campos o error.</returns>
    public async static Task<IResult> Login(
        UserCredentialsDTO credentials,
        LoginUseCase useCase,
        IMapper<UserDomain, PublicUserMAPI> userMapper,
        RoutesUtils utils,
        JwtSettings jwtSettings,
        JwtService jwtService,
        HttpContext httpContext,
        IWebHostEnvironment env
    )
    {
        return await utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => credentials,
            mapResponse: (user) =>
            {
                var token = jwtService.Generate(new AuthPayload { Id = user.Id, Role = user.Role });

                var cookieOpts = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = env.IsProduction(),
                    SameSite = env.IsProduction() ? SameSiteMode.Strict : SameSiteMode.Lax,
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddMinutes(jwtSettings.ExpiresMinutes),
                };

                httpContext.Response.Cookies.Append("AuthToken", token, cookieOpts);

                return Results.Ok(userMapper.Map(user));
            }
        );
    }

    /// <summary>
    /// Cierra la sesión del usuario autenticado eliminando la cookie de autenticación.
    /// </summary>
    /// <param name="httpContext">Contexto HTTP actual.</param>
    /// <param name="env">Entorno de ejecución (dev/prod).</param>
    /// <returns>Un <see cref="IResult"/> indicando el éxito de la operación.</returns>
    public static IResult Logout(
        RoutesUtils utils,
        HttpContext httpContext,
        IWebHostEnvironment env
    )
    {
        return utils.HandleResponse(() =>
        {
            var cookieOpts = new CookieOptions
            {
                HttpOnly = true,
                Secure = env.IsProduction(),
                SameSite = env.IsProduction() ? SameSiteMode.Strict : SameSiteMode.Lax,
                Path = "/",
            };

            httpContext.Response.Cookies.Delete("AuthToken", cookieOpts);
            return Results.NoContent();
        });
    }

    public static async Task<IResult> UserData(
        HttpContext ctx,
        [FromServices] RoutesUtils utils,
        [FromServices] ReadUserUseCase useCase,
        [FromServices] IMapper<UserDomain, PublicUserMAPI> userMapper
    )
    {
        return await utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => utils.GetIdFromContext(ctx),
            mapResponse: (user) => Results.Ok(userMapper.Map(user))
        );
    }
}
