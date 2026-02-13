using Domain.Entities.Questions;
using FluentValidation;
using FluentValidation.Results;

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
            .WithMessage("Al menos 2 opciones")
            .Custom((options, ctx) =>
            {
                foreach (var tuple in options)
                {
                    var (key, value) = tuple;

                    if (!string.IsNullOrEmpty(value))
                        continue;


                    var field = $"options[{key}]";
                    var error = $"Campo requerido";
                    ctx.AddFailure(new ValidationFailure(field, error));
                }
            });

        RuleFor(q => q.CorrectOption)
            .Must((q, cOpt) => q.Options.ContainsKey(cOpt))
            .WithMessage("La opción correcta no corresponde a una opción de la pregunta");
    }
}
