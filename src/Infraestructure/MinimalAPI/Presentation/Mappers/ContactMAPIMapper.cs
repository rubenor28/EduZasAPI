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
            AgendaOwnerId = source.Id.AgendaOwnerId,
            UserId = source.Id.UserId,
            Alias = source.Alias,
            Notes = source.Notes.ToNullable(),
        };

    public static ContactCriteriaDTO ToDomain(this ContactCriteriaMAPI source) =>
        new()
        {
            Page = source.Page,
            Alias = source.Alias.ToOptional(),
            UserId = source.UserId.ToOptional(),
            AgendaOwnerId = source.AgendaOwnerId.ToOptional(),
        };

    public static ContactUpdateDTO ToDomain(this ContactUpdateMAPI source) =>
        new()
        {
            Id = new() { AgendaOwnerId = source.AgendaOwnerId, UserId = source.UserId },
            Alias = source.Alias,
            Notes = source.Notes.ToOptional(),
        };
}
