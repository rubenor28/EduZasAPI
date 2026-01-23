using Application.Configuration;
using Application.DAOs;
using Application.DTOs.Answers;
using Application.DTOs.ClassTests;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Services.Graders;

using IAnswerQuerier = IQuerierAsync<AnswerDomain, AnswerCriteriaDTO>;
using IClassReader = IReaderAsync<string, ClassDomain>;
using IClassTestReader = IReaderAsync<ClassTestIdDTO, ClassTestDomain>;
using ITestReader = IReaderAsync<Guid, TestDomain>;
using IUserReader = IReaderAsync<ulong, UserDomain>;

public class ClassTestAnswersGrader(
    IClassTestReader classTestReader,
    IAnswerQuerier answerQuerier,
    ITestReader testReader,
    IUserReader userReader,
    IClassReader classReader,
    AnswerGrader answerGrader,
    GradeSettings settings
) : IUseCaseAsync<ClassTestIdDTO, GlobalClassTestReport>
{
    private readonly AnswerGrader _answerGrader = answerGrader;
    private readonly IAnswerQuerier _answerQuerier = answerQuerier;
    private readonly ITestReader _testReader = testReader;
    private readonly IClassTestReader _classTestReader = classTestReader;
    private readonly IClassReader _classReader = classReader;

    public async Task<Result<GlobalClassTestReport, UseCaseError>> ExecuteAsync(
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

        var answersSearch = await _answerQuerier.GetByAsync(
            new() { TestId = classTest.TestId, ClassId = classTest.ClassId }
        );

        if(answersSearch.Results.Count() == 0) return UseCaseErrors.Conflict("No existen respuestas para este test");

        var results = await _answerGrader.GradeManyAsync(answersSearch.Results, test);

        var splitted = results.ToLookup(r => r.IsOk);
        var successGrades = splitted[true].Select(r => r.Unwrap()).ToList();
        var errors = splitted[false].Select(r => r.UnwrapErr()).ToList();

        return new GlobalClassTestReport
        {
            ClassName = cls.ClassName,
            TestDate = classTest.CreatedAt,
            Errors = errors,
            TotalStudents = successGrades.Count + errors.Count,
            PassThreshold = settings.PassThreshold,
        };
    }
}
