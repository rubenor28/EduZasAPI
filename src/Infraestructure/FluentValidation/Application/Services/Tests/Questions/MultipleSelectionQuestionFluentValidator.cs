using Domain.Entities.Questions;
using FluentValidation;

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
            .WithMessage("Campo requerido");

        RuleForEach(q => q.CorrectOptions)
            .NotNull()
            .Must((q, cOpt) => q.Options.ContainsKey(cOpt))
            .WithMessage(
                (_, cOpt) => $"La opción {cOpt} correcta no corresponde a una opción de la pregunta"
            );
    }
}
