using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Contacts;

public class ContactMapper : IMapper<AgendaContact, ContactDomain>
{
    public ContactDomain Map(AgendaContact input) =>
        new()
        {
            Id = new() { AgendaOwnerId = input.AgendaOwnerId, UserId = input.UserId },
            Alias = input.Alias,
            Notes = input.Notes,
            CreatedAt = input.CreatedAt,
            ModifiedAt = input.ModifiedAt,
        };
}
