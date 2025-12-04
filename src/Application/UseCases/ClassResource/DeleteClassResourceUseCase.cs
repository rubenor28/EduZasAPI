using Application.DAOs;
using Application.DTOs;
using Application.DTOs.ClassResources;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassResource;

public sealed class DeleteClassResourceUseCase(
    IDeleterAsync<ClassResourceIdDTO, ClassResourceDomain> deleter,
    IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> reader,
    IReaderAsync<Guid, ResourceDomain> resourceReader,
    IBusinessValidationService<ClassResourceIdDTO>? validator = null
)
    : DeleteUseCase<ClassResourceIdDTO, ClassResourceDomain>(
        deleter,
        reader,
        validator
    )
{
    private readonly IReaderAsync<Guid, ResourceDomain> _resourceReader = resourceReader;

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ClassResourceIdDTO> value,
        ClassResourceDomain record
    )
    {
        var resource = await _resourceReader.GetAsync(value.Data.ResourceId);
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
