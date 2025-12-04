using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Users;

public sealed class UpdateUserInfoUseCase(
    IUpdaterAsync<UserDomain, UserUpdateDTO> updater,
    IReaderAsync<ulong, UserDomain> reader,
    IBusinessValidationService<UserUpdateDTO>? validator = null
) : UpdateUseCase<ulong, UserUpdateDTO, UserDomain>(updater, reader, validator)
{
    protected override Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<UserUpdateDTO> value,
        UserDomain recrd
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => value.Data.Id == value.Executor.Id,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    protected override ulong GetId(UserUpdateDTO dto) => dto.Id;
}
