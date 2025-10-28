using Application.DTOs.ContactTag;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public sealed class ContactTagEFMapper
    : IMapper<TagsPerUser, ContactTagDomain>,
        IMapper<NewContactTagDTO, TagsPerUser>
{
    public ContactTagDomain Map(TagsPerUser input) =>
        new()
        {
            Id = new() { TagId = input.TagId, AgendaContactId = input.AgendaContactId },
            CreatedAt = input.CreatedAt,
        };

    public TagsPerUser Map(NewContactTagDTO input) =>
        new() { TagId = input.TagId, AgendaContactId = input.AgendaContactId };
}
