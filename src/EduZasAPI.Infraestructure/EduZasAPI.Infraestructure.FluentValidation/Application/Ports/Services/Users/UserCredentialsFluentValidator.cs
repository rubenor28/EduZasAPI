using EduZasAPI.Application.Users;
using EduZasAPI.Infraestructure.FluentValidation.Application.Common;

using FluentValidation;

namespace EduZasAPI.Infraestructure.FluentValidation.Application.Users;

public class UserCredentialsFluentValidator : FluentValidator<UserCredentialsDTO>
{
    public UserCredentialsFluentValidator()
    {
        RuleFor(x => x.Email)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .WithMessage("La cadena no puede estar vacía")
          .EmailAddress()
          .WithMessage("Formato inválido");

        RuleFor(x => x.Password)
          .NotEmpty()
          .WithMessage("La cadena no puede estar vacía");
    }
}
