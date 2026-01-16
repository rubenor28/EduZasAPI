using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Answers;
using Application.DTOs.ClassTests;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Answers;

using AnswerReader = IReaderAsync<AnswerIdDTO, AnswerDomain>;
using AnswerValidator = IAnswerUpdateStudentValidator;
using ClassTestReader = IReaderAsync<ClassTestIdDTO, ClassTestDomain>;
using StudentAnswerUpdater = IUpdaterAsync<AnswerDomain, AnswerUpdateStudentDTO>;
using TestReader = IReaderAsync<Guid, TestDomain>;

public sealed class UpdateStudentAnswerUseCase(
    StudentAnswerUpdater updater,
    AnswerReader reader,
    TestReader testReader,
    ClassTestReader classTestReader,
    AnswerValidator answerValidator
) : UpdateUseCase<AnswerIdDTO, AnswerUpdateStudentDTO, AnswerDomain>(updater, reader, null)
{
    private readonly TestReader _testReader = testReader;
    private readonly AnswerValidator _answerValidator = answerValidator;
    private readonly ClassTestReader _classTestReader = classTestReader;

    protected override AnswerIdDTO GetId(AnswerUpdateStudentDTO dto) =>
        new()
        {
            UserId = dto.UserId,
            ClassId = dto.ClassId,
            TestId = dto.TestId,
        };

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<AnswerUpdateStudentDTO> value,
        AnswerDomain original
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => await IsCommonUserAuthorized(value),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var test =
            await _testReader.GetAsync(value.Data.TestId)
            ?? throw new InvalidDataException(
                $"El test con ID {value.Data.TestId} deberia existir en este punto"
            );

        var validation = _answerValidator.IsValid((value.Data, test));
        if (validation.IsErr)
            return UseCaseErrors.Input(validation.UnwrapErr());

        return Unit.Value;
    }

    private async Task<bool> IsCommonUserAuthorized(UserActionDTO<AnswerUpdateStudentDTO> value)
    {
        if (value.Data.UserId != value.Executor.Id)
            return false;

        var test =
            await _testReader.GetAsync(value.Data.TestId)
            ?? throw new InvalidDataException(
                $"El test con ID {value.Data.TestId} deberia existir en este punto"
            );

        if (test.TimeLimitMinutes is null)
            return true;

        var classTest =
            await _classTestReader.GetAsync(
                new() { ClassId = value.Data.ClassId, TestId = value.Data.TestId }
            )
            ?? throw new InvalidDataException(
                $"El la relacion clase - evaluacion con ID de clase {value.Data.ClassId} y ID de evaluación {value.Data.TestId} deberia existir en este punto"
            );

        var startTime = classTest.CreatedAt.ToUniversalTime();
        var timeLimit = TimeSpan.FromMinutes(test.TimeLimitMinutes.Value);
        var deadline = startTime.Add(timeLimit);

        return DateTimeOffset.UtcNow <= deadline;
    }
}
