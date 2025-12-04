using System.Text.RegularExpressions;
using Application.DTOs.Users;
using Domain.Extensions;
using Domain.Rules;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Users;

/// <summary>
/// Validador para la actualización de usuarios (<see cref="UserUpdateDTO"/>).
/// Aplica reglas de negocio y formato sobre las propiedades actualizables.
/// </summary>
public class UserUpdateFluentValidator : FluentValidator<UserUpdateDTO>
{
    /// <summary>
    /// Inicializa las reglas de validación para <see cref="UserUpdateDTO"/>.
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
