using Application.DAOs;
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
    IBusinessValidationService<DeleteUserDTO>? validator = null
) : DeleteUseCase<ulong, DeleteUserDTO, UserDomain>(deleter, reader, validator)
{
    protected override Result<Unit, UseCaseError> ExtraValidation(DeleteUserDTO value) =>
        value.Executor.Role switch
        {
            UserType.ADMIN => Unit.Value,
            _ => UseCaseErrors.Unauthorized(),
        };

    protected override ulong GetId(DeleteUserDTO value) => value.Id;

    protected override ulong GetId(UserDomain value) => value.Id;
}
