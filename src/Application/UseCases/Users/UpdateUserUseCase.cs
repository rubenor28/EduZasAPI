using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Users;

public sealed class UpdateUserUseCase(
    IUpdaterAsync<UserDomain, UserUpdateDTO> updater,
    IReaderAsync<ulong, UserDomain> reader,
    IBusinessValidationService<UserUpdateDTO>? validator = null
) : UpdateUseCase<ulong, UserUpdateDTO, UserDomain>(updater, reader, validator)
{
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserUpdateDTO value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => false,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var userSearch = await _reader.GetAsync(value.Id);
        if (userSearch.IsNone)
            return UseCaseErrors.NotFound();

        return Unit.Value;
    }
}
