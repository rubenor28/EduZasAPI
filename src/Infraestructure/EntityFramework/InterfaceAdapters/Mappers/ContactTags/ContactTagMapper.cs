using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ContactTags;

public class ContactTagMapper : IMapper<ContactTag, ContactTagDomain>
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
}
