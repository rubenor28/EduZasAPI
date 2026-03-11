using Application.DTOs.Contacts;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Contacts;

/// <summary>
/// Validador para la actualización de contactos.
/// </summary>
public class ContactUpdateFluentValidator : FluentValidator<ContactUpdateDTO>
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ContactUpdateFluentValidator"/>.
    /// </summary>
    public ContactUpdateFluentValidator()
    {
        RuleFor(c => c.Alias)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .MinimumLength(3)
            .WithMessage("Al menos 3 caracteres");
    }
}
