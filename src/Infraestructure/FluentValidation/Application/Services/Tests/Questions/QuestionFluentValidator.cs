using Domain.Entities.Questions;
using Domain.Extensions;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;


/// <summary>
/// Validador base para preguntas.
/// </summary>
/// <typeparam name="T">El tipo de pregunta a validar.</typeparam>
public class QuestionFluentValidator<T> : FluentValidator<T>
    where T : IQuestion
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="QuestionFluentValidator{T}"/>.
    /// </summary>
    public QuestionFluentValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(q => q.Title).NotNull().NotEmpty().WithMessage("El título es un campo requerido.");

        RuleFor(q => q.ImageUrl)
            .Must(url => url.Match(u => !string.IsNullOrEmpty(u), () => true))
            .WithMessage("La URL de la imagen no puede ser una cadena vacía.");
    }
}
