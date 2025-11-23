using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.ClassProfessors;

namespace MinimalAPI.Presentation.Mappers;

public sealed class ClassProfessorMAPIMapper : IMapper<ClassProfessorDomain, ClassProfessorMAPI>
{
    public ClassProfessorMAPI Map(ClassProfessorDomain input) =>
        new()
        {
            ClassId = input.Id.ClassId,
            UserId = input.Id.UserId,
            IsOwner = input.IsOwner,
        };
}

public sealed class NewClassProfessorMAPIMapper
    : IMapper<ClassProfessorMAPI, Executor, NewClassProfessorDTO>
{
    public NewClassProfessorDTO Map(ClassProfessorMAPI input, Executor ex) =>
        new()
        {
            ClassId = input.ClassId,
            UserId = input.UserId,
            IsOwner = input.IsOwner,
            Executor = ex,
        };
}

public sealed class ClassProfessorUpdateMAPIMapper
    : IMapper<ClassProfessorMAPI, Executor, ClassProfessorUpdateDTO>
{
    public ClassProfessorUpdateDTO Map(ClassProfessorMAPI value, Executor ex) =>
        new()
        {
            ClassId = value.ClassId,
            UserId = value.UserId,
            IsOwner = value.IsOwner,
            Executor = ex,
        };
}

public sealed class ClassProfessorSearchMAPIMapper(
    IMapper<ClassProfessorDomain, ClassProfessorMAPI> mapper,
    IMapper<ClassProfessorCriteriaDTO, ClassProfessorCriteriaMAPI> cMapper
)
    : IMapper<
        PaginatedQuery<ClassProfessorDomain, ClassProfessorCriteriaDTO>,
        PaginatedQuery<ClassProfessorMAPI, ClassProfessorCriteriaMAPI>
    >
{
    public PaginatedQuery<ClassProfessorMAPI, ClassProfessorCriteriaMAPI> Map(
        PaginatedQuery<ClassProfessorDomain, ClassProfessorCriteriaDTO> input
    ) =>
        new()
        {
            Page = input.Page,
            TotalPages = input.TotalPages,
            Criteria = cMapper.Map(input.Criteria),
            Results = input.Results.Select(mapper.Map),
        };
}

public sealed class ClassProfessorsCriteriaMAPIMapper
    : IBidirectionalMapper<ClassProfessorCriteriaMAPI, ClassProfessorCriteriaDTO>,
        IMapper<ClassProfessorCriteriaDTO, ClassProfessorCriteriaMAPI>
{
    public ClassProfessorCriteriaMAPI Map(ClassProfessorCriteriaDTO input) => MapFrom(input);

    public ClassProfessorCriteriaMAPI MapFrom(ClassProfessorCriteriaDTO input) =>
        new()
        {
            Page = input.Page,
            ClassId = input.ClassId.ToNullable(),
            IsOwner = input.IsOwner.ToNullable(),
            UserId = input.UserId.ToNullable(),
        };

    public ClassProfessorCriteriaDTO Map(ClassProfessorCriteriaMAPI input) =>
        new()
        {
            Page = input.Page,
            ClassId = input.ClassId.ToOptional(),
            IsOwner = input.IsOwner.ToOptional(),
            UserId = input.UserId.ToOptional(),
        };
}
