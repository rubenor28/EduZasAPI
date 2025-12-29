using Domain.Entities.Questions;
using Domain.Extensions;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

public class QuestionFluentValidator<T> : FluentValidator<T>
    where T : IQuestion
{
    public QuestionFluentValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(q => q.Title).NotNull().NotEmpty().WithMessage("El título es un campo requerido.");

        RuleFor(q => q.ImageUrl)
            .Must(url => url.Match(u => !string.IsNullOrEmpty(u), () => true))
            .WithMessage("La URL de la imagen no puede ser una cadena vacía.");
    }
}
