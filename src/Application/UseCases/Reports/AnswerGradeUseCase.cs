using Application.DAOs;
using Application.DTOs.Answers;
using Application.Services.Graders;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Reports;

using IAnswerReader = IReaderAsync<AnswerIdDTO, AnswerDomain>;
using IProfessorReader = IReaderAsync<UserClassRelationId, ClassProfessorDomain>;
using ITestReader = IReaderAsync<Guid, TestDomain>;

public class AnswerGradeUseCase(
    IAnswerReader answerReader,
    ITestReader testReader,
    IProfessorReader professorReader,
    AnswerGrader answerGrader
) : IUseCaseAsync<AnswerIdDTO, AnswerGrade>
{
    private readonly ITestReader _testReader = testReader;
    private readonly IAnswerReader _answerReader = answerReader;
    private readonly IProfessorReader _professorReader = professorReader;
    private readonly AnswerGrader _answerGrader = answerGrader;

    public async Task<Result<AnswerGrade, UseCaseError>> ExecuteAsync(
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

        var result = await Task.Run(() => _answerGrader.Grade(answer, test));

        if (result.IsErr)
            return UseCaseErrors.Conflict(result.UnwrapErr());

        return result.Unwrap();
    }

    private async Task<bool> IsProfessorAuthorized(UserActionDTO<AnswerIdDTO> request)
    {
        var classProfessor = await _professorReader.GetAsync(
            new() { ClassId = request.Data.ClassId, UserId = request.Executor.Id }
        );

        return classProfessor is not null;
    }
}
