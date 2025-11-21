using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Contacts;

namespace MinimalAPI.Presentation.Mappers;

public class ContactMAPIMapper
    : IMapper<ContactDomain, PublicContactMAPI>,
        IMapper<ContactCriteriaMAPI, ContactCriteriaDTO>,
        IMapper<ContactCriteriaDTO, ContactCriteriaMAPI>,
        IMapper<ContactUpdateMAPI, ContactUpdateDTO>,
        IMapper<
            PaginatedQuery<ContactDomain, ContactCriteriaDTO>,
            PaginatedQuery<PublicContactMAPI, ContactCriteriaMAPI>
        >,
        IMapper<NewContactMAPI, Executor, NewContactDTO>,
        IMapper<ulong, ulong, Executor, DeleteContactDTO>
{
    public PublicContactMAPI Map(ContactDomain source) =>
        new()
        {
            AgendaOwnerId = source.Id.AgendaOwnerId,
            UserId = source.Id.UserId,
            Alias = source.Alias,
            Notes = source.Notes.ToNullable(),
        };

    public ContactCriteriaMAPI Map(ContactCriteriaDTO source) =>
        new()
        {
            Page = source.Page,
            Alias = source.Alias.ToNullable(),
            UserId = source.UserId.ToNullable(),
            AgendaOwnerId = source.AgendaOwnerId.ToNullable(),
        };

    public ContactCriteriaDTO Map(ContactCriteriaMAPI source) =>
        new()
        {
            Page = source.Page,
            Alias = source.Alias.ToOptional(),
            UserId = source.UserId.ToOptional(),
            AgendaOwnerId = source.AgendaOwnerId.ToOptional(),
        };

    public ContactUpdateDTO Map(ContactUpdateMAPI source) =>
        new()
        {
             AgendaOwnerId = source.AgendaOwnerId, UserId = source.UserId ,
            Alias = source.Alias,
            Notes = source.Notes.ToOptional(),
        };

    public PaginatedQuery<PublicContactMAPI, ContactCriteriaMAPI> Map(
        PaginatedQuery<ContactDomain, ContactCriteriaDTO> input
    ) =>
        new()
        {
            Page = input.Page,
            Criteria = Map(input.Criteria),
            TotalPages = input.TotalPages,
            Results = input.Results.Select(Map),
        };

    public NewContactDTO Map(NewContactMAPI input, Executor ex) =>
        new()
        {
            UserId = input.UserId,
            Tags = input.Tags.ToOptional(),
            Notes = input.Notes.ToOptional(),
            Alias = input.Alias,
            AgendaOwnerId = input.AgendaOwnerId,
            Executor = ex,
        };

    public DeleteContactDTO Map(ulong agendaOwnerId, ulong contactId, Executor ex) =>
        new()
        {
            Id = new() { AgendaOwnerId = agendaOwnerId, UserId = contactId },
            Executor = ex,
        };
}
