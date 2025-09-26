using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;
using EduZasAPI.Application.Auth;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Common;
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
          .MapDelete("/", () => { })
          .WithName("Cerrar sesión");

        return group;
    }

    public async static Task<IResult> Login(
        UserCredentialsDTO credentials,
        LoginUseCase useCase,
        RoutesUtils utils,
        JwtSettings jwtSettings,
        JwtService jwtService,
        HttpContext httpContext)
    {
        return await utils.HandleResponseAsync(async () =>
        {
            var validation = await useCase.ExecuteAsync(credentials);

            if (validation.IsErr)
            {
                var errList = new List<FieldErrorDTO>();
                var error = validation.UnwrapErr();
                var response = new FieldErrorResponse
                {
                    Message = "Formato inválido",
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
                // Secure = isProduction
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(jwtSettings.ExpiresMinutes)
            };

            httpContext.Response.Cookies.Append("AuthToken", token, cookieOpts);
            return Results.Ok(user.ToPublicUserDTO());
        });
    }
}
