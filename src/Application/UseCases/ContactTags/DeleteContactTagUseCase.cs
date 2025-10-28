using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.ContactTags;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ContactTags;

public sealed class DeleteContactTagUseCase(
    IDeleterAsync<ContactTagIdDTO, ContactTagDomain> deleter,
    IReaderAsync<ContactTagIdDTO, ContactTagDomain> reader,
    IReaderAsync<ulong, ContactDomain> contactReader,
    IBusinessValidationService<DeleteContactTagDTO>? validator = null
)
    : DeleteUseCase<ContactTagIdDTO, DeleteContactTagDTO, ContactTagDomain>(
        deleter,
        reader,
        validator
    )
{
    private readonly IReaderAsync<ulong, ContactDomain> _contactReader = contactReader;

    protected override async Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        DeleteContactTagDTO value
    )
    {
        var recordSearch = await _reader.GetAsync(value.Id);
        if (recordSearch.IsNone)
            return UseCaseError.NotFound();

        var record = recordSearch.Unwrap();

        var contactSearch = await _contactReader.GetAsync(record.Id.AgendaContactId);
        if (contactSearch.IsNone)
            throw new InvalidDataException(
                "Registro del contacto sobre el que se aplica una etiqueta no encontrado"
            );

        var contact = contactSearch.Unwrap();
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => contact.AgendaOwnerId == value.Executor.Id,
        };

        if (!authorized)
            return UseCaseError.Unauthorized();

        return Unit.Value;
    }
}
