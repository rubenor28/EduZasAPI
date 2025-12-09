using Application.DTOs.Users;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Auth;

/// <summary>
/// Validador para credenciales de usuario.
/// </summary>
public class UserCredentialsFluentValidator : FluentValidator<UserCredentialsDTO>
{
    /// <summary>
    /// Inicializa reglas de validación.
    /// </summary>
    public UserCredentialsFluentValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .EmailAddress()
            .WithMessage("Formato inválido");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Campo requerido");
    }
}
