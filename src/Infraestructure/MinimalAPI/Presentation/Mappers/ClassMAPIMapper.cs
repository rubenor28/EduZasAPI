using Application.DTOs.Classes;
using Application.DTOs.Common;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Classes;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Presentation.Mappers;

public sealed class  WithProfessorMAPIMapper
    : IBidirectionalMapper<WithProfessorMAPI?, Optional<WithProfessorDTO>>,
        IMapper<Optional<WithProfessorDTO>, WithProfessorMAPI?>
{
    public Optional<WithProfessorDTO> Map(WithProfessorMAPI? input) =>
        input is null
            ? Optional<WithProfessorDTO>.None()
            : new WithProfessorDTO { Id = input.Id, IsOwner = input.IsOwner.ToOptional() };

    public WithProfessorMAPI? Map(Optional<WithProfessorDTO> input) => MapFrom(input);

    public WithProfessorMAPI? MapFrom(Optional<WithProfessorDTO> input) =>
        input.Match<WithProfessorMAPI?>(
            value => new WithProfessorMAPI { Id = value.Id, IsOwner = value.IsOwner.ToNullable() },
            () => null
        );
}

public sealed class  WithStudentMAPIMapper
    : IBidirectionalMapper<WithStudentMAPI?, Optional<WithStudentDTO>>,
        IMapper<Optional<WithStudentDTO>, WithStudentMAPI?>
{
    public Optional<WithStudentDTO> Map(WithStudentMAPI? input) =>
        input is null
            ? Optional<WithStudentDTO>.None()
            : new WithStudentDTO { Id = input.Id, Hidden = input.Hidden.ToOptional() };

    public WithStudentMAPI? Map(Optional<WithStudentDTO> input) => MapFrom(input);

    public WithStudentMAPI? MapFrom(Optional<WithStudentDTO> input) =>
        input.Match<WithStudentMAPI?>(
            value => new WithStudentMAPI { Id = value.Id, Hidden = value.Hidden.ToNullable() },
            () => null
        );
}

public sealed class  ClassCriteriaMAPIMapper(
    IBidirectionalResultMapper<StringQueryMAPI?, Optional<StringQueryDTO>, Unit> strqMapper,
    IBidirectionalMapper<WithProfessorMAPI?, Optional<WithProfessorDTO>> wpMapper,
    IBidirectionalMapper<WithStudentMAPI?, Optional<WithStudentDTO>> wsMapper
)
    : IBidirectionalResultMapper<ClassCriteriaMAPI, ClassCriteriaDTO, IEnumerable<FieldErrorDTO>>,
        IMapper<ClassCriteriaDTO, ClassCriteriaMAPI>
{
    private readonly IBidirectionalResultMapper<
        StringQueryMAPI?,
        Optional<StringQueryDTO>,
        Unit
    > _strqMapper = strqMapper;

    private readonly IBidirectionalMapper<
        WithProfessorMAPI?,
        Optional<WithProfessorDTO>
    > _wpMapper = wpMapper;
    private readonly IBidirectionalMapper<WithStudentMAPI?, Optional<WithStudentDTO>> _wsMapper =
        wsMapper;

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
            Active = input.Active.ToOptional(),
            ClassName = className.Unwrap(),
            Subject = subject.Unwrap(),
            Section = section.Unwrap(),
            WithProfessor = _wpMapper.Map(input.WithProfessor),
            WithStudent = _wsMapper.Map(input.WithStudent),
        };
    }

    public ClassCriteriaMAPI Map(ClassCriteriaDTO input) => MapFrom(input);

    public ClassCriteriaMAPI MapFrom(ClassCriteriaDTO input) =>
        new()
        {
            Page = input.Page,
            Active = input.Active.ToNullable(),
            ClassName = _strqMapper.MapFrom(input.ClassName),
            Section = _strqMapper.MapFrom(input.Section),
            Subject = _strqMapper.MapFrom(input.Subject),
            WithProfessor = _wpMapper.MapFrom(input.WithProfessor),
            WithStudent = _wsMapper.MapFrom(input.WithStudent),
        };
}

public sealed class  ClassMAPIMapper : IMapper<ClassDomain, PublicClassMAPI>
{
    /// <summary>
    /// Mapea una entidad de dominio <see cref="ClassDomain"/> a la representación de la API.
    /// </summary>
    public PublicClassMAPI Map(ClassDomain input) =>
        new()
        {
            Id = input.Id,
            Active = input.Active,
            ClassName = input.ClassName,
            Color = input.Color,
            Section = input.Section.ToNullable(),
            Subject = input.Subject.ToNullable(),
        };
}

public sealed class  NewClassMAPIMapper : IMapper<NewClassMAPI, Executor, NewClassDTO>
{
    public NewClassDTO Map(NewClassMAPI input, Executor ex) =>
        new()
        {
            Id = "",
            ClassName = input.ClassName,
            Subject = input.Subject.ToOptional(),
            Section = input.Section.ToOptional(),
            Color = input.Color,
            OwnerId = input.OwnerId,
            Professors = input.Professors,
            Executor = ex,
        };
}

public sealed class  DeleteClassMAPIMapper : IMapper<string, Executor, DeleteClassDTO>
{
    /// <summary>
    /// Mapea la representación de la API a una entidad de dominio.
    /// </summary>
    public DeleteClassDTO Map(string id, Executor ex) => new() { Id = id, Executor = ex };
}

public sealed class  ClassUpdateMAPIMapper : IMapper<ClassUpdateMAPI, Executor, ClassUpdateDTO>
{
    /// <summary>
    /// Mapea la representación de la API a una entidad de dominio.
    /// </summary>
    public ClassUpdateDTO Map(ClassUpdateMAPI data, Executor ex) =>
        new()
        {
            Id = data.Id,
            Active = data.Active,
            ClassName = data.ClassName,
            Section = data.Section.ToOptional(),
            Subject = data.Subject.ToOptional(),
            Color = data.Color,
            Executor = ex,
        };
}

public sealed class  ClassSearchMAPIMapper(
    IMapper<ClassCriteriaDTO, ClassCriteriaMAPI> criteriaMapper,
    IMapper<ClassDomain, PublicClassMAPI> mapper
)
    : IMapper<
        PaginatedQuery<ClassDomain, ClassCriteriaDTO>,
        PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI>
    >
{
    private readonly IMapper<ClassCriteriaDTO, ClassCriteriaMAPI> _cMapper = criteriaMapper;
    private readonly IMapper<ClassDomain, PublicClassMAPI> _mapper = mapper;

    /// <summary>
    /// Mapea una entidad de dominio a la representación de la API.
    /// </summary>
    public PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI> Map(
        PaginatedQuery<ClassDomain, ClassCriteriaDTO> search
    ) =>
        new()
        {
            Page = search.Page,
            Criteria = _cMapper.Map(search.Criteria),
            TotalPages = search.TotalPages,
            Results = search.Results.Select(_mapper.Map),
        };
}
