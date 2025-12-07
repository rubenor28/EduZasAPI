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

public sealed class DeleteUserUseCase(
    IDeleterAsync<ulong, UserDomain> deleter,
    IReaderAsync<ulong, UserDomain> reader,
    IQuerierAsync<UserDomain, UserCriteriaDTO> querier,
    IBusinessValidationService<ulong>? validator = null
) : DeleteUseCase<ulong, UserDomain>(deleter, reader, validator)
{
    private readonly IQuerierAsync<UserDomain, UserCriteriaDTO> _querier = querier;

    protected override Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<ulong> value,
        UserDomain record
    ) =>
        value.Executor.Role switch
        {
            UserType.ADMIN => Unit.Value,
            _ => UseCaseErrors.Unauthorized(),
        };

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ulong> value,
        UserDomain record
    )
    {
        if (record.Role == UserType.ADMIN)
        {
            var adminsCount = await _querier.CountAsync(new() { Role = UserType.ADMIN });
            if (adminsCount >= 1)
                return UseCaseErrors.Conflict("Debe haber al menos un administrador");
        }

        return Unit.Value;
    }
}
