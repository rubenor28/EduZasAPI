using EduZasAPI.Application.Users;
using EduZasAPI.Infraestructure.FluentValidation.Application.Common;

using FluentValidation;

namespace EduZasAPI.Infraestructure.FluentValidation.Application.Auth;

public class UserCredentialsFluentValidator : FluentValidator<UserCredentialsDTO>
{
    public UserCredentialsFluentValidator()
    {
        RuleFor(x => x.Email)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .WithMessage("Campo requerido")
          .EmailAddress()
          .WithMessage("Formato inválido");

        RuleFor(x => x.Password)
          .NotEmpty()
          .WithMessage("Campo requerido");
    }
}
