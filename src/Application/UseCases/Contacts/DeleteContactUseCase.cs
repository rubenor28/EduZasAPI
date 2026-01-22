using Application.DAOs;
using Application.DTOs.Contacts;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Contacts;

using ContactDeleter = IDeleterAsync<ContactIdDTO, ContactDomain>;
using ContactReader = IReaderAsync<ContactIdDTO, ContactDomain>;

/// <summary>
/// Caso de uso para eliminar un contacto y limpiar etiquetas huérfanas.
/// </summary>
public sealed class DeleteContactUseCase(
    ContactDeleter deleter,
    ContactReader reader
) : DeleteUseCase<ContactIdDTO, ContactDomain>(deleter, reader, null)
{
    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ContactIdDTO> value,
        ContactDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => record.AgendaOwnerId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }
}
