using Application.DAOs;
using Application.DTOs.Answers;
using Application.DTOs.Tests;
using Application.DTOs.Users;
using Application.Services.Graders;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Domain.ValueObjects.Grades;

namespace Application.UseCases.Reports;

using IAnswerQuerier = IQuerierAsync<AnswerDomain, AnswerCriteriaDTO>;
using IClassReader = IReaderAsync<string, ClassDomain>;
using IProfessorReader = IReaderAsync<UserClassRelationId, ClassProfessorDomain>;
using ITestQuerier = IQuerierAsync<TestDomain, TestCriteriaDTO>;
using IUserQuerier = IQuerierAsync<UserDomain, UserCriteriaDTO>;

public record SimpleStudentGrade
{
    public required string Email { get; init; }
    public required string StudentName { get; init; }
    public required double Score { get; init; }
}

public record TestGrades
{
    public required string TestTitle { get; init; }
    public required IEnumerable<SimpleStudentGrade> StudentGrade { get; init; }
    public required IEnumerable<IndividualGradeError> Errors { get; init; }
}

public record GlobalClassGradeReport
{
    public required string ClassId { get; init; }
    public required string ClassName { get; init; }
    public required IEnumerable<TestGrades> TestsGrades { get; init; }
}

public class GlobalClassGradeUseCase(
    IProfessorReader professorReader,
    IAnswerQuerier answerQuerier,
    AnswerGrader answerGrader,
    IClassReader classReader,
    ITestQuerier testQuerier,
    IUserQuerier userQuerier
) : IUseCaseAsync<string, GlobalClassGradeReport>
{
    private readonly IClassReader _classReader = classReader;
    private readonly ITestQuerier _testQuerier = testQuerier;
    private readonly IUserQuerier _userQuerier = userQuerier;
    private readonly AnswerGrader _answerGrader = answerGrader;
    private readonly IAnswerQuerier _answerQuerier = answerQuerier;
    private readonly IProfessorReader _professorReader = professorReader;

    public async Task<Result<GlobalClassGradeReport, UseCaseError>> ExecuteAsync(
        UserActionDTO<string> req
    )
    {
        var cls = await _classReader.GetAsync(req.Data);

        if (cls is null)
            return UseCaseErrors.NotFound();

        var authorized = req.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(req),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var allTests = await GetAllPaginatedResults(
            _testQuerier,
            new TestCriteriaDTO { AssignedInClass = cls.Id }
        );

        var allGrades =
            new Dictionary<TestDomain, (List<SimpleStudentGrade>, List<IndividualGradeError>)>();

        foreach (var test in allTests)
        {
            var answers = await GetAllPaginatedResults(
                _answerQuerier,
                new AnswerCriteriaDTO { ClassId = cls.Id, TestId = test.Id }
            );

            var (studentGrades, studentGradesErrors) = await GetStudentsGrade(answers, test);

            allGrades.Add(test, (studentGrades, studentGradesErrors));
        }

        var testGrades = allGrades.Select(pair =>
        {
            var (test, (grades, errors)) = pair;

            return new TestGrades
            {
                Errors = errors,
                TestTitle = test.Title,
                StudentGrade = grades,
            };
        });

        return new GlobalClassGradeReport()
        {
            ClassId = cls.Id,
            ClassName = cls.ClassName,
            TestsGrades = testGrades,
        };
    }

    private async Task<bool> IsProfessorAuthorized(UserActionDTO<string> req)
    {
        var professor = await _professorReader.GetAsync(
            new() { ClassId = req.Data, UserId = req.Executor.Id }
        );

        return professor is not null;
    }

    private static async Task<List<T>> GetAllPaginatedResults<T, TParam>(
        IQuerierAsync<T, TParam> querier,
        TParam criteria
    )
        where T : notnull
        where TParam : CriteriaDTO
    {
        var allResults = new List<T>();
        var currentCriteria = criteria with { Page = 1 };

        while (true)
        {
            var searchResult = await querier.GetByAsync(currentCriteria);
            allResults.AddRange(searchResult.Results);

            if (searchResult.Page >= searchResult.TotalPages)
                break;

            currentCriteria = currentCriteria with { Page = currentCriteria.Page + 1 };
        }

        return allResults;
    }

    private async Task<(List<SimpleStudentGrade>, List<IndividualGradeError>)> GetStudentsGrade(
        IEnumerable<AnswerDomain> answers,
        TestDomain test
    )
    {
        var gradeResults = await _answerGrader.SimpleGradeManyAsync(answers, test);

        var grades = gradeResults.ToLookup(r => r.IsOk);
        var successGrades = grades[true].Select(r => r.Unwrap()).ToList();
        var errors = grades[false].Select(r => r.UnwrapErr()).ToList();

        if (!successGrades.Any())
        {
            return (new List<SimpleStudentGrade>(), errors);
        }

        var studentIds = successGrades.Select(g => g.StudentId).Distinct().ToList();

        var users = await GetAllPaginatedResults(
            _userQuerier,
            new UserCriteriaDTO { Ids = studentIds }
        );

        var userDictionary = users.ToDictionary(u => u.Id);

        var formattedGrades = new List<SimpleStudentGrade>();
        foreach (var g in successGrades)
        {
            if (!userDictionary.TryGetValue(g.StudentId, out var student))
            {
                throw new InvalidOperationException(
                    $"Usuario con ID: {g.StudentId} debería existir en este punto"
                );
            }

            formattedGrades.Add(
                new()
                {
                    Email = student.Email,
                    StudentName = student.FullName,
                    Score = Math.Round((double)g.Points / g.TotalPoints * 100, 2),
                }
            );
        }

        return (formattedGrades, errors);
    }
}
