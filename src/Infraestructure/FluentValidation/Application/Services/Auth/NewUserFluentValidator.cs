using System.Text.RegularExpressions;
using Application.DTOs.Users;
using Domain.Extensions;
using Domain.Rules;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Auth;

/// <summary>
/// Validador para creación de usuarios.
/// </summary>
public class NewUserFluentValidator : FluentValidator<NewUserDTO>
{
    /// <summary>
    /// Inicializa reglas de validación.
    /// </summary>
    public NewUserFluentValidator()
    {
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

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .Matches(UserRegexs.Password)
            .WithMessage("Formato inválido");

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
