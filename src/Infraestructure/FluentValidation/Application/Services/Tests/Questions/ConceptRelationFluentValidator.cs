using Domain.Entities.Questions;
using FluentValidation;
using FluentValidation.Results;


/// <summary>
/// Validador para preguntas de relación de conceptos.
/// </summary>
public sealed class ConceptRelationFluentValidator
    : QuestionFluentValidator<ConceptRelationQuestion>
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ConceptRelationFluentValidator"/>.
    /// </summary>
    public ConceptRelationFluentValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(q => q.Concepts)
            .NotNull()
            .WithMessage("Campo requerido.")
            .NotEmpty()
            .WithMessage("Campo requerido.")
            .Custom((options, ctx) =>
            {
                var optionsList = options.ToList();
                for (var i = 0; i < options.Count; i++)
                {
                    var pair = optionsList[i];

                    if (string.IsNullOrEmpty(pair.ConceptA))
                    {
                        var field = $"concepts[{i}].conceptA";
                        var error = $"Campo requerido";
                        ctx.AddFailure(new ValidationFailure(field, error));
                    }

                    if (string.IsNullOrEmpty(pair.ConceptB))
                    {
                        var field = $"concepts[{i}].conceptB";
                        var error = $"Campo requerido";
                        ctx.AddFailure(new ValidationFailure(field, error));
                    }
                }
            });
    }
}
