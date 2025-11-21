using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Contacts;

namespace MinimalAPI.Presentation.Mappers;

public class ContactMAPIMapper(
    IMapper<Optional<StringQueryDTO>, StringQueryMAPI?> strqFromDomainMapper,
    IMapper<StringQueryMAPI?, Result<Optional<StringQueryDTO>, Unit>> strqToDomainMapper
)
    : IMapper<ContactDomain, PublicContactMAPI>,
        IMapper<ContactCriteriaDTO, ContactCriteriaMAPI>,
        IMapper<ContactCriteriaMAPI, Result<ContactCriteriaDTO, IEnumerable<FieldErrorDTO>>>,
        IMapper<ContactUpdateMAPI, ContactUpdateDTO>,
        IMapper<
            PaginatedQuery<ContactDomain, ContactCriteriaDTO>,
            PaginatedQuery<PublicContactMAPI, ContactCriteriaMAPI>
        >,
        IMapper<NewContactMAPI, Executor, NewContactDTO>,
        IMapper<ulong, ulong, Executor, DeleteContactDTO>
{
    private readonly IMapper<
        StringQueryMAPI?,
        Result<Optional<StringQueryDTO>, Unit>
    > _strqToDomainMapper = strqToDomainMapper;

    private readonly IMapper<Optional<StringQueryDTO>, StringQueryMAPI?> _strqFromDomainMapper =
        strqFromDomainMapper;

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
            Alias = _strqFromDomainMapper.Map(source.Alias),
            UserId = source.UserId.ToNullable(),
            AgendaOwnerId = source.AgendaOwnerId.ToNullable(),
        };

    public ContactUpdateDTO Map(ContactUpdateMAPI source) =>
        new()
        {
            AgendaOwnerId = source.AgendaOwnerId,
            UserId = source.UserId,
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

    Result<ContactCriteriaDTO, IEnumerable<FieldErrorDTO>> IMapper<
        ContactCriteriaMAPI,
        Result<ContactCriteriaDTO, IEnumerable<FieldErrorDTO>>
    >.Map(ContactCriteriaMAPI input)
    {
        List<FieldErrorDTO> errors = [];
        var alias = _strqToDomainMapper.Map(input.Alias);
        alias.IfErr(_ => errors.Add(new() { Field = "alias" }));

        if(errors.Count > 0)return errors;

        return new ContactCriteriaDTO
        {
            Alias = alias.Unwrap(),
            Page = input.Page,
            UserId = input.UserId.ToOptional(),
            Tags = input.Tags.ToOptional(),
            AgendaOwnerId = input.AgendaOwnerId.ToOptional(),
        };
    }
}
