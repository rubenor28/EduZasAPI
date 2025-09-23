using FluentValidation;

namespace EduZasAPI.Infraestructure.FluentValidation.Application.Common;

/// <summary>
/// Validador para valores de tipo <see cref="ulong"/>.
/// Se asegura de que el valor sea mayor o igual a 1.
/// </summary>
public class ULongFluentValidator : FluentValidator<ulong>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ULongFluentValidator"/> 
    /// configurando la regla de validación.
    /// </summary>
    public ULongFluentValidator()
    {
        RuleFor(x => x)
            .Must(x => x >= 1)
            .WithMessage("El valor debe ser mayor o igual a 1");
    }
}
