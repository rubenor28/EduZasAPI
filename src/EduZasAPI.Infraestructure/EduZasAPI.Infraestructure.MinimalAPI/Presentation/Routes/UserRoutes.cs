using EduZasAPI.Domain.Users;

using EduZasAPI.Application.Users;
using EduZasAPI.Application.Common;

using EduZasAPI.Infraestructure.MinimalAPI.Application.Users;

namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Users;

public static class UserRoutes
{
    public static RouteGroupBuilder MapUserRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/users")
          .WithTags("Usuarios");

        group.MapPost("/", AddUser);
    }

    public static IResult AddUser(NewUserMAPI newUser,
        IBusinessValidationService<NewUserDTO> validator,
        AddUseCase<NewUserDTO, UserDomain> useCase)
    {
        var newUsr = NewUserMAPIMapper.ToDomain(newUser);

        var validation = validator.IsValid(newUsr);

        if (validation.IsErr)
        {
            var errs = validation.UnwrapErr();
            return Results.BadRequest(errs);
        }
    }
}
