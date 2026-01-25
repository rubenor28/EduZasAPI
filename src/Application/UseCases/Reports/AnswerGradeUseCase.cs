using Application.Configuration;
using Application.DAOs;
using Application.DTOs.Answers;
using Application.DTOs.ClassTests;
using Application.Services.Graders;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Domain.ValueObjects.Grades;

namespace Application.UseCases.Reports;

using IAnswerReader = IReaderAsync<AnswerIdDTO, AnswerDomain>;
using IClassReader = IReaderAsync<string, ClassDomain>;
using IClassTestReader = IReaderAsync<ClassTestIdDTO, ClassTestDomain>;
using IProfessorReader = IReaderAsync<UserClassRelationId, ClassProfessorDomain>;
using ITestReader = IReaderAsync<Guid, TestDomain>;
using IUserReader = IReaderAsync<ulong, UserDomain>;

public class AnswerGradeUseCase(
    IAnswerReader answerReader,
    ITestReader testReader,
    IProfessorReader professorReader,
    AnswerGrader answerGrader,
    IUserReader userReader,
    IClassReader classReader,
    IClassTestReader classTestReader,
    GradeSettings settings
) : IUseCaseAsync<AnswerIdDTO, AnswerGradeDetail>
{
    private readonly IUserReader _userReader = userReader;
    private readonly ITestReader _testReader = testReader;
    private readonly AnswerGrader _answerGrader = answerGrader;
    private readonly IAnswerReader _answerReader = answerReader;
    private readonly GradeSettings _settings = settings;
    private readonly IProfessorReader _professorReader = professorReader;
    private readonly IClassTestReader _classTestReader = classTestReader;
    private readonly IClassReader _classReader = classReader;

    public async Task<Result<AnswerGradeDetail, UseCaseError>> ExecuteAsync(
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
            UserType.STUDENT => request.Data.UserId == request.Executor.Id,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var test =
            await _testReader.GetAsync(answer.TestId)
            ?? throw new InvalidOperationException(
                $"El test con ID: {answer.TestId} debería existir en este punto"
            );

        var gradeResult = _answerGrader.Grade(answer, test);

        if (gradeResult.IsErr)
            return UseCaseErrors.Conflict(gradeResult.UnwrapErr());

        var grade = gradeResult.Unwrap();

        var classTest =
            await _classTestReader.GetAsync(
                new() { ClassId = answer.ClassId, TestId = answer.TestId }
            )
            ?? throw new InvalidOperationException(
                $"Relación clase - test: {answer.ClassId} - {answer.TestId} debería existir en este punto"
            );

        var student =
            await _userReader.GetAsync(grade.StudentId)
            ?? throw new InvalidOperationException(
                $"El estudiante con ID: {grade.StudentId} debería existir en este punto"
            );

        var professor =
            await _userReader.GetAsync(test.ProfessorId)
            ?? throw new InvalidOperationException(
                $"El profesor con ID: {test.ProfessorId} debería existir en este punto"
            );

        var score = Math.Round((double)grade.Points / grade.TotalPoints * 100, 2);

        Console.WriteLine(
            $"[AnswerGradeDetail] Score: {score} PassThreshold: {_settings.PassThreshold}"
        );

        var cls =
            await _classReader.GetAsync(classTest.ClassId)
            ?? throw new InvalidOperationException(
                $"La clase con ID: {classTest.ClassId} debería existir en este punto"
            );

        return new AnswerGradeDetail
        {
            ClassName = cls.ClassName,
            StudentId = student.Id,
            StudentName = student.FullName,
            Score = score,
            Approved = score >= _settings.PassThreshold,
            Points = grade.Points,
            TotalPoints = grade.TotalPoints,
            Date = classTest.CreatedAt,
            TestId = test.Id,
            TestTitle = test.Title,
            ProfessorName = professor.FullName,
            GradeDetails = grade.GradeDetails,
        };
    }

    private async Task<bool> IsProfessorAuthorized(UserActionDTO<AnswerIdDTO> request)
    {
        var classProfessor = await _professorReader.GetAsync(
            new() { ClassId = request.Data.ClassId, UserId = request.Executor.Id }
        );

        return classProfessor is not null;
    }
}
