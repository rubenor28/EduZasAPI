using Application.DAOs;
using Application.DTOs.Answers;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Answers;

public class ReadAnswerUseCase(
    IReaderAsync<AnswerIdDTO, AnswerDomain> reader,
    IBusinessValidationService<AnswerIdDTO>? validator = null
) : ReadUseCase<AnswerIdDTO, AnswerDomain>(reader, validator)
{
    protected override Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<AnswerIdDTO> value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => value.Data.UserId == value.Executor.Id,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }
}
