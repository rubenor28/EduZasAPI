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

    private async Task<ClassTestReport> EmptyReport(
        ClassDomain cls,
        TestDomain test,
        UserDomain professor,
        ClassTestDomain classTest,
        List<IndividualGradeError>? errors = null
    )
    {
        errors ??= [];

        return new()
        {
            ClassName = cls.ClassName,
            TestTitle = test.Title,
            ProfessorName = $"{professor.FirstName} {professor.FatherLastname}",
            TestDate = classTest.CreatedAt,
            Errors = await FormatErrors(errors),
            TotalStudents = errors?.Count ?? 0,
            PassThreshold = settings.PassThreshold,
            AveragePercentage = 0,
            MedianPercentage = 0,
            PassPercentage = 0,
            StandardDeviation = 0,
            MaxScore = 0,
            MinScore = 0,
            Results = [],
        };
    }

    private async Task<List<IndividualGradeErrorDetail>> FormatErrors(
        List<IndividualGradeError> errors
    )
    {
        var formattedErrors = new List<IndividualGradeErrorDetail>(errors.Count);
        foreach (var error in errors)
        {
            var user = await _userReader.GetAsync(error.UserId);
            NullException.ThrowIfNull(user);

            formattedErrors.Add(
                new()
                {
                    StudentId = user.Id,
                    StudentName = user.FullName,
                    Error = error.Error,
                }
            );
        }

        return formattedErrors;
    }

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

        var allAnswers = new List<AnswerDomain>();
        var criteria = new AnswerCriteriaDTO
        {
            TestId = classTest.TestId,
            ClassId = classTest.ClassId,
            Page = 1,
        };

        while (true)
        {
            var answersSearch = await _answerQuerier.GetByAsync(criteria);
            allAnswers.AddRange(answersSearch.Results);

            if (criteria.PageSize >= answersSearch.TotalPages)
                break;

            criteria.Page += 1;
        }

        if (allAnswers.Count == 0)
            return await EmptyReport(cls, test, professor, classTest);

        var results = await _answerGrader.GradeManyAsync(allAnswers, test);

        var splitted = results.ToLookup(r => r.IsOk);
        var successGrades = splitted[true].Select(r => r.Unwrap()).ToList();
        var errors = splitted[false].Select(r => r.UnwrapErr()).ToList();

        if (successGrades.Count == 0)
            return await EmptyReport(cls, test, professor, classTest, errors);

        var query = successGrades.Count switch
        {
            >= 1000 => successGrades
                .AsEnumerable()
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount),
            < 1000 => successGrades.AsEnumerable(),
        };

        var studentResults = query
            .Select(g => new StudentResult
            {
                Grade =
                    g.TotalPoints > 0 ? Math.Round((double)g.Points / g.TotalPoints * 100, 2) : 0.0,
                StudentId = g.StudentId,
            })
            .OrderBy(g => g.Grade)
            .ToList();

        var percentages = studentResults.Select(g => g.Grade / 100).ToList();

        var averagePercentage = percentages.Average();

        double medianPercentage;
        var mid = percentages.Count / 2;

        if (percentages.Count % 2 == 0)
            medianPercentage = (percentages[mid - 1] + percentages[mid]) / 2.0;
        else
            medianPercentage = percentages[mid];

        var passCount = percentages.Count(p => p >= settings.PassThresholdPercentage);
        var passRate = (double)passCount / successGrades.Count;

        var sumOfSquares = percentages.Sum(p => Math.Pow(p - averagePercentage, 2));
        var standardDeviation = Math.Sqrt(sumOfSquares / percentages.Count);

        var maxPoints = long.MinValue;
        var minPoints = long.MaxValue;
        uint? total = null;

        foreach (var g in successGrades)
        {
            total ??= g.TotalPoints;

            if (g.Points > maxPoints)
                maxPoints = g.Points;

            if (g.Points < minPoints)
                minPoints = g.Points;
        }

        var formattedResults = new List<StudentResultDetail>(studentResults.Count);
        foreach (var r in studentResults)
        {
            var user = await _userReader.GetAsync(r.StudentId);
            NullException.ThrowIfNull(user);

            formattedResults.Add(
                new()
                {
                    StudentId = r.StudentId,
                    Grade = r.Grade,
                    StudentName = user.FullName,
                }
            );
        }

        return new ClassTestReport
        {
            TestTitle = test.Title,
            ClassName = cls.ClassName,
            ProfessorName = $"{professor.FirstName} {professor.FatherLastname}",
            TestDate = classTest.CreatedAt,
            Errors = await FormatErrors(errors),
            TotalStudents = successGrades.Count,
            PassThreshold = settings.PassThreshold,
            AveragePercentage = Math.Round(averagePercentage * 100, 2),
            MedianPercentage = Math.Round(medianPercentage * 100, 2),
            PassPercentage = Math.Round(passRate * 100, 2),
            StandardDeviation = Math.Round(standardDeviation * 100, 3),
            MaxScore = Math.Round((double)maxPoints / total!.Value * 100, 2),
            MinScore = Math.Round((double)minPoints / total!.Value * 100, 2),
            Results = formattedResults,
        };
    }
}
