using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Contacts;

namespace MinimalAPI.Presentation.Mappers;

public sealed class PublicContactMAPIMapper : IMapper<ContactDomain, PublicContactMAPI>
{
    public PublicContactMAPI Map(ContactDomain input) =>
        new()
        {
            AgendaOwnerId = input.Id.AgendaOwnerId,
            UserId = input.Id.UserId,
            Alias = input.Alias,
            Notes = input.Notes.ToNullable(),
        };
}

public sealed class ContactCriteriaMAPIMapper(
    IBidirectionalResultMapper<StringQueryMAPI?, Optional<StringQueryDTO>, Unit> strqMapper
)
    : IBidirectionalResultMapper<
        ContactCriteriaMAPI,
        ContactCriteriaDTO,
        IEnumerable<FieldErrorDTO>
    >,
        IMapper<ContactCriteriaDTO, ContactCriteriaMAPI>
{
    private readonly IBidirectionalResultMapper<
        StringQueryMAPI?,
        Optional<StringQueryDTO>,
        Unit
    > _strqMapper = strqMapper;

    public Result<ContactCriteriaDTO, IEnumerable<FieldErrorDTO>> Map(ContactCriteriaMAPI input)
    {
        List<FieldErrorDTO> errors = [];
        var alias = _strqMapper.Map(input.Alias);
        alias.IfErr(_ => errors.Add(new() { Field = "alias" }));

        if (errors.Count > 0)
            return errors;

        return new ContactCriteriaDTO
        {
            Alias = alias.Unwrap(),
            Page = input.Page,
            UserId = input.UserId.ToOptional(),
            Tags = input.Tags.ToOptional(),
            AgendaOwnerId = input.AgendaOwnerId.ToOptional(),
        };
    }

    public ContactCriteriaMAPI Map(ContactCriteriaDTO input) => MapFrom(input);

    public ContactCriteriaMAPI MapFrom(ContactCriteriaDTO input) =>
        new()
        {
            Page = input.Page,
            Alias = _strqMapper.MapFrom(input.Alias),
            UserId = input.UserId.ToNullable(),
            AgendaOwnerId = input.AgendaOwnerId.ToNullable(),
        };
}

public sealed class ContactUpdateMAPIMapper : IMapper<ContactUpdateMAPI, ContactUpdateDTO>
{
    public ContactUpdateDTO Map(ContactUpdateMAPI source) =>
        new()
        {
            AgendaOwnerId = source.AgendaOwnerId,
            UserId = source.UserId,
            Alias = source.Alias,
            Notes = source.Notes.ToOptional(),
        };
}

public sealed class ContactSearchMAPIMapper(
    IMapper<ContactDomain, PublicContactMAPI> mapper,
    IMapper<ContactCriteriaDTO, ContactCriteriaMAPI> cMapper
)
    : IMapper<
        PaginatedQuery<ContactDomain, ContactCriteriaDTO>,
        PaginatedQuery<PublicContactMAPI, ContactCriteriaMAPI>
    >
{
    public PaginatedQuery<PublicContactMAPI, ContactCriteriaMAPI> Map(
        PaginatedQuery<ContactDomain, ContactCriteriaDTO> input
    ) =>
        new()
        {
            Page = input.Page,
            Criteria = cMapper.Map(input.Criteria),
            TotalPages = input.TotalPages,
            Results = input.Results.Select(mapper.Map),
        };
}

public sealed class NewContactMAPIMapper : IMapper<NewContactMAPI, Executor, NewContactDTO>
{
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
}

public sealed class DeleteContactMAPIMapper : IMapper<ulong, ulong, Executor, DeleteContactDTO>
{
    public DeleteContactDTO Map(ulong agendaOwnerId, ulong contactId, Executor ex) =>
        new()
        {
            Id = new() { AgendaOwnerId = agendaOwnerId, UserId = contactId },
            Executor = ex,
        };
}
