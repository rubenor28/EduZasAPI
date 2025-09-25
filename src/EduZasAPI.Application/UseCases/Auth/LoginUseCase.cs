using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;

namespace EduZasAPI.Application.Auth;

public class LoginUseCase
{
    public async Task<Result<UserDomain, FieldErrorDTO>> Login(
        UserCredentialsDTO credentials,
        IUserRepositoryAsync repo,
        IHashService hasher)
    {
        var userSearch = await repo.FindByEmail(credentials.Email);

        if (userSearch.IsNone)
            return Result<UserDomain, FieldErrorDTO>
              .Err(new FieldErrorDTO
              {
                  Field = "email",
                  Message = "Email no encontrado",
              });

        var usr = userSearch.Unwrap();
        var pwdMatch = hasher.Matches(credentials.Password, usr.Password);

        if (!pwdMatch)
            return Result<UserDomain, FieldErrorDTO>
              .Err(new FieldErrorDTO
              {
                  Field = "password",
                  Message = "Contraseña incorrecta"
              });

        return Result<UserDomain, FieldErrorDTO>
          .Ok(usr);
    }
}
