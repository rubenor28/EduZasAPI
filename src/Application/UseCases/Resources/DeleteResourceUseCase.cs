using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Resources;

public sealed class DeleteResourceUseCase(
    IDeleterAsync<Guid, ResourceDomain> deleter,
    IReaderAsync<Guid, ResourceDomain> reader
) : DeleteUseCase<Guid, ResourceDomain>(deleter, reader, null)
{
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<Guid> value,
        ResourceDomain record
    )
    {
        var resource = await _reader.GetAsync(value.Data);
        if (resource is null)
            return UseCaseErrors.NotFound();

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => resource.ProfessorId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }
}
