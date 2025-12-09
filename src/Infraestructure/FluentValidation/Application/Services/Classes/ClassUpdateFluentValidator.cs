using Application.DTOs.Classes;
using Domain.Extensions;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Classes;

/// <summary>
/// Validador para actualización de clases.
/// </summary>
public class ClassUpdateFluentValidator : FluentValidator<ClassUpdateDTO>
{
    /// <summary>
    /// Inicializa reglas de validación.
    /// </summary>
    public ClassUpdateFluentValidator()
    {
        RuleFor(c => c.ClassName)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .MinimumLength(3)
            .WithMessage("Al menos 3 caracteres");

        RuleFor(c => c.Subject)
            .Must(s => s.Match(str => str.Trim().Length >= 3, () => true))
            .WithMessage("Al menos 3 caracteres");

        RuleFor(c => c.Section)
            .Must(s => s.Match(str => str.Trim().Length >= 3, () => true))
            .WithMessage("Al menos 3 caracteres");
    }
}
