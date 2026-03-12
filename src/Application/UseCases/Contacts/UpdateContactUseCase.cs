using Application.DAOs;
using Application.DTOs.Contacts;
using Application.Services.Validators;
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
    /// <summary>
    /// Obtiene el identificador compuesto del contacto desde el DTO de actualización.
    /// </summary>
    /// <param name="dto">DTO de actualización.</param>
    /// <returns>ID compuesto del contacto.</returns>
    protected override ContactIdDTO GetId(ContactUpdateDTO dto) =>
        new() { UserId = dto.UserId, AgendaOwnerId = dto.AgendaOwnerId };
}
