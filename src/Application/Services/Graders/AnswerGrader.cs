using Domain.Entities;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using Domain.ValueObjects;
using Domain.ValueObjects.Grades;

/// <summary>
/// Proporciona métodos para calificar las respuestas de un examen.
/// </summary>
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
                if (answer.Content.TryGetValue(id, out var questionAnswer))
                {
                    var manualGraded = answer.Metadata.ManualGrade.TryGetValue(
                        id,
                        out var manualGrade
                    );

                    return CreateGrade(
                        id,
                        question,
                        questionAnswer,
                        manualGraded ? manualGrade : null
                    );
                }

                return new MissingAnswerGrade
                {
                    QuestionId = id,
                    Title = question.Title,
                    QuestionWeight = GetQuestionWeight(question),
                };
            })
            .ToList();

        return grades;
    }

    /// <summary>
    /// Califica la respuesta de un examen y devuelve un informe detallado de la calificación.
    /// </summary>
    /// <param name="answer">La respuesta del estudiante al examen.</param>
    /// <param name="test">El examen que se está calificando.</param>
    /// <param name="requireAllManualGrades">Indica si se requiere que todas las preguntas de calificación manual estén calificadas.</param>
    /// <returns>Un resultado que contiene el informe de calificación detallado o un mensaje de error.</returns>
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

    /// <summary>
    /// Califica la respuesta de un examen y devuelve una calificación simple (puntos obtenidos y totales).
    /// </summary>
    /// <param name="answer">La respuesta del estudiante al examen.</param>
    /// <param name="test">El examen que se está calificando.</param>
    /// <param name="requireAllManualGrades">Indica si se requiere que todas las preguntas de calificación manual estén calificadas.</param>
    /// <returns>Un resultado que contiene la calificación simple o un mensaje de error.</returns>
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

    /// <summary>
    /// Califica de forma asíncrona una colección de respuestas de examen y devuelve un informe detallado para cada una.
    /// </summary>
    /// <param name="answers">La colección de respuestas de los estudiantes.</param>
    /// <param name="test">El examen que se está calificando.</param>
    /// <param name="requireAllManualGrades">Indica si se requiere que todas las preguntas de calificación manual estén calificadas.</param>
    /// <param name="ct">El token de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con una colección de resultados de calificación detallada.</returns>
    public Task<IEnumerable<Result<AnswerGrade, IndividualGradeError>>> GradeManyAsync(
        IEnumerable<AnswerDomain> answers,
        TestDomain test,
        bool requireAllManualGrades = true,
        CancellationToken ct = default
    ) => ExecuteBatchAsync(answers, (ans, token) => Grade(ans, test, requireAllManualGrades), ct);

    /// <summary>
    /// Califica de forma asíncrona una colección de respuestas de examen y devuelve una calificación simple para cada una.
    /// </summary>
    /// <param name="answers">La colección de respuestas de los estudiantes.</param>
    /// <param name="test">El examen que se está calificando.</param>
    /// <param name="requireAllManualGrades">Indica si se requiere que todas las preguntas de calificación manual estén calificadas.</param>
    /// <param name="ct">El token de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con una colección de resultados de calificación simple.</returns>
    public Task<IEnumerable<Result<SimpleGrade, IndividualGradeError>>> SimpleGradeManyAsync(
        IEnumerable<AnswerDomain> answers,
        TestDomain test,
        bool requireAllManualGrades = true,
        CancellationToken ct = default
    ) => ExecuteBatchAsync(answers, (ans, token) => SimpleGrade(ans, test, requireAllManualGrades), ct);

    private uint GetQuestionWeight(IQuestion question) =>
        question switch
        {
            ConceptRelationQuestion q => (uint)q.Concepts.Count,
            MultipleChoiseQuestion => 1,
            MultipleSelectionQuestion q => (uint)q.Options.Count,
            OpenQuestion => 1,
            OrderingQuestion q => (uint)q.Sequence.Count,
            _
                => throw new InvalidOperationException(
                    $"No es posible calcular puntaje para pregunta de tipo {question.GetType().Name}"
                )
        };

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

