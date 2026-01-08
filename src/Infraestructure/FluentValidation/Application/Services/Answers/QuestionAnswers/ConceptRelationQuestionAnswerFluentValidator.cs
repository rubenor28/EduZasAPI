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
                (pairsSet, ctx) =>
                {
                    if (pairsSet is null)
                        return;

                    var pairs = pairsSet.ToList();
                    var (_, question) = ctx.InstanceToValidate;
                    var columnASet = new HashSet<string>(pairsSet.Count);
                    var columnBSet = new HashSet<string>(pairsSet.Count);

                    foreach (var (conceptA, conceptB) in pairsSet)
                    {
                        columnASet.Add(conceptA);
                        columnBSet.Add(conceptB);
                    }

                    for (var i = 0; i < pairs.Count; i++)
                    {
                        var answeredPair = pairs[i];
                        if (!columnASet.Contains(answeredPair.ConceptA))
                        {
                            var fieldName = $"{ctx.PropertyPath}[{i}].ConceptA";
                            var message =
                                $"El concepto '{answeredPair.ConceptA}' no es una opción de la primera columna.";
                            ctx.AddFailure(new ValidationFailure(fieldName, message));
                        }

                        if (!columnBSet.Contains(answeredPair.ConceptB))
                        {
                            var fieldName = $"{ctx.PropertyPath}[{i}].ConceptB";
                            var message =
                                $"El concepto '{answeredPair.ConceptB}' no es una opción de la segunda columna.";
                            ctx.AddFailure(new ValidationFailure(fieldName, message));
                        }
                    }
                }
            );
    }
}
