using System.Text.RegularExpressions;
using Application.DTOs.Users;
using Domain.Extensions;
using Domain.Rules;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Users;

/// <summary>
/// Validador para actualización de usuarios.
/// </summary>
public class UserUpdateFluentValidator : FluentValidator<UserUpdateDTO>
{
    /// <summary>
    /// Inicializa reglas de validación.
    /// </summary>
    public UserUpdateFluentValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .Must(x => x >= 1)
            .WithMessage("Formato inválido");

        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .Matches(UserRegexs.SimpleName)
            .WithMessage("Formato inválido");

        RuleFor(x => x.FatherLastname)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .Matches(UserRegexs.SimpleName)
            .WithMessage("Formato inválido");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .EmailAddress()
            .WithMessage("Formato inválido");

        When(x => x.Password is not null, () =>
        {
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("La contraseña no puede ser una cadena vacía.")
                .Matches(UserRegexs.Password)
                .WithMessage("La contraseña debe tener al menos 8 caracteres, incluyendo una mayúscula, una minúscula, un número y un caracter especial.");
        });

        RuleFor(x => x.MidName)
            .Must(opt =>
                opt.Match(
                    name =>
                        Regex.IsMatch(name, UserRegexs.SimpleName)
                        || Regex.IsMatch(name, UserRegexs.CompositeName),
                    () => true
                )
            )
            .WithMessage("Formato inválido");

        RuleFor(x => x.MotherLastname)
            .Must(opt =>
                opt.Match(
                    name =>
                        Regex.IsMatch(name, UserRegexs.SimpleName)
                        || Regex.IsMatch(name, UserRegexs.CompositeName),
                    () => true
                )
            )
            .WithMessage("Formato inválido");
    }
}
