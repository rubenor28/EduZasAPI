using Application.DTOs.Common;
using Application.DTOs.Tags;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Tags;

namespace MinimalAPI.Presentation.Mappers;

public sealed class TagCriteriaMAPIMapper(
    IBidirectionalResultMapper<StringQueryMAPI?, StringQueryDTO?, Unit> strqMapper
)
    : IBidirectionalResultMapper<TagCriteriaMAPI, TagCriteriaDTO, IEnumerable<FieldErrorDTO>>,
        IMapper<TagCriteriaDTO, TagCriteriaMAPI>
{
    private readonly IBidirectionalResultMapper<StringQueryMAPI?, StringQueryDTO?, Unit> _strqMapper =
        strqMapper;

    public Result<TagCriteriaDTO, IEnumerable<FieldErrorDTO>> Map(TagCriteriaMAPI input)
    {
        List<FieldErrorDTO> errs = [];
        var textValidation = _strqMapper.Map(input.Text);
        textValidation.IfErr(_ => errs.Add(new() { Field = "text", Message = "Formato invalido" }));

        if (errs.Count > 0)
            return errs;

        return new TagCriteriaDTO
        {
            Text = textValidation.Unwrap(),
            ContactId = input.ContactId,
            AgendaOwnerId = input.AgendaOwnerId,
            Page = input.Page,
        };
    }

    public TagCriteriaMAPI Map(TagCriteriaDTO input) => MapFrom(input);

    public TagCriteriaMAPI MapFrom(TagCriteriaDTO input) =>
        new()
        {
            Page = input.Page,
            ContactId = input.ContactId,
            AgendaOwnerId = input.AgendaOwnerId,
            Text = _strqMapper.MapFrom(input.Text),
        };
}

public sealed class TagSearchMAPIMapper(IMapper<TagCriteriaDTO, TagCriteriaMAPI> cMapper)
    : IMapper<PaginatedQuery<TagDomain, TagCriteriaDTO>, PaginatedQuery<string, TagCriteriaMAPI>>
{
    public PaginatedQuery<string, TagCriteriaMAPI> Map(
        PaginatedQuery<TagDomain, TagCriteriaDTO> input
    ) =>
        new()
        {
            Page = input.Page,
            Criteria = cMapper.Map(input.Criteria),
            Results = input.Results.Select(t => t.Text),
            TotalPages = input.TotalPages,
        };
}
