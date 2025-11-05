using Application.DTOs.ContactTags;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public sealed class ContactTagEFMapper
    : IMapper<ContactTag, ContactTagDomain>,
        IMapper<ContactTagDTO, ContactTag>
{
    public ContactTagDomain Map(ContactTag input) =>
        new()
        {
            Id = new()
            {
                Tag = input.TagText,
                AgendaOwnerId = input.AgendaOwnerId,
                UserId = input.UserId,
            },
            CreatedAt = input.CreatedAt,
        };

    public ContactTag Map(ContactTagDTO input) =>
        new()
        {
            TagText = input.Id.Tag,
            AgendaOwnerId = input.Id.AgendaOwnerId,
            UserId = input.Id.UserId,
        };
}
