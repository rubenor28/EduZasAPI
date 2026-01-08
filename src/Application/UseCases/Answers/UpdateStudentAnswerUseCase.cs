using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Answers;
using Application.DTOs.Common;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Answers;

using AnswerReader = IReaderAsync<AnswerIdDTO, AnswerDomain>;
using AnswerValidator = IBusinessValidationService<AnswerUpdateStudentDTO, TestDomain>;
using StudentAnswerUpdater = IUpdaterAsync<AnswerDomain, AnswerUpdateStudentDTO>;
using TestReader = IReaderAsync<Guid, TestDomain>;

public sealed class StudentUpdateAnswerUseCase(
    StudentAnswerUpdater updater,
    AnswerReader reader,
    TestReader testReader,
    AnswerValidator answerValidator
) : UpdateUseCase<AnswerIdDTO, AnswerUpdateStudentDTO, AnswerDomain>(updater, reader, null)
{
    private readonly TestReader _testReader = testReader;
    private readonly AnswerValidator _answerValidator = answerValidator;

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
            _ => value.Data.UserId == value.Data.UserId,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var test =
            await _testReader.GetAsync(value.Data.TestId)
            ?? throw new InvalidDataException(
                $"El test con ID {value.Data.TestId} deberia existir en este punto"
            );

        var validation = _answerValidator.IsValid(value.Data, test);
        if (validation.IsErr)
            return UseCaseErrors.Input(validation.UnwrapErr());

        return Unit.Value;
    }
}
