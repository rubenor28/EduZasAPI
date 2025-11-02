using Application.DTOs.Tests;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Tests;

public sealed class NewTestFluentValidator : FluentValidator<NewTestDTO>
{
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
