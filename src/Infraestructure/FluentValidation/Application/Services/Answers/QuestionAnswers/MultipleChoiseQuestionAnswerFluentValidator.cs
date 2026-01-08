using Application.Services.Validators;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Answers.QuestionAnswers;

public class MultipleChoiseQuestionAnswerFluentValidator
    : FluentValidator<(MultipleChoiseQuestionAnswer, MultipleChoiseQuestion)>,
        IMultipleChoiseQuestionAnswerValidator
{
    public MultipleChoiseQuestionAnswerFluentValidator()
    {
        RuleFor(tuple => tuple.Item1.SelectedOption)
            .NotNull()
            .WithMessage("Campo requerido")
            .Must((tuple, option) => tuple.Item2.Options.ContainsKey(option))
            .WithMessage("No es una respuesta de la lista de opciones");
    }
}
