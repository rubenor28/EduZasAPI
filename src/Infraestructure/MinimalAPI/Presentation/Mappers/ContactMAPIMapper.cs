using Application.DTOs.Contacts;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Contacts;

namespace MinimalAPI.Presentation.Mappers;

public static class ContactMAPIMapper
{
    public static PublicContactMAPI FromDomain(this ContactDomain source) =>
        new()
        {
            Id = source.Id,
            Alias = source.Alias,
            Notes = source.Notes.ToNullable(),
            AgendaOwnerId = source.AgendaOwnerId,
            ContactId = source.ContactId,
        };

    public static ContactCriteriaDTO ToDomain(this ContactCriteriaMAPI source) =>
        new()
        {
            Page = source.Page,
            Alias = source.Alias.ToOptional(),
            ContactId = source.ContactId.ToOptional(),
            AgendaOwnerId = source.AgendaOwnerId.ToOptional(),
        };

    public static ContactUpdateDTO ToDomain(this ContactUpdateMAPI source) =>
        new()
        {
            Id = source.Id,
            AgendaOwnerId = source.AgendaOwnerId,
            ContactId = source.ContactId,
            Alias = source.Alias,
            Notes = source.Notes.ToOptional(),
        };
}
