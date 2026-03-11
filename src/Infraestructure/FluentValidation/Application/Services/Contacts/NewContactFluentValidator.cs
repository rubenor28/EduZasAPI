using Application.DTOs.Contacts;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Contacts;

/// <summary>
/// Validador para la creación de nuevos contactos.
/// </summary>
public class NewContactFluentValidator : FluentValidator<NewContactDTO>
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="NewContactFluentValidator"/>.
    /// </summary>
    public NewContactFluentValidator()
    {
        RuleFor(c => c.Alias)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .MinimumLength(3)
            .WithMessage("Al menos 3 caracteres");
    }
}
