using Application.DAOs;
using Application.DTOs.ClassResources;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.ClassResource;

public sealed class UpdateClassResourceUseCase(
    IUpdaterAsync<ClassResourceDomain, ClassResourceDTO> updater,
    IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> reader,
    IBusinessValidationService<ClassResourceDTO>? validator = null
)
    : UpdateUseCase<ClassResourceIdDTO, ClassResourceDTO, ClassResourceDomain>(
        updater,
        reader,
        validator
    )
{
    protected override ClassResourceIdDTO GetId(ClassResourceDTO dto) =>
        new() { ClassId = dto.ClassId, ResourceId = dto.ResourceId };
}
