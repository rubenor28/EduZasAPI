using Application.Services.Validators;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Answers.QuestionAnswers;

/// <summary>
/// Validador para las respuestas a preguntas de ordenamiento.
/// </summary>
public sealed class OrderingQuestionAnswerFluentValidator
    : FluentValidator<(OrderingQuestionAnswer, OrderingQuestion)>,
        IOrderingQuestionAnswerValidator
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="OrderingQuestionAnswerFluentValidator"/>.
    /// </summary>
    public OrderingQuestionAnswerFluentValidator()
    {
        RuleFor(tuple => tuple.Item1.Sequence)
            .Custom(
                (answerSequence, ctx) =>
                {
                    if (ctx.InstanceToValidate.Item2.Sequence.Count != answerSequence.Count)
                    {
                        var field = $"Sequence";
                        var message =
                            "El número de elementos no corresponde al esperado en la pregunta";

                        ctx.AddFailure(new ValidationFailure(field, message));
                    }

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
