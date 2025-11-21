using FluentValidation;

namespace FluentValidationProj.Application.Services.Common;

/// <summary>
/// Validador para valores de tipo <see cref="ulong"/>.
/// Se asegura de que el valor sea mayor o igual a 1.
/// </summary>
public class EmailFluentValidator : FluentValidator<string>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ULongFluentValidator"/>
    /// configurando la regla de validación.
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
