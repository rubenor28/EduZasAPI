using Application.DTOs.Contacts;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Contacts;

public class NewContactFluentValidator : FluentValidator<NewContactDTO>
{
    public NewContactFluentValidator()
    {
        RuleFor(c => c.Alias)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .MinimumLength(3)
            .WithMessage("Al menos 3 caracteres");
    }
}
