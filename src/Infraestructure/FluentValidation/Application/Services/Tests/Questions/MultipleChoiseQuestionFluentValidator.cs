using Domain.Entities.Questions;
using FluentValidation;

public class MultipleChoiseQuestionFluentValidator : QuestionFluentValidator<MultipleChoiseQuestion>
{
    public MultipleChoiseQuestionFluentValidator()
    {
        RuleFor(q => q.Options)
            .NotNull()
            .WithMessage("Campo requerido")
            .NotEmpty()
            .WithMessage("Campo requerido");

        RuleFor(q => q.CorrectOption)
            .Must((q, cOpt) => q.Options.ContainsKey(cOpt))
            .WithMessage("La opción correcta no corresponde a una opción de la pregunta");
    }
}
