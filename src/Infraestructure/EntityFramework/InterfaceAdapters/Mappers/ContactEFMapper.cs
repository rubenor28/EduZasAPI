using Application.DTOs.Contacts;
using Domain.Entities;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public sealed class ContactEFMapper
    : IMapper<AgendaContact, ContactDomain>,
        IMapper<NewContactDTO, AgendaContact>,
        IUpdateMapper<ContactUpdateDTO, AgendaContact>
{
    public ContactDomain Map(AgendaContact input) =>
        new()
        {
            Id = input.AgendaContactId,
            AgendaOwnerId = input.AgendaOwnerId,
            Alias = input.Alias,
            ContactId = input.ContactId,
            Notes = input.Notes.ToOptional(),
            CreatedAt = input.CreatedAt,
            ModifiedAt = input.ModifiedAt,
        };

    public AgendaContact Map(NewContactDTO input) =>
        new()
        {
            Alias = input.Alias,
            Notes = input.Notes.ToNullable(),
            ContactId = input.ContactId,
            AgendaOwnerId = input.AgendaOwnerId,
        };

    public void Map(ContactUpdateDTO source, AgendaContact destination)
    {
        destination.AgendaContactId = source.Id;
        destination.Alias = source.Alias;
        destination.Notes = source.Notes.ToNullable();
        destination.AgendaOwnerId = source.AgendaOwnerId;
        destination.ContactId = source.ContactId;
    }
}
