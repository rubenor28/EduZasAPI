using Domain.Entities;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using Domain.ValueObjects;
using Domain.ValueObjects.Grades;

namespace Application.Services.Graders;

public class AnswerGrader
{
    public Result<AnswerGrade, string> Grade(
        AnswerDomain answer,
        TestDomain test,
        CancellationToken ct = default
    )
    {
        var missingManualGrades = test.RequiresManualGrade.Any(id =>
            !answer.Metadata.ManualGrade.ContainsKey(id)
        );

        if (missingManualGrades)
            return "Calificacion manual requerida";

        var grades = test
            .Content.AsParallel()
            .WithCancellation(ct)
            .Select(kvp =>
            {
                var (id, question) = kvp;
                var questionAnswer = answer.Content[id];
                answer.Metadata.ManualGrade.TryGetValue(id, out var manualGrade);
                return CreateGrade(question, questionAnswer, manualGrade);
            })
            .ToList();

        var totalPoints = (uint)grades.Sum(g => g.TotalPoints);
        var earnedPoints = (uint)grades.Sum(g => g.Points);

        return new AnswerGrade
        {
            StudentId = answer.UserId,
            Points = earnedPoints,
            TotalPoints = totalPoints,
            GradeDetails = grades,
        };
    }

    private Grade CreateGrade(IQuestion question, IQuestionAnswer answer, bool? manualGrade)
    {
        return (question, answer) switch
        {
            (ConceptRelationQuestion q, ConceptRelationQuestionAnswer qa) =>
                new ConceptRelationGrade
                {
                    Title = q.Title,
                    Pairs = [.. q.Concepts],
                    AnsweredPairs = [.. qa.AnsweredPairs],
                    ManualGrade = manualGrade,
                },
            (MultipleChoiseQuestion q, MultipleChoiseQuestionAnswer qa) => new MultipleChoiseGrade
            {
                Title = q.Title,
                Options = q.Options,
                CorrectOption = q.CorrectOption,
                SelectedOption = qa.SelectedOption,
                ManualGrade = manualGrade,
            },
            (MultipleSelectionQuestion q, MultipleSelectionQuestionAnswer qa) =>
                new MultipleSelectionGrade
                {
                    Title = q.Title,
                    Options = q.Options,
                    CorrectOptions = q.CorrectOptions,
                    AnsweredOptions = qa.SelectedOptions,
                    ManualGrade = manualGrade,
                },
            (OpenQuestion, OpenQuestionAnswer) => new OpenGrade
            {
                ManualGrade = manualGrade ?? false,
            },
            (OrderingQuestion q, OrderingQuestionAnswer qa) => new OrderingGrade
            {
                Title = q.Title,
                Sequence = q.Sequence,
                AnsweredSequence = qa.Sequence,
                ManualGrade = manualGrade,
            },
            _ => throw new InvalidOperationException(
                $"No es posible calificar pregunta de tipo {question.GetType().Name} con tipo de respuesta {answer.GetType().Name}"
            ),
        };
    }

    public async Task<IEnumerable<Result<AnswerGrade, IndividualGradeError>>> GradeManyAsync(
        IEnumerable<AnswerDomain> answers,
        TestDomain test,
        CancellationToken ct = default
    )
    {
        return await Task.Run(
            () =>
            {
                return answers
                    .AsParallel()
                    .WithCancellation(ct)
                    .WithDegreeOfParallelism(Environment.ProcessorCount)
                    .Select<AnswerDomain, Result<AnswerGrade, IndividualGradeError>>(answer =>
                    {
                        var result = Grade(answer, test, ct);
                        return result.IsErr
                            ? new IndividualGradeError(answer.UserId, result.UnwrapErr())
                            : result.Unwrap();
                    })
                    .ToList();
            },
            ct
        );
    }
}
