using Application.Services.Validators;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Answers.QuestionAnswers;

public class MultipleSelectionQuestionAnswerFluentValidator
    : FluentValidator<(MultipleSelectionQuestionAnswer, MultipleSelectionQuestion)>,
        IMultipleSelectionQuestionAnswerValidator
{
    public MultipleSelectionQuestionAnswerFluentValidator()
    {
        RuleFor(tuple => tuple.Item1.SelectedOptions)
            .Custom(
                (optionsSet, ctx) =>
                {
                    var question = ctx.InstanceToValidate.Item2;
                    var options = optionsSet.ToList();

                    for (var i = 0; i < options.Count; i++)
                    {
                        var option = options[i];
                        if (!question.Options.ContainsKey(option))
                        {
                            var fieldName = $"SelectedOptions[{i}]";
                            var message = $"La opcion '{option}' no es una opción válida";
                            ctx.AddFailure(new ValidationFailure(fieldName, message));
                        }
                    }
                }
            );
    }
}
