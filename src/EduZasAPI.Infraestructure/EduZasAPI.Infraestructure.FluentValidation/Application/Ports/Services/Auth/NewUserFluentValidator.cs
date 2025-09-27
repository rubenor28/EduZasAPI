using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Users;
using EduZasAPI.Infraestructure.FluentValidation.Application.Common;
using System.Text.RegularExpressions;
using FluentValidation;

namespace EduZasAPI.Infraestructure.FluentValidation.Application.Auth;

/// <summary>
/// Validador para la creación de un nuevo usuario,
/// aplicando reglas específicas a cada propiedad del DTO.
/// </summary>
public class NewUserFluentValidator : FluentValidator<NewUserDTO>
{
    /// <summary>
    /// Inicializa las reglas de validación para <see cref="NewUserDTO"/>.
    /// </summary>
    public NewUserFluentValidator()
    {
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
