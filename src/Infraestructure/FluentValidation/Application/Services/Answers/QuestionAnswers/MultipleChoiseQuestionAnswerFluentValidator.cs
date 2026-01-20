using Application.Services.Validators;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Answers.QuestionAnswers;

public class MultipleChoiseQuestionAnswerFluentValidator
    : FluentValidator<(MultipleChoiseQuestionAnswer, MultipleChoiseQuestion)>,
        IMultipleChoiseQuestionAnswerValidator
{
    public MultipleChoiseQuestionAnswerFluentValidator()
    {
        RuleFor(tuple => tuple.Item1.SelectedOption)
            .Custom(
                (option, ctx) =>
                {
                    if (!option.HasValue)
                        return;

                    if (ctx.InstanceToValidate.Item2.Options.ContainsKey(option.Value))
                        return;

                    var field = "SelectedOption";
                    var message = "La opción seleccionada no corresponde a las opciones dispnibles";

                    ctx.AddFailure(new ValidationFailure(field, message));
                }
            );
    }
}
