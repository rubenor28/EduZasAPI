using Application.Services.Validators;
using Domain.Entities;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Answers;

public sealed class AnswerUpdateStudentFluentValidator
    : FluentValidator<(AnswerUpdateStudentDTO, TestDomain)>,
        IAnswerUpdateStudentValidator
{
    private readonly IOpenQuestionAnswerValidator _openQAValidator;
    private readonly IConceptRelationQuestionAnswerValidator _conceptRelationQAValidator;
    private readonly IMultipleChoiseQuestionAnswerValidator _multipleChoiseQAValidator;
    private readonly IMultipleSelectionQuestionAnswerValidator _multipleSelectionQAValidator;
    private readonly IOrderingQuestionAnswerValidator _orderingQuestionQAValidator;

    public AnswerUpdateStudentFluentValidator(
        IOpenQuestionAnswerValidator openQuestionAnswerValidator,
        IConceptRelationQuestionAnswerValidator conceptRelationQuestionAnswerValidator,
        IMultipleChoiseQuestionAnswerValidator multipleChoiseQuestionAnswerValidator,
        IMultipleSelectionQuestionAnswerValidator multipleSelectionQuestionAnswerValidator,
        IOrderingQuestionAnswerValidator orderingQuestionAnswerValidator
    )
    {
        _openQAValidator = openQuestionAnswerValidator;
        _conceptRelationQAValidator = conceptRelationQuestionAnswerValidator;
        _multipleChoiseQAValidator = multipleChoiseQuestionAnswerValidator;
        _multipleSelectionQAValidator = multipleSelectionQuestionAnswerValidator;
        _orderingQuestionQAValidator = orderingQuestionAnswerValidator;

        RuleFor(tuple => tuple.Item1.Content)
            .Custom(
                (content, ctx) =>
                {
                    if (content.Count == 0)
                        ctx.AddFailure(new ValidationFailure("content", "Campo requerido"));
                }
            );

        RuleForEach(tuple => tuple.Item1.Content)
            .Custom(
                (answerKvp, context) =>
                {
                    var (_, test) = context.InstanceToValidate;
                    var (id, answer) = answerKvp;

                    if (!test.Content.TryGetValue(id, out var question))
                    {
                        var fieldName = $"content[{id}]";
                        var message = $"Pregunta con ID {id} no existe";
                        context.AddFailure(new ValidationFailure(fieldName, message));
                        return;
                    }

                    var validation = (answer, question) switch
                    {
                        (ConceptRelationQuestionAnswer a, ConceptRelationQuestion q) =>
                            _conceptRelationQAValidator.IsValid((a, q)),

                        (MultipleChoiseQuestionAnswer a, MultipleChoiseQuestion q) =>
                            _multipleChoiseQAValidator.IsValid((a, q)),

                        (MultipleSelectionQuestionAnswer a, MultipleSelectionQuestion q) =>
                            _multipleSelectionQAValidator.IsValid((a, q)),

                        (OpenQuestionAnswer a, OpenQuestion q) => _openQAValidator.IsValid((a, q)),

                        (OrderingQuestionAnswer a, OrderingQuestion q) =>
                            _orderingQuestionQAValidator.IsValid((a, q)),

                        _ => null,
                    };

                    if (validation is null)
                    {
                        var message =
                            $"Tipo de pregunta {answer.GetType().Name} no soportada para tipo de pregunta {question.GetType().Name}";

                        return;
                    }

                    if (validation.IsErr)
                    {
                        foreach (var error in validation.UnwrapErr())
                        {
                            var fieldName = $"content[{id}].{error.Field}";
                            context.AddFailure(new ValidationFailure(fieldName, error.Message));
                        }
                    }
                }
            );
    }
}
