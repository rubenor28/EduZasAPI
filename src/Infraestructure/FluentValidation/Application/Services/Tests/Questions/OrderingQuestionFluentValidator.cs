using Domain.Entities.Questions;
using FluentValidation;

public class OrderingQuestionFluentValidator : QuestionFluentValidator<OrderingQuestion>
{
    public OrderingQuestionFluentValidator()
    {
        RuleFor(q => q.Sequence)
            .NotNull()
            .WithMessage("Campo requerido")
            .NotEmpty()
            .WithMessage("Campo requerido")
            .Must(sequence => sequence.Count >= 2)
            .WithMessage("Al menos 2 elementos");
    }
}
