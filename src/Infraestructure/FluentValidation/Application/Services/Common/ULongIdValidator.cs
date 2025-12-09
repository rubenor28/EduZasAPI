using FluentValidation;

namespace FluentValidationProj.Application.Services.Common;

/// <summary>
/// Validador para IDs (ulong).
/// </summary>
public class ULongFluentValidator : FluentValidator<ulong>
{
    /// <summary>
    /// Inicializa el validador de IDs.
    /// </summary>
    public ULongFluentValidator()
    {
        RuleFor(x => x)
            .Must(x => x >= 1)
            .WithMessage("Id debe ser mayor a 0")
            .WithName("id");
    }
}
