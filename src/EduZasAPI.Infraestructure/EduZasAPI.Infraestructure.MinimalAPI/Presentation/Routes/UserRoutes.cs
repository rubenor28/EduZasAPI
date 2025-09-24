using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;

using EduZasAPI.Application.Users;
using EduZasAPI.Application.Common;

using EduZasAPI.Infraestructure.MinimalAPI.Application.Common;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Users;

namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Users;

public static class UserRoutes
{
    public static RouteGroupBuilder MapUserRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/users")
          .WithTags("Usuarios");

        group.MapPost("/", AddUser);

        return group;
    }

    public async static Task<IResult> AddUser(
        NewUserMAPI newUser,
        IUserRepositoryAsync repo,
        AddUseCase<NewUserDTO, UserDomain> useCase)
    {
        try
        {
            var newUsr = NewUserMAPIMapper.ToDomain(newUser);
            var validation = await useCase.ExecuteAsync(
                request: newUsr,
                extraValidationAsync: async _ =>
                {
                    var errs = new List<FieldErrorDTO>();
                    var repeatedEmail = await repo.FindByEmail(newUsr.Email);

                    if (repeatedEmail.IsSome)
                    {
                        var error = new FieldErrorDTO { Field = "email", Message = "Email ya registrado" };
                        errs.Add(error);
                        return Result.Err(errs);
                    }

                    return Result<Unit, List<FieldErrorDTO>>.Ok(Unit.Value);
                }
            );

            if (validation.IsErr)
            {
                var errs = validation.UnwrapErr();
                var response = new FieldErrorResponse { Message = "Formato inválido", Errors = errs };
                return Results.BadRequest(response);
            }

            var newRecord = validation.Unwrap();
            return Results.BadRequest(new WithDataResponse<PublicUserMAPI>
            {
                Message = "Usuario registrado",
                Data = newRecord.FromDomain()
            });
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            return Results.InternalServerError(new MessageResponse
            {
                Message = "Internal server error",
            });
        }
    }
}
