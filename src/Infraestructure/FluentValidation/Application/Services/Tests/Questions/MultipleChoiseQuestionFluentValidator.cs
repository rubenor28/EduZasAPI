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
            .WithMessage("Campo requerido")
            .Must(opts => opts.Count >= 2)
            .WithMessage("Al menos 2 opciones");

        RuleFor(q => q.CorrectOption)
            .Must((q, cOpt) => q.Options.ContainsKey(cOpt))
            .WithMessage("La opción correcta no corresponde a una opción de la pregunta");
    }
}
