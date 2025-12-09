using Application.DTOs.Tests;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Tests;

/// <summary>
/// Validador para creación de exámenes.
/// </summary>
public sealed class NewTestFluentValidator : FluentValidator<NewTestDTO>
{
    /// <summary>
    /// Inicializa reglas de validación.
    /// </summary>
    public NewTestFluentValidator()
    {
        RuleFor(t => t.Title)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .MinimumLength(3)
            .WithMessage("Al menos 3 caracteres");

        RuleFor(t => t.Content).NotEmpty().WithMessage("Campo requerido");
    }
}
