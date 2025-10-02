using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;
using EduZasAPI.Application.Auth;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Common;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Users;
using EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;

namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Auth;

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
        var group = app
          .MapGroup("/auth")
          .WithTags("Autenticacion");

        group.MapPost("/sign-in", AddUser)
            .WithName("Registrar usuario")
            .Produces<PublicUserMAPI>(StatusCodes.Status201Created)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("/login", Login)
            .WithName("Iniciar sesión")
            .Produces<PublicUserMAPI>(StatusCodes.Status200OK)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/logout", Logout)
            .WithName("Cerrar sesión")
            .RequireAuthorization("RequireAuthenticated")
            .Produces(StatusCodes.Status200OK);

        group.MapGet("/me", UserData)
            .WithName("Verificar autenticado")
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<UserIdFilter>()
            .Produces<WithDataResponse<PublicUserMAPI>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return group;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="newUser">DTO con la información del nuevo usuario.</param>
    /// <param name="useCase">Caso de uso para registrar al usuario.</param>
    /// <param name="utils">Utilidad para manejar respuestas y excepciones.</param>
    /// <returns>Un <see cref="IResult"/> con el resultado de la operación.</returns>
    public static Task<IResult> AddUser(
        NewUserMAPI newUser,
        AddUserUseCase useCase,
        RoutesUtils utils)
    {
        return utils.HandleResponseAsync(async () =>
        {
            var newUsr = UserMAPIMapper.ToDomain(newUser);
            var validation = await useCase.ExecuteAsync(newUsr);

            if (validation.IsErr)
            {
                var errs = validation.UnwrapErr();
                var response = new FieldErrorResponse { Message = "Formato inválido", Errors = errs };
                return Results.BadRequest(response);
            }

            var newRecord = validation.Unwrap();
            var publicUser = UserMAPIMapper.FromDomain(newRecord);

            return Results.Created($"/users/{publicUser.Id}", publicUser);
        });
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
        RoutesUtils utils,
        JwtSettings jwtSettings,
        JwtService jwtService,
        HttpContext httpContext,
        IWebHostEnvironment env)
    {
        return await utils.HandleResponseAsync(async () =>
        {
            var validation = await useCase.ExecuteAsync(credentials);

            if (validation.IsErr)
            {
                var response = new FieldErrorResponse
                {
                    Message = "Error al iniciar sesión",
                    Errors = validation.UnwrapErr()
                };

                return Results.BadRequest(response);
            }

            var user = validation.Unwrap();

            var token = jwtService.Generate(new AuthPayload
            {
                Id = user.Id,
                Role = user.Role,
            });

            var cookieOpts = new CookieOptions
            {
                HttpOnly = true,
                Secure = env.IsProduction(),
                SameSite = env.IsProduction() ? SameSiteMode.Strict : SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(jwtSettings.ExpiresMinutes)
            };

            httpContext.Response.Cookies.Append("AuthToken", token, cookieOpts);

            var publicUser = UserMAPIMapper.FromDomain(user.ToPublicUserDTO());
            return Results.Ok(publicUser);
        });
    }

    /// <summary>
    /// Cierra la sesión del usuario autenticado eliminando la cookie de autenticación.
    /// </summary>
    /// <param name="httpContext">Contexto HTTP actual.</param>
    /// <param name="env">Entorno de ejecución (dev/prod).</param>
    /// <returns>Un <see cref="IResult"/> indicando el éxito de la operación.</returns>
    public static IResult Logout(HttpContext httpContext, IWebHostEnvironment env)
    {
        var cookieOpts = new CookieOptions
        {
            HttpOnly = true,
            Secure = env.IsProduction(),
            SameSite = env.IsProduction() ? SameSiteMode.Strict : SameSiteMode.Lax,
            Path = "/",
        };

        httpContext.Response.Cookies.Delete("AuthToken", cookieOpts);
        return Results.Ok();
    }

    public static async Task<IResult> UserData(HttpContext ctx, RoutesUtils utils, IReaderAsync<ulong, UserDomain> reader)
    {
        var userId = utils.GetIdFromContext(ctx);
        var usrSearch = await reader.GetAsync(userId);

        if (usrSearch.IsNone) return Results.Problem("Error al procesar al usuario");

        var usr = UserMAPIMapper.FromDomain(usrSearch.Unwrap());

        return Results.Ok(new WithDataResponse<PublicUserMAPI>
        {
            Message = "Autenticado",
            Data = usr
        });
    }
}
