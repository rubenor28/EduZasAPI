using EduZasAPI.Application.Users;

using EduZasAPI.Infraestructure.MinimalAPI.Application.Common;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Users;

using EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;

namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Users;

public static class UserRoutes
{
    public static RouteGroupBuilder MapUserRoutes(this WebApplication app)
    {
        var group = app
          .MapGroup("/users")
          .WithTags("Usuarios");

        group
          .MapPost("/", AddUser)
          .WithName("Agregar usuario");

        return group;
    }

    public async static Task<IResult> AddUser(
        NewUserMAPI newUser,
        AddUserUseCase useCase,
        RoutesUtils utils)
    {
        return await utils.HandleResponseAsync(async () =>
        {

            var newUsr = NewUserMAPIMapper.ToDomain(newUser);
            var validation = await useCase.ExecuteAsync(newUsr);

            if (validation.IsErr)
            {
                var errs = validation.UnwrapErr();
                var response = new FieldErrorResponse { Message = "Formato inválido", Errors = errs };
                return Results.BadRequest(response);
            }

            var newRecord = validation.Unwrap();

            var publicUser = PublicUserMAPIMapper.FromDomain(newRecord);
            return Results.Created($"/users/{publicUser.Id}", publicUser);
        });
    }
}
