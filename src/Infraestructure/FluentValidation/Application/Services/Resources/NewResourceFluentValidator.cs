using Application.DTOs.Resources;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Resources;

public class NewResourceFluentValidator : FluentValidator<NewResourceDTO>
{
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

