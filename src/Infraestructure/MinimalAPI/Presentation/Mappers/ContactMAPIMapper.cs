using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Contacts;

namespace MinimalAPI.Presentation.Mappers;

public sealed class ContactCriteriaMAPIMapper(
    IBidirectionalResultMapper<StringQueryMAPI?, StringQueryDTO?, Unit> strqMapper
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
        StringQueryDTO?,
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
            UserId = input.UserId,
            Tags = input.Tags,
            AgendaOwnerId = input.AgendaOwnerId,
        };
    }

    public ContactCriteriaMAPI Map(ContactCriteriaDTO input) => MapFrom(input);

    public ContactCriteriaMAPI MapFrom(ContactCriteriaDTO input) =>
        new()
        {
            Page = input.Page,
            Alias = _strqMapper.MapFrom(input.Alias),
            UserId = input.UserId,
            AgendaOwnerId = input.AgendaOwnerId,
        };
}

public sealed class ContactSearchMAPIMapper(
    IMapper<ContactCriteriaDTO, ContactCriteriaMAPI> cMapper
)
    : IMapper<
        PaginatedQuery<ContactDomain, ContactCriteriaDTO>,
        PaginatedQuery<ContactDomain, ContactCriteriaMAPI>
    >
{
    public PaginatedQuery<ContactDomain, ContactCriteriaMAPI> Map(
        PaginatedQuery<ContactDomain, ContactCriteriaDTO> input
    ) =>
        new()
        {
            Page = input.Page,
            Criteria = cMapper.Map(input.Criteria),
            TotalPages = input.TotalPages,
            Results = input.Results,
        };
}
