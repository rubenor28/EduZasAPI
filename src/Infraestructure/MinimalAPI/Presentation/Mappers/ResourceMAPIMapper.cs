using Application.DTOs.Common;
using Application.DTOs.Resources;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Resources;

namespace MinimalAPI.Presentation.Mappers;

public sealed class ResourceMAPIMapper(
    IMapper<StringQueryMAPI?, Result<Optional<StringQueryDTO>, Unit>> strqToDomainMapper,
    IMapper<Optional<StringQueryDTO>, StringQueryMAPI?> strqFromDomainMapper
)
    : IMapper<NewResourceMAPI, Executor, NewResourceDTO>,
        IMapper<ResourceDomain, PublicResourceMAPI>,
        IMapper<ulong, Executor, DeleteResourceDTO>,
        IMapper<ResourceCriteriaDTO, ResourceCriteriaMAPI>,
        IMapper<Optional<ResourceCriteriaDTO>, ResourceCriteriaMAPI?>,
        IMapper<ResourceUpdateMAPI, Executor, ResourceUpdateDTO>,
        IMapper<ResourceCriteriaMAPI, Result<ResourceCriteriaDTO, IEnumerable<FieldErrorDTO>>>,
        IMapper<PaginatedQuery<ResourceDomain, ResourceCriteriaDTO>, PaginatedQuery<PublicResourceMAPI, ResourceCriteriaMAPI>>
{
    private readonly IMapper<
        StringQueryMAPI?,
        Result<Optional<StringQueryDTO>, Unit>
    > _strqToDomainMapper = strqToDomainMapper;

    private readonly IMapper<Optional<StringQueryDTO>, StringQueryMAPI?> _strqFromDomainMapper =
        strqFromDomainMapper;

    NewResourceDTO IMapper<NewResourceMAPI, Executor, NewResourceDTO>.Map(
        NewResourceMAPI value,
        Executor ex
    ) =>
        new()
        {
            Title = value.Title,
            Content = value.Content,
            ProfessorId = value.ProfessorId,
            Executor = ex,
        };

    public PublicResourceMAPI Map(ResourceDomain input) =>
        new()
        {
            Id = input.Id,
            Active = input.Active,
            Title = input.Title,
            Content = input.Content,
            ProfessorId = input.ProfessorId,
        };

    Result<ResourceCriteriaDTO, IEnumerable<FieldErrorDTO>> IMapper<
        ResourceCriteriaMAPI,
        Result<ResourceCriteriaDTO, IEnumerable<FieldErrorDTO>>
    >.Map(ResourceCriteriaMAPI input)
    {
        var errors = new List<FieldErrorDTO>();
        var titleValidation = _strqToDomainMapper.Map(input.Title);
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

    PaginatedQuery<PublicResourceMAPI, ResourceCriteriaMAPI> IMapper<
        PaginatedQuery<ResourceDomain, ResourceCriteriaDTO>,
        PaginatedQuery<PublicResourceMAPI, ResourceCriteriaMAPI>
    >.Map(PaginatedQuery<ResourceDomain, ResourceCriteriaDTO> input) =>
        new()
        {
            Page = input.Page,
            TotalPages = input.TotalPages,
            Criteria = Map(input.Criteria),
            Results = input.Results.Select(Map),
        };

    DeleteResourceDTO IMapper<ulong, Executor, DeleteResourceDTO>.Map(
        ulong resourceId,
        Executor ex
    ) => new() { Id = resourceId, Executor = ex };

    ResourceUpdateDTO IMapper<ResourceUpdateMAPI, Executor, ResourceUpdateDTO>.Map(
        ResourceUpdateMAPI value,
        Executor ex
    ) =>
        new()
        {
            Id = value.Id,
            Active = value.Active,
            Title = value.Title,
            Content = value.Content,
            Executor = ex,
        };

    public ResourceCriteriaMAPI Map(ResourceCriteriaDTO input) =>
        new()
        {
            ClassId = input.ClassId.ToNullable(),
            ProfessorId = input.ProfessorId.ToNullable(),
            Page = input.Page,
            Active = input.Active.ToNullable(),
            Title = _strqFromDomainMapper.Map(input.Title),
        };

    public ResourceCriteriaMAPI? Map(Optional<ResourceCriteriaDTO> input) =>
        input.IsSome ? Map(input.Unwrap()) : null;
}
