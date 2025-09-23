namespace EduZasAPI.Infraestructure.Application.Ports.Services.Common;

using FluentValidation;

public class ULongFluentValidator : FluentValidator<ulong>
{
    public ULongFluentValidator()
    {
        RuleFor(x => x)
            .Must(x => x >= 1)
            .WithMessage("El valor debe ser mayor o igual a 1");
    }
}
