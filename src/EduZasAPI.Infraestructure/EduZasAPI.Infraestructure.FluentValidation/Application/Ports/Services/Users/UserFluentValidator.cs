using EduZasAPI.Domain.Users;
using EduZasAPI.Infraestructure.FluentValidation.Application.Common;
using FluentValidation;
using System.Text.RegularExpressions;

namespace EduZasAPI.Infraestructure.FluentValidation.Application.Users;

/// <summary>
/// Validador para la entidad de dominio <see cref="UserDomain"/>.
/// Aplica reglas de negocio y formato sobre sus propiedades.
/// </summary>
public class UserFluentValidator : FluentValidator<UserDomain>
{
    /// <summary>
    /// Inicializa las reglas de validación para <see cref="UserDomain"/>.
    /// </summary>
    public UserFluentValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .Must(x => x >= 1)
            .WithMessage("Debe ser mayor o igual a 1");

        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .Matches(UserRegexs.SimpleName)
            .WithMessage("Formato inválido");

        RuleFor(x => x.FatherLastName)
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
            .Must(opt => opt.Match(
                name => Regex.IsMatch(name, UserRegexs.SimpleName) ||
                        Regex.IsMatch(name, UserRegexs.CompositeName),
                () => true
            )).WithMessage("Formato inválido");

        RuleFor(x => x.MotherLastname)
            .Must(opt => opt.Match(
                name => Regex.IsMatch(name, UserRegexs.SimpleName) ||
                        Regex.IsMatch(name, UserRegexs.CompositeName),
                () => true
            )).WithMessage("Formato inválido");
    }
}
