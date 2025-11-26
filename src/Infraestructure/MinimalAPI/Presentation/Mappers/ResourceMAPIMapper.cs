using Application.DTOs.Common;
using Application.DTOs.Resources;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Resources;

namespace MinimalAPI.Presentation.Mappers;

public sealed class ResourceCriteriaMAPIMapper(
    IBidirectionalResultMapper<StringQueryMAPI?, Optional<StringQueryDTO>, Unit> strqMapper
)
    : IBidirectionalResultMapper<
        ResourceCriteriaMAPI,
        ResourceCriteriaDTO,
        IEnumerable<FieldErrorDTO>
    >,
        IMapper<ResourceCriteriaDTO, ResourceCriteriaMAPI>
{
    private readonly IBidirectionalResultMapper<StringQueryMAPI?, Optional<StringQueryDTO>, Unit> _strqMapper =
        strqMapper;

    public Result<ResourceCriteriaDTO, IEnumerable<FieldErrorDTO>> Map(ResourceCriteriaMAPI input)
    {
        var errors = new List<FieldErrorDTO>();
        var titleValidation = _strqMapper.Map(input.Title);
        titleValidation.IfErr(_ => errors.Add(new() { Field = "title" }));

        if (errors.Count > 0)
            return errors;

        return new ResourceCriteriaDTO
        {
            ClassId = input.ClassId.ToOptional(),
            ProfessorId = input.ProfessorId.ToOptional(),
            Title = titleValidation.Unwrap(),
            Active = input.Active.ToOptional(),
            Page = input.Page,
        };
    }

    public ResourceCriteriaMAPI Map(ResourceCriteriaDTO input) => MapFrom(input);

    public ResourceCriteriaMAPI MapFrom(ResourceCriteriaDTO input) =>
        new()
        {
            ClassId = input.ClassId.ToNullable(),
            ProfessorId = input.ProfessorId.ToNullable(),
            Page = input.Page,
            Active = input.Active.ToNullable(),
            Title = _strqMapper.MapFrom(input.Title),
        };
}

public sealed class NewResourceMAPIMapper : IMapper<NewResourceMAPI, Executor, NewResourceDTO>
{
    public NewResourceDTO Map(NewResourceMAPI value, Executor ex) =>
        new()
        {
            Title = value.Title,
            Content = value.Content,
            ProfessorId = value.ProfessorId,
            Executor = ex,
        };
}

public sealed class PublicResourceMAPIMapper : IMapper<ResourceDomain, PublicResourceMAPI>
{
    public PublicResourceMAPI Map(ResourceDomain input) =>
        new()
        {
            Id = input.Id,
            Active = input.Active,
            Title = input.Title,
            Content = input.Content,
            ProfessorId = input.ProfessorId,
        };
}

public sealed class DeleteResourceMAPIMapper : IMapper<Guid, Executor, DeleteResourceDTO>
{
    public DeleteResourceDTO Map(Guid resourceId, Executor ex) =>
        new() { Id = resourceId, Executor = ex };
}

public sealed class ResourceUpdateMAPIMapper
    : IMapper<ResourceUpdateMAPI, Executor, ResourceUpdateDTO>
{
    public ResourceUpdateDTO Map(ResourceUpdateMAPI value, Executor ex) =>
        new()
        {
            Id = value.Id,
            Active = value.Active,
            Title = value.Title,
            Content = value.Content,
            Executor = ex,
        };
}

public sealed class ResourceSearchMAPIMapper(
    IMapper<ResourceDomain, PublicResourceMAPI> mapper,
    IMapper<ResourceCriteriaDTO, ResourceCriteriaMAPI> cMapper
)
    : IMapper<
        PaginatedQuery<ResourceDomain, ResourceCriteriaDTO>,
        PaginatedQuery<PublicResourceMAPI, ResourceCriteriaMAPI>
    >
{
    public PaginatedQuery<PublicResourceMAPI, ResourceCriteriaMAPI> Map(
        PaginatedQuery<ResourceDomain, ResourceCriteriaDTO> input
    ) =>
        new()
        {
            Page = input.Page,
            TotalPages = input.TotalPages,
            Criteria = cMapper.Map(input.Criteria),
            Results = input.Results.Select(mapper.Map),
        };
}
