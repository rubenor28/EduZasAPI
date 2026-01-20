using Application.Services.Validators;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Answers.QuestionAnswers;

public class ConceptRelationQuestionAnswerFluentValidator
    : FluentValidator<(ConceptRelationQuestionAnswer, ConceptRelationQuestion)>,
        IConceptRelationQuestionAnswerValidator
{
    public ConceptRelationQuestionAnswerFluentValidator()
    {
        RuleFor(answer => answer.Item1.AnsweredPairs)
            .Custom(
                (answeredPairsSet, ctx) =>
                {
                    var questionColumA = new HashSet<string>(
                        ctx.InstanceToValidate.Item2.Concepts.Count
                    );
                    var questionColumB = new HashSet<string>(
                        ctx.InstanceToValidate.Item2.Concepts.Count
                    );

                    foreach (var pair in ctx.InstanceToValidate.Item2.Concepts)
                    {
                        questionColumA.Add(pair.ConceptA);
                        questionColumB.Add(pair.ConceptB);
                    }

                    var idx = 0;
                    foreach (var pair in answeredPairsSet)
                    {
                        var existsConceptA = questionColumA.Contains(pair.ConceptA);
                        var existsConceptB = questionColumB.Contains(pair.ConceptB);

                        if (!existsConceptA)
                        {
                            var field = $"answeredPairs[{idx}].conceptA";
                            var error = $"{pair.ConceptA} no es un valor válido";
                            ctx.AddFailure(new ValidationFailure(field, error));
                        }

                        if (!existsConceptB)
                        {
                            var field = $"answeredPairs[{idx}].conceptB";
                            var error = $"{pair.ConceptB} no es un valor válido";
                            ctx.AddFailure(new ValidationFailure(field, error));
                        }

                        idx += 1;
                    }
                }
            );
    }
}
