using Domain.Entities;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using Domain.ValueObjects;
using Domain.ValueObjects.Grades;

namespace Application.Services.Graders;

public class AnswerGrader
{
    private Result<List<Grade>, string> CalculateGradesInternal(
        AnswerDomain answer,
        TestDomain test,
        bool requireAllManualGrades
    )
    {
        if (requireAllManualGrades)
        {
            var missingManualGrades = test.RequiredManualGrade.Any(id =>
                !answer.Metadata.ManualGrade.ContainsKey(id)
            );

            if (missingManualGrades)
                return "Calificación manual requerida";
        }

        var grades = test
            .Content.Select(kvp =>
            {
                var (id, question) = kvp;
                var questionAnswer = answer.Content[id];
                var manualGraded = answer.Metadata.ManualGrade.TryGetValue(id, out var manualGrade);
                return CreateGrade(id, question, questionAnswer, manualGraded ? manualGrade : null);
            })
            .ToList();

        return grades;
    }

    public Result<AnswerGrade, string> Grade(
        AnswerDomain answer,
        TestDomain test,
        bool requireAllManualGrades = true
    )
    {
        var result = CalculateGradesInternal(answer, test, requireAllManualGrades);
        if (result.IsErr)
            return result.UnwrapErr();

        var grades = result.Unwrap();

        return new AnswerGrade
        {
            StudentId = answer.UserId,
            Points = (uint)grades.Sum(g => g.Points),
            TotalPoints = (uint)grades.Sum(g => g.TotalPoints),
            GradeDetails = grades,
        };
    }

    public Result<SimpleGrade, string> SimpleGrade(
        AnswerDomain answer,
        TestDomain test,
        bool requireAllManualGrades = true
    )
    {
        var result = CalculateGradesInternal(answer, test, requireAllManualGrades);
        if (result.IsErr)
            return result.UnwrapErr();

        var grades = result.Unwrap();

        return new SimpleGrade
        {
            StudentId = answer.UserId,
            Points = (uint)grades.Sum(g => g.Points),
            TotalPoints = (uint)grades.Sum(g => g.TotalPoints),
        };
    }

    private async Task<
        IEnumerable<Result<TResult, IndividualGradeError>>
    > ExecuteBatchAsync<TResult>(
        IEnumerable<AnswerDomain> answers,
        Func<AnswerDomain, CancellationToken, Result<TResult, string>> graderFunc,
        CancellationToken ct
    )
    {
        var answersList = answers as IList<AnswerDomain> ?? [.. answers];

        var query =
            answersList.Count >= 1000
                ? answersList
                    .AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount)
                    .WithCancellation(ct)
                : answersList.AsEnumerable();

        return await Task.Run(
            () =>
                query
                    .Select<AnswerDomain, Result<TResult, IndividualGradeError>>(answer =>
                    {
                        var result = graderFunc(answer, ct);
                        return result.IsErr
                            ? new IndividualGradeError(answer.UserId, result.UnwrapErr())
                            : result.Unwrap();
                    })
                    .ToList(),
            ct
        );
    }

    public Task<IEnumerable<Result<AnswerGrade, IndividualGradeError>>> GradeManyAsync(
        IEnumerable<AnswerDomain> answers,
        TestDomain test,
        bool requireAllManualGrades = true,
        CancellationToken ct = default
    ) => ExecuteBatchAsync(answers, (ans, token) => Grade(ans, test, requireAllManualGrades), ct);

    public Task<IEnumerable<Result<SimpleGrade, IndividualGradeError>>> SimpleGradeManyAsync(
        IEnumerable<AnswerDomain> answers,
        TestDomain test,
        bool requireAllManualGrades = true,
        CancellationToken ct = default
    ) => ExecuteBatchAsync(answers, (ans, token) => SimpleGrade(ans, test, requireAllManualGrades), ct);

    private Grade CreateGrade(
        Guid id,
        IQuestion question,
        IQuestionAnswer answer,
        bool? manualGrade
    )
    {
        return (question, answer) switch
        {
            (ConceptRelationQuestion q, ConceptRelationQuestionAnswer qa) =>
                new ConceptRelationGrade
                {
                    QuestionId = id,
                    Title = q.Title,
                    Pairs = [.. q.Concepts],
                    AnsweredPairs = [.. qa.AnsweredPairs],
                    ManualGrade = manualGrade,
                },
            (MultipleChoiseQuestion q, MultipleChoiseQuestionAnswer qa) => new MultipleChoiseGrade
            {
                QuestionId = id,
                Title = q.Title,
                Options = q.Options,
                CorrectOption = q.CorrectOption,
                SelectedOption = qa.SelectedOption,
                ManualGrade = manualGrade,
            },
            (MultipleSelectionQuestion q, MultipleSelectionQuestionAnswer qa) =>
                new MultipleSelectionGrade
                {
                    QuestionId = id,
                    Title = q.Title,
                    Options = q.Options,
                    CorrectOptions = q.CorrectOptions,
                    AnsweredOptions = qa.SelectedOptions,
                    ManualGrade = manualGrade,
                },
            (OpenQuestion q, OpenQuestionAnswer qa) => new OpenGrade
            {
                Title = q.Title,
                QuestionId = id,
                ManualGrade = manualGrade,
                Text = qa.Text,
            },
            (OrderingQuestion q, OrderingQuestionAnswer qa) => new OrderingGrade
            {
                QuestionId = id,
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
}
