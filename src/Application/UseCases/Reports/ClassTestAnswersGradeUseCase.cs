using Application.Configuration;
using Application.DAOs;
using Application.DTOs.Answers;
using Application.DTOs.ClassTests;
using Application.Services.Graders;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.ValueObjects;
using Domain.ValueObjects.Grades;

namespace Application.UseCases.Reports;

using IAnswerQuerier = IQuerierAsync<AnswerDomain, AnswerCriteriaDTO>;
using IClassReader = IReaderAsync<string, ClassDomain>;
using IClassTestReader = IReaderAsync<ClassTestIdDTO, ClassTestDomain>;
using ITestReader = IReaderAsync<Guid, TestDomain>;
using IUserReader = IReaderAsync<ulong, UserDomain>;

public class ClassTestAnswersGradeUseCase(
    IClassTestReader classTestReader,
    IAnswerQuerier answerQuerier,
    AnswerGrader answerGrader,
    IClassReader classReader,
    GradeSettings settings,
    ITestReader testReader,
    IUserReader userReader
) : IUseCaseAsync<ClassTestIdDTO, ClassTestReport>
{
    private readonly AnswerGrader _answerGrader = answerGrader;
    private readonly IAnswerQuerier _answerQuerier = answerQuerier;
    private readonly ITestReader _testReader = testReader;
    private readonly IClassTestReader _classTestReader = classTestReader;
    private readonly IClassReader _classReader = classReader;
    private readonly IUserReader _userReader = userReader;

    private ClassTestReport EmptyReport(
        ClassDomain cls,
        UserDomain professor,
        ClassTestDomain classTest,
        List<IndividualGradeError>? errors = null
    ) =>
        new()
        {
            ClassName = cls.ClassName,
            ProfessorName = $"{professor.FirstName} {professor.FatherLastname}",
            TestDate = classTest.CreatedAt,
            Errors = errors ?? [],
            TotalStudents = errors?.Count ?? 0,
            PassThreshold = settings.PassThreshold,
            AveragePercentage = 0,
            MedianPercentage = 0,
            PassRate = 0,
            StandardDeviation = 0,
            MaxPoints = 0,
            MinPoints = 0,
            Results = [],
        };

    public async Task<Result<ClassTestReport, UseCaseError>> ExecuteAsync(
        UserActionDTO<ClassTestIdDTO> request
    )
    {
        var classTest = await _classTestReader.GetAsync(
            new() { ClassId = request.Data.ClassId, TestId = request.Data.TestId }
        );

        if (classTest is null)
            return UseCaseErrors.NotFound();

        var test =
            await _testReader.GetAsync(classTest.TestId)
            ?? throw new InvalidOperationException(
                $"El test con ID {classTest.TestId} debería existir en este punto"
            );

        var cls =
            await _classReader.GetAsync(classTest.ClassId)
            ?? throw new InvalidOperationException(
                $"La clase con ID {classTest.ClassId} debería existir en este punto"
            );

        var professor =
            await _userReader.GetAsync(test.ProfessorId)
            ?? throw new InvalidOperationException(
                $"Usuario con ID {test.ProfessorId} debería existir en este punto"
            );

        var answersSearch = await _answerQuerier.GetByAsync(
            new() { TestId = classTest.TestId, ClassId = classTest.ClassId }
        );

        if (answersSearch.Results.Count() == 0)
            return EmptyReport(cls, professor, classTest);

        var results = await _answerGrader.GradeManyAsync(answersSearch.Results, test);

        var splitted = results.ToLookup(r => r.IsOk);
        var successGrades = splitted[true].Select(r => r.Unwrap()).ToList();
        var errors = splitted[false].Select(r => r.UnwrapErr()).ToList();

        if (successGrades.Count == 0)
            return EmptyReport(cls, professor, classTest);

        var percentages = successGrades
            .Select(g => g.TotalPoints > 0 ? (double)g.Points / g.TotalPoints : 0.0)
            .OrderBy(p => p)
            .ToList();

        var studentResults = successGrades
            .Select(g => new StudentResult
            {
                Grade = g.TotalPoints > 0 ? Math.Round((double)g.Points / g.TotalPoints * 100, 2) : 0.0,
                StudentId = g.StudentId,
            })
            .OrderBy(g => g.Grade)
            .ToList();

        var averagePercentage = percentages.Average();

        double medianPercentage;
        var mid = percentages.Count / 2;

        if (percentages.Count % 2 == 0)
            medianPercentage = (percentages[mid - 1] + percentages[mid]) / 2.0;
        else
            medianPercentage = percentages[mid];

        var passCount = percentages.Count(p => p >= settings.PassThreshold);
        var passRate = (double)passCount / successGrades.Count;

        var sumOfSquares = percentages.Sum(p => Math.Pow(p - averagePercentage, 2));
        var standardDeviation = Math.Sqrt(sumOfSquares / percentages.Count);

        return new ClassTestReport
        {
            ClassName = cls.ClassName,
            ProfessorName = $"{professor.FirstName} {professor.FatherLastname}",
            TestDate = classTest.CreatedAt,
            Errors = errors,
            TotalStudents = successGrades.Count,
            PassThreshold = settings.PassThreshold,
            AveragePercentage = Math.Round(averagePercentage * 100, 2),
            MedianPercentage = Math.Round(medianPercentage * 100, 2),
            PassRate = Math.Round(passRate * 100, 2),
            StandardDeviation = Math.Round(standardDeviation * 100, 3),
            MaxPoints = successGrades.Max(g => g.Points),
            MinPoints = successGrades.Min(g => g.Points),
            Results = studentResults,
        };
    }
}
