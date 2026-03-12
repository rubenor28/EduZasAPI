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
    IBusinessValidationService<ResourceUpdateDTO> validator
) : UpdateUseCase<Guid, ResourceUpdateDTO, ResourceDomain>(updater, reader, validator)
{
    /// <summary>
    /// Obtiene el identificador del recurso desde el DTO de actualización.
    /// </summary>
    /// <param name="dto">DTO de actualización.</param>
    /// <returns>ID del recurso.</returns>
    protected override Guid GetId(ResourceUpdateDTO dto) => dto.Id;
}
