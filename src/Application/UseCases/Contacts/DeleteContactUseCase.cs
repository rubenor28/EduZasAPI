using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Contacts;

public sealed class DeleteContactUseCase(
    IDeleterAsync<ulong, ContactDomain> deleter,
    IReaderAsync<ulong, ContactDomain> reader,
    IBusinessValidationService<DeleteContactDTO>? validator = null
) : DeleteUseCase<ulong, DeleteContactDTO, ContactDomain>(deleter, reader, validator)
{
    protected override async Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        DeleteContactDTO value
    )
    {
        var recordSearch = await _reader.GetAsync(value.Id);
        if (recordSearch.IsNone)
            return UseCaseError.NotFound();

        var record = recordSearch.Unwrap();

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => record.AgendaOwnerId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseError.Unauthorized();

        return Unit.Value;
    }
}
