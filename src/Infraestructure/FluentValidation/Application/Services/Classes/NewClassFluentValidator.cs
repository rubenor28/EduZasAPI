using Application.DTOs.Classes;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Classes;

public class NewClassFluentValidator : FluentValidator<NewClassDTO>
{
    public NewClassFluentValidator()
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
