using Application.DAOs;
using Application.DTOs.Answers;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using Domain.Enums;
using Domain.ValueObjects;
using Domain.ValueObjects.Grades;

namespace Application.Services.Graders;

using IAnswerReader = IReaderAsync<AnswerIdDTO, AnswerDomain>;
using IProfessorReader = IReaderAsync<UserClassRelationId, ClassProfessorDomain>;
using ITestReader = IReaderAsync<Guid, TestDomain>;

public class AnswerGradeUseCase(
    IAnswerReader answerReader,
    ITestReader testReader,
    IProfessorReader professorReader
) : IUseCaseAsync<AnswerIdDTO, TestGrade>
{
    private readonly ITestReader _testReader = testReader;
    private readonly IAnswerReader _answerReader = answerReader;
    private readonly IProfessorReader _professorReader = professorReader;

    public async Task<Result<TestGrade, UseCaseError>> ExecuteAsync(
        UserActionDTO<AnswerIdDTO> request
    )
    {
        var answer = await _answerReader.GetAsync(request.Data);
        if (answer is null)
            return UseCaseErrors.NotFound();

        var authorized = request.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(request),
            UserType => request.Data.UserId == request.Executor.Id,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var test =
            await _testReader.GetAsync(answer.TestId)
            ?? throw new InvalidOperationException(
                $"El test con ID: {answer.TestId} debería existir en este punto"
            );

        return await Task.Run(() => Grade(answer, test));
    }

    private async Task<bool> IsProfessorAuthorized(UserActionDTO<AnswerIdDTO> request)
    {
        var classProfessor = await _professorReader.GetAsync(
            new() { ClassId = request.Data.ClassId, UserId = request.Executor.Id }
        );

        return classProfessor is not null;
    }

    public Result<TestGrade, UseCaseError> Grade(AnswerDomain answer, TestDomain test)
    {
        var missingManualGrades = test.RequiresManualGrade.Any(id =>
            !answer.Metadata.ManualGrade.ContainsKey(id)
        );

        if (missingManualGrades)
            return UseCaseErrors.Conflict("Esta respuesta requiere calificación manual");

        var grades = test
            .Content.AsParallel()
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

        return new TestGrade
        {
            Points = earnedPoints,
            TotalPoints = totalPoints,
            GradeDetails = grades,
        };
    }

    private static Grade CreateGrade(IQuestion question, IQuestionAnswer answer, bool? manualGrade)
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
                $"Cannot grade question of type {question.GetType().Name} with answer of type {answer.GetType().Name}"
            ),
        };
    }
}
