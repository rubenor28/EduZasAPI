using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Users;

public sealed class DeleteUserUseCase(
    IDeleterAsync<ulong, UserDomain> deleter,
    IReaderAsync<ulong, UserDomain> reader,
    IBusinessValidationService<ulong>? validator = null
) : DeleteUseCase<ulong, UserDomain>(deleter, reader, validator)
{
    protected override Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<ulong> value,
        UserDomain record
    ) =>
        value.Executor.Role switch
        {
            UserType.ADMIN => Unit.Value,
            _ => UseCaseErrors.Unauthorized(),
        };
}
