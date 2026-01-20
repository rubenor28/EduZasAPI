using Application.Services.Validators;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Answers.QuestionAnswers;

public sealed class OrderingQuestionAnswerFluentValidator
    : FluentValidator<(OrderingQuestionAnswer, OrderingQuestion)>,
        IOrderingQuestionAnswerValidator
{
    public OrderingQuestionAnswerFluentValidator()
    {
        RuleFor(tuple => tuple.Item1.Sequence)
            .Custom(
                (answerSequence, ctx) =>
                {
                    var sequence = ctx.InstanceToValidate.Item2.Sequence.ToHashSet();
                    for (var i = 0; i < answerSequence.Count; i++)
                    {
                        var option = answerSequence[i];
                        if (!sequence.Contains(option))
                        {
                            var field = $"Sequence[{i}]";
                            var message =
                                $"La opción {option} no existe en la secuencia de la pregunta";

                            ctx.AddFailure(new ValidationFailure(field, message));
                        }
                    }
                }
            );
    }
}
