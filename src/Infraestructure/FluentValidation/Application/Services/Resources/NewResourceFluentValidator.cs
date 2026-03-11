using Application.DTOs.Resources;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Resources;

/// <summary>
/// Validador para la creación de nuevos recursos.
/// </summary>
public class NewResourceFluentValidator : FluentValidator<NewResourceDTO>
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="NewResourceFluentValidator"/>.
    /// </summary>
    public NewResourceFluentValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .MinimumLength(3)
            .WithMessage("Al menos 3 caracteres");

        RuleFor(r => r.Content)
            .NotEmpty()
            .WithMessage("Campo requerido");
    }
}

