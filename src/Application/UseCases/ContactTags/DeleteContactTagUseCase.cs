using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ContactTags;

using ContactQuerier = IQuerierAsync<ContactDomain, ContactCriteriaDTO>;
using ContactTagDeleter = IDeleterAsync<ContactTagIdDTO, ContactTagDomain>;
using ContactTagReader = IReaderAsync<ContactTagIdDTO, ContactTagDomain>;
using ContactTagValidator = IBusinessValidationService<ContactTagIdDTO>;
using TagDeleter = IDeleterAsync<string, TagDomain>;

public sealed class DeleteContactTagUseCase(
    ContactTagDeleter deleter,
    ContactTagReader reader,
    ContactQuerier contactQuerier,
    TagDeleter tagDeleter,
    ContactTagValidator? validator = null
) : DeleteUseCase<ContactTagIdDTO, ContactTagDomain>(deleter, reader, validator)
{
    private readonly ContactQuerier _contactQuerier = contactQuerier;
    private readonly TagDeleter _tagDeleter = tagDeleter;

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

    protected override async Task ExtraTaskAsync(
        UserActionDTO<ContactTagIdDTO> deleteDTO,
        ContactTagDomain deletedEntity
    )
    {
        var tagSearch = new[] { deleteDTO.Data.Tag };
        var otherContactsUseTag = (
            await _contactQuerier.GetByAsync(new() { Tags = tagSearch })
        ).Results.Any();

        if (!otherContactsUseTag)
            await _tagDeleter.DeleteAsync(deleteDTO.Data.Tag);
    }
}
