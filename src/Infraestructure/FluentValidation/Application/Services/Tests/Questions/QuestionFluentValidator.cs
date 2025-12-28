using Domain.Entities.Questions;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

public class QuestionFluentValidator<T> : FluentValidator<T>
    where T : IQuestion
{
    public QuestionFluentValidator()
    {
        RuleFor(q => q.Title).NotNull().NotEmpty().WithMessage("El título es un campo requerido.");

        RuleFor(q => q.ImageUrl)
            .NotEmpty()
            .WithMessage("La URL de la imagen no puede ser una cadena vacía.")
            .Null();
    }
}
