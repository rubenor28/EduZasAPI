namespace EduZasAPI.Infraestructure.Application.Ports.Services.Users;

using System.Text.RegularExpressions;

using EduZasAPI.Domain.Rules;
using EduZasAPI.Application.DTOs.Users;
using EduZasAPI.Infraestructure.Application.Ports.Services.Common;

using FluentValidation;

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
          .WithMessage("La cadena no puede estar vacía")
          .Matches(UserRegexs.SimpleName)
          .WithMessage("Formato inválido");

        RuleFor(x => x.FatherLastName)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .WithMessage("La cadena no puede estar vacía")
          .Matches(UserRegexs.SimpleName)
          .WithMessage("Formato inválido");

        RuleFor(x => x.Email)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .WithMessage("La cadena no puede estar vacía")
          .EmailAddress()
          .WithMessage("Formato inválido");

        RuleFor(x => x.Password)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .WithMessage("La cadena no puede estar vacía")
          .Matches(UserRegexs.Password)
          .WithMessage("Formato inválido");

        RuleFor(x => x.MidName)
            .Must(opt => opt.Match(
                  name => Regex.IsMatch(UserRegexs.SimpleName, name) ||
                          Regex.IsMatch(UserRegexs.CompositeName, name),
                  () => true
            )).WithMessage("Formato inválido");

        RuleFor(x => x.MotherLastname)
            .Must(opt => opt.Match(
                  name => Regex.IsMatch(UserRegexs.SimpleName, name) ||
                          Regex.IsMatch(UserRegexs.CompositeName, name),
                  () => true
            )).WithMessage("Formato inválido");
    }
}
