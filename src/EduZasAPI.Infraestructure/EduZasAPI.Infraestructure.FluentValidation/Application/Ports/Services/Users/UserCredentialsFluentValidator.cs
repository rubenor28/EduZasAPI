namespace EduZasAPI.Infraestructure.Application.Ports.Services.Users;

using EduZasAPI.Application.DTOs.Users;
using EduZasAPI.Infraestructure.Application.Ports.Services.Common;

using FluentValidation;

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
