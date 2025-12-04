using Application.DTOs.Classes;
using Application.DTOs.Common;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Classes;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Presentation.Mappers;

public sealed class ClassCriteriaMAPIMapper(
    IBidirectionalResultMapper<StringQueryMAPI?, StringQueryDTO?, Unit> strqMapper
)
    : IBidirectionalResultMapper<ClassCriteriaMAPI, ClassCriteriaDTO, IEnumerable<FieldErrorDTO>>,
        IMapper<ClassCriteriaDTO, ClassCriteriaMAPI>
{
    private readonly IBidirectionalResultMapper<
        StringQueryMAPI?,
        StringQueryDTO?,
        Unit
    > _strqMapper = strqMapper;

    public Result<ClassCriteriaDTO, IEnumerable<FieldErrorDTO>> Map(ClassCriteriaMAPI input)
    {
        var errors = new List<FieldErrorDTO>();
        var className = _strqMapper.Map(input.ClassName);
        var subject = _strqMapper.Map(input.Subject);
        var section = _strqMapper.Map(input.Section);

        className.IfErr(_ => errors.Add(new() { Field = "className" }));
        subject.IfErr(_ => errors.Add(new() { Field = "subject" }));
        section.IfErr(_ => errors.Add(new() { Field = "section" }));

        if (errors.Count > 0)
            return errors;

        return new ClassCriteriaDTO
        {
            Page = input.Page,
            Active = input.Active,
            ClassName = className.Unwrap(),
            Subject = subject.Unwrap(),
            Section = section.Unwrap(),
            WithProfessor = input.WithProfessor,
            WithStudent = input.WithStudent,
        };
    }

    public ClassCriteriaMAPI Map(ClassCriteriaDTO input) => MapFrom(input);

    public ClassCriteriaMAPI MapFrom(ClassCriteriaDTO input) =>
        new()
        {
            Page = input.Page,
            Active = input.Active,
            ClassName = _strqMapper.MapFrom(input.ClassName),
            Section = _strqMapper.MapFrom(input.Section),
            Subject = _strqMapper.MapFrom(input.Subject),
            WithProfessor = input.WithProfessor,
            WithStudent = input.WithStudent,
        };
}

public sealed class ClassSearchMAPIMapper(
    IMapper<ClassCriteriaDTO, ClassCriteriaMAPI> criteriaMapper
)
    : IMapper<
        PaginatedQuery<ClassDomain, ClassCriteriaDTO>,
        PaginatedQuery<ClassDomain, ClassCriteriaMAPI>
    >
{
    public PaginatedQuery<ClassDomain, ClassCriteriaMAPI> Map(
        PaginatedQuery<ClassDomain, ClassCriteriaDTO> input
    ) =>
        new()
        {
            Page = input.Page,
            TotalPages = input.TotalPages,
            Results = input.Results,
            Criteria = criteriaMapper.Map(input.Criteria),
        };
}
