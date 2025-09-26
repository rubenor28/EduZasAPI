using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;
using EduZasAPI.Application.Auth;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Common;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Users;
using EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;

namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Auth;

public static class AuthRoutes
{
    public static RouteGroupBuilder MapAuthRoutes(this WebApplication app)
    {
        var group = app
          .MapGroup("/auth")
          .WithTags("Autenticacion");

        group
          .MapPost("/", Login)
          .WithName("Iniciar sesión");

        group
          .MapGet("/", () => { })
          .WithName("Verificar autenticado");

        group
          .MapDelete("/", () => Results.Ok("Acceso a ruta protegida"))
          .WithName("Cerrar sesión")
          .RequireAuthorization();

        return group;
    }

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
                var errList = new List<FieldErrorDTO>()
                {
                  validation.UnwrapErr()
                };

                var response = new FieldErrorResponse
                {
                    Message = "Error al iniciar sesión",
                    Errors = errList
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

            var publicUser = PublicUserMAPIMapper.FromDomain(user.ToPublicUserDTO());
            return Results.Ok(publicUser);
        });
    }
}
