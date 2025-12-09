using FluentValidation;

namespace FluentValidationProj.Application.Services.Common;

/// <summary>
/// Validador para emails.
/// </summary>
public class EmailFluentValidator : FluentValidator<string>
{
    /// <summary>
    /// Inicializa el validador de email.
    /// </summary>
    public EmailFluentValidator()
    {
        RuleFor(x => x)
            .EmailAddress()
            .NotNull()
            .NotEmpty()
            .WithMessage("Debe ser un email")
            .WithName("email");
    }
}
