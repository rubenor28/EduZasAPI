using Application.DTOs.Common;
using Application.DTOs.Tags;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Tags;

namespace MinimalAPI.Presentation.Mappers;

using StringQueryFromDomainMapper = IMapper<Optional<StringQueryDTO>, StringQueryMAPI?>;
using StringQueryToDomainMapper = IMapper<StringQueryMAPI?, Result<Optional<StringQueryDTO>, Unit>>;

public class TagMAPIMapper(
    StringQueryToDomainMapper strqToDomainMapper,
    StringQueryFromDomainMapper strqFromDomainMapper
)
    : IMapper<TagCriteriaMAPI, Result<TagCriteriaDTO, IEnumerable<FieldErrorDTO>>>,
        IMapper<TagCriteriaDTO, TagCriteriaMAPI>,
        IMapper<TagDomain, PublicTagMAPI>,
        IMapper<PaginatedQuery<TagDomain, TagCriteriaDTO>, PaginatedQuery<string, TagCriteriaMAPI>>
{
    private readonly StringQueryToDomainMapper _strqToDomainMapper = strqToDomainMapper;
    private readonly StringQueryFromDomainMapper _strqFromDomainMapper = strqFromDomainMapper;

    public Result<TagCriteriaDTO, IEnumerable<FieldErrorDTO>> Map(TagCriteriaMAPI source)
    {
        List<FieldErrorDTO> errs = [];
        var textValidation = _strqToDomainMapper.Map(source.Text);
        textValidation.IfErr(_ => errs.Add(new() { Field = "text", Message = "Formato invalido" }));

        if (errs.Count > 0)
            return errs;

        return new TagCriteriaDTO
        {
            Text = textValidation.Unwrap(),
            ContactId = source.ContactId.ToOptional(),
            AgendaOwnerId = source.AgendaOwnerId.ToOptional(),
            Page = source.Page,
        };
    }

    public TagCriteriaMAPI Map(TagCriteriaDTO source) =>
        new()
        {
            Page = source.Page,
            ContactId = source.ContactId.ToNullable(),
            AgendaOwnerId = source.AgendaOwnerId.ToNullable(),
            Text = _strqFromDomainMapper.Map(source.Text),
        };

    public PublicTagMAPI Map(TagDomain source) => new() { Text = source.Text };

    public PaginatedQuery<string, TagCriteriaMAPI> Map(
        PaginatedQuery<TagDomain, TagCriteriaDTO> input
    ) =>
        new()
        {
            Page = input.Page,
            Criteria = Map(input.Criteria),
            Results = input.Results.Select(t => t.Text),
            TotalPages = input.TotalPages,
        };
}
