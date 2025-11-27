using Application.DAOs;
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
    IBusinessValidationService<DeleteClassResourceDTO>? validator = null
)
    : DeleteUseCase<ClassResourceIdDTO, DeleteClassResourceDTO, ClassResourceDomain>(
        deleter,
        reader,
        validator
    )
{
    private readonly IReaderAsync<Guid, ResourceDomain> _resourceReader = resourceReader;

    protected override ClassResourceIdDTO GetId(DeleteClassResourceDTO value) =>
        new() { ClassId = value.ClassId, ResourceId = value.ResourceId };

    protected override ClassResourceIdDTO GetId(ClassResourceDomain value) =>
        new() { ClassId = value.ClassId, ResourceId = value.ResourceId };

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        DeleteClassResourceDTO value
    )
    {
        var resourceSearch = await _resourceReader.GetAsync(value.ResourceId);
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
