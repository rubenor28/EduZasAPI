using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Auth;

public sealed class ReadUserUseCase(
    IReaderAsync<ulong, UserDomain> reader,
    IBusinessValidationService<ulong> validator
) : ReadUseCase<ulong, UserDomain>(reader, validator)
{
    protected override Result<Unit, UseCaseError> ExtraValidation(UserActionDTO<ulong> value)
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => value.Data == value.Executor.Id,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }
}
