using Domain.Entities.Questions;
using FluentValidation;
using FluentValidation.Results;


/// <summary>
/// Validador para preguntas de ordenamiento.
/// </summary>
public class OrderingQuestionFluentValidator : QuestionFluentValidator<OrderingQuestion>
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="OrderingQuestionFluentValidator"/>.
    /// </summary>
    public OrderingQuestionFluentValidator()
    {
        RuleFor(q => q.Sequence)
            .NotNull()
            .WithMessage("Campo requerido")
            .NotEmpty()
            .WithMessage("Campo requerido")
            .Must(sequence => sequence.Count >= 2)
            .WithMessage("Al menos 2 elementos")
            .Custom((sequence, ctx) =>
            {
                for (var i = 0; i < sequence.Count; i++)
                {
                    var option = sequence[i];

                    if (!string.IsNullOrEmpty(option))
                        continue;


                    var field = $"sequence[{i}]";
                    var error = $"Campo requerido";
                    ctx.AddFailure(new ValidationFailure(field, error));
                }
            });
    }
}
