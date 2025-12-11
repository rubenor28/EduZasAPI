using Application.DTOs.Contacts;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Contacts;

public class ContactUpdateFluentValidator : FluentValidator<ContactUpdateDTO>
{
    public ContactUpdateFluentValidator()
    {
        RuleFor(c => c.Alias)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .MinimumLength(3)
            .WithMessage("Al menos 3 caracteres");
    }
}
