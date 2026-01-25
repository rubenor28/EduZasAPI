using Application.DAOs;
using Application.DTOs.Answers;
using Application.DTOs.ClassTests;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.UseCases.Answers;

using IAnswerReader = IReaderAsync<AnswerIdDTO, AnswerDomain>;
using IClassTestReader = IReaderAsync<ClassTestIdDTO, ClassTestDomain>;
using ITestReader = IReaderAsync<Guid, TestDomain>;

public static class AnswerGradeState
{
    public static string Idle = "idle";
    public static string WaitingGrade = "waiting-grade";
    public static string Graded = "graded";
}

public class GetAnswerStateUseCase(
    IAnswerReader answerReader,
    IClassTestReader classTestReader,
    ITestReader testReader
) : IUseCaseAsync<AnswerIdDTO, string>
{
    private readonly IAnswerReader _answerReader = answerReader;
    private readonly IClassTestReader _classTestReader = classTestReader;
    private readonly ITestReader _testReader = testReader;

    public async Task<Result<string, UseCaseError>> ExecuteAsync(UserActionDTO<AnswerIdDTO> request)
    {
        var data = request.Data;

        var answer = await _answerReader.GetAsync(request.Data);
        if (answer is null)
            return UseCaseErrors.NotFound();

        var ctId = new ClassTestIdDTO { TestId = data.TestId, ClassId = data.ClassId };
        var ct = await _classTestReader.GetAsync(ctId);
        NullException.ThrowIfNull(ct);

        var test = await _testReader.GetAsync(data.TestId);
        NullException.ThrowIfNull(test);

        var manualGradeQuestions = test.RequiredManualGrade;
        var requiresManual = manualGradeQuestions.Any();
        var allowModify = ct.AllowModifyAnswers; // Flag global para todas las respuestas que indica si pueden seguir contestando
        var tryFinished = answer.TryFinished; // Flag individual para una respuesta que indica si pueden seguir contestando

        var isFinished = !allowModify || tryFinished;

        return (isFinished, requiresManual) switch
        {
            (false, _) => AnswerGradeState.Idle, // Si no ha terminado, siempre es Idle
            (true, true) => IsManualAlreadyGraded(manualGradeQuestions, answer), // Terminado y requiere calificacion manual
            (true, false) => AnswerGradeState.Graded, // Terminado y es automático
        };
    }

    private string IsManualAlreadyGraded(
        IEnumerable<Guid> manualGradeQuestions,
        AnswerDomain answer
    )
    {
        var missingToGrade = manualGradeQuestions.Any(q =>
            !answer.Metadata.ManualGrade.ContainsKey(q)
        );

        return missingToGrade ? AnswerGradeState.WaitingGrade : AnswerGradeState.Graded;
    }
}
