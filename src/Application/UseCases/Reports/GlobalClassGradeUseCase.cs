using Application.DAOs;
using Application.DTOs.Answers;
using Application.DTOs.Tests;
using Application.Services.Graders;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

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
    public required string TestName { get; init; }
    public required IEnumerable<SimpleStudentGrade> StudentGrade;
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

        var criteria = new TestCriteriaDTO { AssignedInClass = cls.Id };
        PaginatedQuery<TestDomain, TestCriteriaDTO> tests = null!;
        do
        {
            tests = await _testQuerier.GetByAsync(criteria);
              

            criteria = criteria with {Page = criteria.Page + 1};
        } while (tests.Page < tests.TotalPages);
    }

    private async Task<bool> IsProfessorAuthorized(UserActionDTO<string> req)
    {
        var professor = await _professorReader.GetAsync(
            new() { ClassId = req.Data, UserId = req.Executor.Id }
        );

        return professor is not null;
    }
}
