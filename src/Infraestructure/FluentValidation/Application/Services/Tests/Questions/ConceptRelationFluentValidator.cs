using Domain.Entities.Questions;
using FluentValidation;

public sealed class ConceptRelationFluentValidator
    : QuestionFluentValidator<ConceptRelationQuestion>
{
    public ConceptRelationFluentValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(q => q.Concepts)
            .NotNull()
            .WithMessage("Campo requerido.")
            .NotEmpty()
            .WithMessage("Campo requerido.");
    }
}
