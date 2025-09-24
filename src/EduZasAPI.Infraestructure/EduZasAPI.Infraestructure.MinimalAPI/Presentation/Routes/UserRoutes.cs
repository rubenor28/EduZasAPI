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
        IBusinessValidationService<NewUserDTO> validator)
    {
        try
        {
            var newUsr = NewUserMAPIMapper.ToDomain(newUser);

            var validation = validator.IsValid(newUsr);

            if (validation.IsErr)
            {
                var errs = validation.UnwrapErr();
                return Results.BadRequest(new FieldErrorResponse
                {
                    Message = "Formato inválido",
                    Errors = errs
                });
            }

            var repeatedEmail = await repo.FindByEmail(newUsr.Email);
            if (repeatedEmail.IsSome)
            {
                var errs = new List<FieldErrorDTO>();
                errs.Add(new FieldErrorDTO
                {
                    Field = "email",
                    Message = "Email ya registrado",
                });

                return Results.BadRequest(new FieldErrorResponse
                {
                    Message = "Formato inválido",
                    Errors = errs
                });
            }

            var newRecord = await repo.AddAsync(newUsr);
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
