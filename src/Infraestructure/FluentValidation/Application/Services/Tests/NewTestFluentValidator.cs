using Application.DTOs.Tests;
using Application.Services;
using Domain.Entities.Questions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Tests;

/// <summary>
/// Validador para creación de exámenes.
/// </summary>
public sealed class NewTestFluentValidator : FluentValidator<NewTestDTO>
{
    private readonly IBusinessValidationService<ConceptRelationQuestion> _conceptRelationValidator;
    private readonly IBusinessValidationService<MultipleChoiseQuestion> _multipleChoiceValidator;
    private readonly IBusinessValidationService<MultipleSelectionQuestion> _multipleSelectionValidator;
    private readonly IBusinessValidationService<OpenQuestion> _openQuestionValidator;
    private readonly IBusinessValidationService<OrderingQuestion> _orderingQuestionValidator;

    /// <summary>
    /// Inicializa reglas de validación.
    /// </summary>
    public NewTestFluentValidator(
        IBusinessValidationService<ConceptRelationQuestion> conceptRelationValidator,
        IBusinessValidationService<MultipleChoiseQuestion> multipleChoiceValidator,
        IBusinessValidationService<MultipleSelectionQuestion> multipleSelectionValidator,
        IBusinessValidationService<OpenQuestion> openQuestionValidator,
        IBusinessValidationService<OrderingQuestion> orderingQuestionValidator
    )
    {
        _conceptRelationValidator = conceptRelationValidator;
        _multipleChoiceValidator = multipleChoiceValidator;
        _multipleSelectionValidator = multipleSelectionValidator;
        _openQuestionValidator = openQuestionValidator;
        _orderingQuestionValidator = orderingQuestionValidator;

        RuleFor(t => t.Title)
            .NotEmpty()
            .WithMessage("Campo requerido")
            .MinimumLength(3)
            .WithMessage("Al menos 3 caracteres");

        RuleFor(t => t.Content).NotEmpty().WithMessage("Una evaluacion no puede estar vacía");

        RuleForEach(t => t.Content)
            .Custom(
                (questionKvp, context) =>
                {
                    var (id, question) = (questionKvp.Key, questionKvp.Value);

                    var validation = question switch
                    {
                        ConceptRelationQuestion q => _conceptRelationValidator.IsValid(q),
                        MultipleChoiseQuestion q => _multipleChoiceValidator.IsValid(q),
                        MultipleSelectionQuestion q => _multipleSelectionValidator.IsValid(q),
                        OpenQuestion q => _openQuestionValidator.IsValid(q),
                        OrderingQuestion q => _orderingQuestionValidator.IsValid(q),
                        _ => null,
                    };

                    if (validation is null)
                    {
                        var fieldName = $"content[{id}]";
                        var message = $"Tipo de pregunta no soportada: {question.GetType().Name}";
                        context.AddFailure(new ValidationFailure(fieldName, message));

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
