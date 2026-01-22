using Application.DAOs;
using Application.DTOs.Contacts;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ContactTags;

using ContactQuerier = IQuerierAsync<ContactDomain, ContactCriteriaDTO>;
using ContactTagDeleter = IDeleterAsync<ContactTagIdDTO, ContactTagDomain>;
using ContactTagReader = IReaderAsync<ContactTagIdDTO, ContactTagDomain>;
using ContactTagValidator = IBusinessValidationService<ContactTagIdDTO>;
using TagDeleter = IDeleterAsync<ulong, TagDomain>;
using TagReader = IReaderAsync<ulong, TagDomain>;

/// <summary>
/// Caso de uso para eliminar una etiqueta de un contacto.
/// </summary>
public sealed class DeleteContactTagUseCase(
    ContactTagDeleter deleter,
    ContactTagReader reader,
    ContactQuerier contactQuerier,
    TagDeleter tagDeleter,
    TagReader tagReader,
    ContactTagValidator? validator = null
) : DeleteUseCase<ContactTagIdDTO, ContactTagDomain>(deleter, reader, validator)
{
    private readonly ContactQuerier _contactQuerier = contactQuerier;
    private readonly TagDeleter _tagDeleter = tagDeleter;
    private readonly TagReader _tagReader = tagReader;

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ContactTagIdDTO> value,
        ContactTagDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.Data.AgendaOwnerId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new InvalidOperationException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <inheritdoc/>
    protected override async Task ExtraTaskAsync(
        UserActionDTO<ContactTagIdDTO> deleteDTO,
        ContactTagDomain deletedEntity
    )
    {
        var tag =
            await _tagReader.GetAsync(deleteDTO.Data.TagId)
            ?? throw new Exception("Etiqueta no deberia ser null");

        var otherUsesTag = await _contactQuerier.AnyAsync(new() { Tags = [tag.Text] });

        if (!otherUsesTag)
            await _tagDeleter.DeleteAsync(deleteDTO.Data.TagId);
    }
}
