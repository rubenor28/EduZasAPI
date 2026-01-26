using Application.DAOs;
using Application.DTOs.Answers;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Answers;

using IAnswerProfessorUpdater = IUpdaterAsync<AnswerDomain, AnswerUpdateProfessorDTO>;
using IAnswerReader = IReaderAsync<AnswerIdDTO, AnswerDomain>;
using IProfessorReader = IReaderAsync<UserClassRelationId, ClassProfessorDomain>;

public class ProfessorUpdateAnswerUseCase(
    IProfessorReader professorReader,
    IAnswerProfessorUpdater updater,
    IAnswerReader reader,
    IBusinessValidationService<AnswerUpdateProfessorDTO>? validator = null
) : UpdateUseCase<AnswerIdDTO, AnswerUpdateProfessorDTO, AnswerDomain>(updater, reader, validator)
{
    private readonly IProfessorReader _professorReader = professorReader;

    protected override AnswerIdDTO GetId(AnswerUpdateProfessorDTO dto) =>
        new()
        {
            UserId = dto.UserId,
            ClassId = dto.ClassId,
            TestId = dto.TestId,
        };

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<AnswerUpdateProfessorDTO> value,
        AnswerDomain original
    )
    {
        var data = value.Data;
        var executor = value.Executor;

        var authorized = executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(data, executor.Id),
            _ => false,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    private async Task<bool> IsProfessorAuthorized(AnswerUpdateProfessorDTO data, ulong executorId)
    {
        var professor = await _professorReader.GetAsync(
            new() { ClassId = data.ClassId, UserId = executorId }
        );

        return professor is not null;
    }
}
