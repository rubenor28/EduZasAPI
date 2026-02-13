using Domain.Entities.Questions;
using FluentValidation;
using FluentValidation.Results;

namespace FluentValidationProj.Application.Services.Tests.Questions;

public class MultipleSelectionQuestionFluentValidator
    : QuestionFluentValidator<MultipleSelectionQuestion>
{
    public MultipleSelectionQuestionFluentValidator()
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

        RuleForEach(q => q.CorrectOptions)
            .NotNull()
            .Must((q, cOpt) => q.Options.ContainsKey(cOpt))
            .WithMessage(
                (_, cOpt) => $"La opción {cOpt} correcta no corresponde a una opción de la pregunta"
            );
    }
}
