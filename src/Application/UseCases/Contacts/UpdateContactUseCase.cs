using Application.DAOs;
using Application.DTOs.Contacts;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Contacts;

/// <summary>
/// Caso de uso para actualizar un contacto.
/// </summary>
public sealed class UpdateContactUseCase(
    IUpdaterAsync<ContactDomain, ContactUpdateDTO> updater,
    IReaderAsync<ContactIdDTO, ContactDomain> reader,
    IBusinessValidationService<ContactUpdateDTO>? validator = null
) : UpdateUseCase<ContactIdDTO, ContactUpdateDTO, ContactDomain>(updater, reader, validator)
{
    /// <inheritdoc/>
    protected override ContactIdDTO GetId(ContactUpdateDTO dto) =>
        new() { UserId = dto.UserId, AgendaOwnerId = dto.AgendaOwnerId };
}
