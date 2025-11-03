using Application.DTOs.ContactTags;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public sealed class ContactTagEFMapper
    : IMapper<ContactTag, ContactTagDomain>,
        IMapper<NewContactTagDTO, ContactTag>
{
    public ContactTagDomain Map(ContactTag input) =>
        new()
        {
            Id = new()
            {
                Tag = input.TagText,
                AgendaOwnerId = input.AgendaOwnerId,
                ContactId = input.ContactId,
            },
            CreatedAt = input.CreatedAt,
        };

    public ContactTag Map(NewContactTagDTO input) =>
        new()
        {
            TagText = input.Tag,
            AgendaOwnerId = input.AgendaOwnerId,
            ContactId = input.ContactId,
        };
}
