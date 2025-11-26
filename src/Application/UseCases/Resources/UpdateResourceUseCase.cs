using Application.DAOs;
using Application.DTOs.Resources;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Resources;

public sealed class UpdateResourceUseCase(
    IUpdaterAsync<ResourceDomain, ResourceUpdateDTO> updater,
    IReaderAsync<Guid, ResourceDomain> reader,
    IBusinessValidationService<ResourceUpdateDTO>? validator = null
) : UpdateUseCase<Guid, ResourceUpdateDTO, ResourceDomain>(updater, reader, validator)
{
    protected override Guid GetId(ResourceUpdateDTO dto) => dto.Id;
}
