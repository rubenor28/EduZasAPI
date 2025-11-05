using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.ContactTags;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ContactTags;

using ContactQuerier = IQuerierAsync<ContactDomain, ContactCriteriaDTO>;
using ContactTagDeleter = IDeleterAsync<ContactTagIdDTO, ContactTagDomain>;
using ContactTagReader = IReaderAsync<ContactTagIdDTO, ContactTagDomain>;
using ContactTagValidator = IBusinessValidationService<ContactTagDTO>;
using TagDeleter = IDeleterAsync<string, TagDomain>;

public sealed class DeleteContactTagUseCase(
    ContactTagDeleter deleter,
    ContactTagReader reader,
    ContactQuerier contactQuerier,
    TagDeleter tagDeleter,
    ContactTagValidator? validator = null
) : DeleteUseCase<ContactTagIdDTO, ContactTagDTO, ContactTagDomain>(deleter, reader, validator)
{
    private readonly ContactQuerier _contactQuerier = contactQuerier;
    private readonly TagDeleter _tagDeleter = tagDeleter;

    protected override async Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        ContactTagDTO value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.Id.AgendaOwnerId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new InvalidOperationException(),
        };

        if (!authorized)
            return UseCaseError.Unauthorized();

        var recordSearch = await _reader.GetAsync(value.Id);
        if (recordSearch.IsNone)
            return UseCaseError.NotFound();

        return Unit.Value;
    }

    protected override async Task ExtraTaskAsync(
        ContactTagDTO deleteDTO,
        ContactTagDomain deletedEntity
    )
    {
        var tagSearch = new[] { deleteDTO.Id.Tag };
        var otherContactsUseTag = (
            await _contactQuerier.GetByAsync(new() { Tags = tagSearch })
        ).Results.Any();

        if (!otherContactsUseTag)
            await _tagDeleter.DeleteAsync(deleteDTO.Id.Tag);
    }
}
