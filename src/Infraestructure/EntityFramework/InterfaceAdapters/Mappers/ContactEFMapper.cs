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
            Id = new() { AgendaOwnerId = input.AgendaOwnerId, UserId = input.UserId },
            Alias = input.Alias,
            Notes = input.Notes.ToOptional(),
            CreatedAt = input.CreatedAt,
            ModifiedAt = input.ModifiedAt,
        };

    public AgendaContact Map(NewContactDTO input) =>
        new()
        {
            Alias = input.Alias,
            Notes = input.Notes.ToNullable(),
            UserId = input.UserId,
            AgendaOwnerId = input.AgendaOwnerId,
        };

    public void Map(ContactUpdateDTO source, AgendaContact destination)
    {
        destination.Alias = source.Alias;
        destination.Notes = source.Notes.ToNullable();
        destination.AgendaOwnerId = source.AgendaOwnerId;
        destination.UserId = source.UserId;
    }
}
