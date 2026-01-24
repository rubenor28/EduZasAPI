using Application.DAOs;
using Application.DTOs.Answers;
using Application.DTOs.Tests;
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
using IUserReader = IReaderAsync<ulong, UserDomain>;

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
    IUserReader userReader
) : IUseCaseAsync<string, GlobalClassGradeReport>
{
    private readonly IUserReader _userReader = userReader;
    private readonly IClassReader _classReader = classReader;
    private readonly ITestQuerier _testQuerier = testQuerier;
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

        var allTests = new List<TestDomain>();
        var allAnswers = new Dictionary<Guid, List<AnswerDomain>>();

        var testCriteria = new TestCriteriaDTO { AssignedInClass = cls.Id, Page = 1 };

        while (true)
        {
            var testSearch = await _testQuerier.GetByAsync(testCriteria);
            allTests.AddRange(testSearch.Results);

            if (testSearch.Page >= testSearch.TotalPages)
                break;

            testCriteria.Page += 1;
        }

        foreach (var test in allTests)
        {
            var allTestAnswers = new List<AnswerDomain>();
            var criteria = new AnswerCriteriaDTO
            {
                ClassId = cls.Id,
                TestId = test.Id,
                Page = 1,
            };

            while (true)
            {
                var answerSearch = await _answerQuerier.GetByAsync(criteria);
                allTestAnswers.AddRange(answerSearch.Results);

                if (answerSearch.Page >= answerSearch.TotalPages)
                    break;

                criteria.Page += 1;
            }

            allAnswers.Add(test.Id, allTestAnswers);
        }
    }

    private async Task<bool> IsProfessorAuthorized(UserActionDTO<string> req)
    {
        var professor = await _professorReader.GetAsync(
            new() { ClassId = req.Data, UserId = req.Executor.Id }
        );

        return professor is not null;
    }

    private async Task<TestGrades> GetStudentsGrade(
        IEnumerable<AnswerDomain> answers,
        TestDomain test
    )
    {
        var gradeResults = await _answerGrader.SimpleGradeManyAsync(answers, test);

        var grades = gradeResults.ToLookup(r => r.IsOk);
        var successGrades = grades[true].Select(r => r.Unwrap()).ToList();
        var errors = grades[false].Select(r => r.UnwrapErr()).ToList();

        var formatTask = successGrades.Select(async (g) => {
            var student  = await _userReader.GetAsync(g.StudentId)
              ?? throw new InvalidOperationException($"Usuario con ID: {g.StudentId} debería existir en este punto");

            return new SimpleStudentGrade {
            Email = student.Email,
            StudentName = student.FullName,
            Score = Math.Round((double)g.Points / g.TotalPoints * 100, 2)
            };
            });

        Task.WaitAll(formatTask);

        return new() {
          TestTitle = test.Title,
          Errors = errors,
          StudentGrade = formatTask
        }
    }
}
