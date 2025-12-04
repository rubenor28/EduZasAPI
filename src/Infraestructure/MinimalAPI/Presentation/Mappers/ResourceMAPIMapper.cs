using Application.DTOs.Common;
using Application.DTOs.Resources;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Resources;

namespace MinimalAPI.Presentation.Mappers;

public sealed class ResourceCriteriaMAPIMapper(
    IBidirectionalResultMapper<StringQueryMAPI?, StringQueryDTO?, Unit> strqMapper
)
    : IBidirectionalResultMapper<
        ResourceCriteriaMAPI,
        ResourceCriteriaDTO,
        IEnumerable<FieldErrorDTO>
    >,
        IMapper<ResourceCriteriaDTO, ResourceCriteriaMAPI>
{
    private readonly IBidirectionalResultMapper<
        StringQueryMAPI?,
        StringQueryDTO?,
        Unit
    > _strqMapper = strqMapper;

    public Result<ResourceCriteriaDTO, IEnumerable<FieldErrorDTO>> Map(ResourceCriteriaMAPI input)
    {
        var errors = new List<FieldErrorDTO>();
        var titleValidation = _strqMapper.Map(input.Title);
        titleValidation.IfErr(_ => errors.Add(new() { Field = "title" }));

        if (errors.Count > 0)
            return errors;

        return new ResourceCriteriaDTO
        {
            ClassId = input.ClassId,
            ProfessorId = input.ProfessorId,
            Title = titleValidation.Unwrap(),
            Active = input.Active,
            Page = input.Page,
        };
    }

    public ResourceCriteriaMAPI Map(ResourceCriteriaDTO input) => MapFrom(input);

    public ResourceCriteriaMAPI MapFrom(ResourceCriteriaDTO input) =>
        new()
        {
            ClassId = input.ClassId,
            ProfessorId = input.ProfessorId,
            Page = input.Page,
            Active = input.Active,
            Title = _strqMapper.MapFrom(input.Title),
        };
}

public sealed class ResourceSearchMAPIMapper(
    IMapper<ResourceCriteriaDTO, ResourceCriteriaMAPI> cMapper
)
    : IMapper<
        PaginatedQuery<ResourceSummary, ResourceCriteriaDTO>,
        PaginatedQuery<ResourceSummary, ResourceCriteriaMAPI>
    >
{
    public PaginatedQuery<ResourceSummary, ResourceCriteriaMAPI> Map(
        PaginatedQuery<ResourceSummary, ResourceCriteriaDTO> input
    ) =>
        new()
        {
            Page = input.Page,
            TotalPages = input.TotalPages,
            Criteria = cMapper.Map(input.Criteria),
            Results = input.Results,
        };
}
