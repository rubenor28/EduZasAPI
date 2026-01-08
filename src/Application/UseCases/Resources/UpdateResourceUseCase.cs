using Application.DAOs;
using Application.DTOs.Resources;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Resources;

/// <summary>
/// Caso de uso para actualizar un recurso.
/// </summary>
public sealed class UpdateResourceUseCase(
    IUpdaterAsync<ResourceDomain, ResourceUpdateDTO> updater,
    IReaderAsync<Guid, ResourceDomain> reader,
    IBusinessValidationService<ResourceUpdateDTO>? validator = null
) : UpdateUseCase<Guid, ResourceUpdateDTO, ResourceDomain>(updater, reader, validator)
{
    /// <inheritdoc/>
    protected override Guid GetId(ResourceUpdateDTO dto) => dto.Id;
}
