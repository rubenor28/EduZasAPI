using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Resources;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Resources;

public sealed class DeleteResourceUseCase(
    IDeleterAsync<ulong, ResourceDomain> deleter,
    IReaderAsync<ulong, ResourceDomain> reader,
    IBusinessValidationService<DeleteResourceDTO>? validator = null
) : DeleteUseCase<ulong, DeleteResourceDTO, ResourceDomain>(deleter, reader, validator)
{
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        DeleteResourceDTO value
    )
    {
        var resourceSearch = await _reader.GetAsync(value.Id);
        if (resourceSearch.IsNone)
            return UseCaseErrors.NotFound();

        var resource = resourceSearch.Unwrap();
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
