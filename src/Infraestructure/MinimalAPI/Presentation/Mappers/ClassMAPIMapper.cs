using Application.DTOs.Classes;
using Application.DTOs.Common;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Classes;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Presentation.Mappers;

// Alias para mejorar la legibilidad de los tipos de mapeadores genéricos complejos.
using StringQueryFromDomainMapper = IMapper<Optional<StringQueryDTO>, StringQueryMAPI?>;
using StringQueryToDomainMapper = IMapper<StringQueryMAPI?, Result<Optional<StringQueryDTO>, Unit>>;

/// <summary>
/// Mapeador centralizado para la entidad 'Class' en la capa de la API.
/// </summary>
/// <remarks>
/// Esta clase se encarga de las transformaciones entre los DTOs de la API (Minimal API),
/// los DTOs de la capa de aplicación y las entidades de dominio para 'Class'.
/// Implementa directamente toda la lógica de mapeo, incluyendo la de sub-componentes
/// como <see cref="WithProfessorDTO"/> y <see cref="WithStudentDTO"/>.
/// </remarks>
public class ClassMAPIMapper(
    StringQueryToDomainMapper strqToDomainMapper,
    StringQueryFromDomainMapper strqFromDomainMapper
)
    : IMapper<ClassDomain, PublicClassMAPI>,
        IMapper<NewClassMAPI, Executor, NewClassDTO>,
        IMapper<ClassCriteriaMAPI, Result<ClassCriteriaDTO, IEnumerable<FieldErrorDTO>>>,
        IMapper<ClassCriteriaDTO, ClassCriteriaMAPI>,
        IMapper<WithProfessorDTO, WithProfessorMAPI>,
        IMapper<WithStudentDTO, WithStudentMAPI>,
        IMapper<Optional<WithStudentDTO>, WithStudentMAPI?>,
        IMapper<Optional<WithProfessorDTO>, WithProfessorMAPI?>,
        IMapper<WithStudentMAPI?, Optional<WithStudentDTO>>,
        IMapper<WithProfessorMAPI?, Optional<WithProfessorDTO>>,
        IMapper<
            PaginatedQuery<ClassDomain, ClassCriteriaDTO>,
            PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI>
        >,
        IMapper<ClassUpdateMAPI, Executor, ClassUpdateDTO>,
        IMapper<string, Executor, DeleteClassDTO>
{
    private readonly StringQueryToDomainMapper _strqToDomainMapper = strqToDomainMapper;
    private readonly StringQueryFromDomainMapper _strqFromDomainMapper = strqFromDomainMapper;

    /// <summary>
    /// Valida y mapea los criterios de búsqueda de clases desde la API a un DTO para la capa de aplicación.
    /// </summary>
    public Result<ClassCriteriaDTO, IEnumerable<FieldErrorDTO>> Map(ClassCriteriaMAPI source)
    {
        List<FieldErrorDTO> errs = [];
        var subjectValidation = _strqToDomainMapper.Map(source.Subject);
        subjectValidation.IfErr(_ => errs.Add(new() { Field = "subject" }));

        var classNameValidation = _strqToDomainMapper.Map(source.ClassName);
        classNameValidation.IfErr(_ => errs.Add(new() { Field = "className" }));

        var sectionValidation = _strqToDomainMapper.Map(source.Section);
        sectionValidation.IfErr(_ => errs.Add(new() { Field = "section" }));

        if (errs.Count > 0)
            return Result.Err<ClassCriteriaDTO, IEnumerable<FieldErrorDTO>>(errs);

        return Result.Ok<ClassCriteriaDTO, IEnumerable<FieldErrorDTO>>(
            new ClassCriteriaDTO
            {
                Page = source.Page,
                Active = source.Active.ToOptional(),
                Subject = subjectValidation.Unwrap(),
                ClassName = classNameValidation.Unwrap(),
                Section = sectionValidation.Unwrap(),
                WithStudent = Map(source.WithStudent),
                WithProfessor = Map(source.WithProfessor),
            }
        );
    }

    /// <summary>
    /// Mapea los criterios de búsqueda desde la capa de aplicación de vuelta a su representación en la API.
    /// </summary>
    public ClassCriteriaMAPI Map(ClassCriteriaDTO input) =>
        new()
        {
            Page = input.Page,
            Active = input.Active.ToNullable(),
            Subject = _strqFromDomainMapper.Map(input.Subject),
            Section = _strqFromDomainMapper.Map(input.Section),
            ClassName = _strqFromDomainMapper.Map(input.ClassName),
            WithProfessor = Map(input.WithProfessor),
            WithStudent = Map(input.WithStudent),
        };

    /// <summary>
    /// Mapea un <see cref="WithProfessorDTO"/> a su representación para la API, <see cref="WithProfessorMAPI"/>.
    /// </summary>
    public WithProfessorMAPI Map(WithProfessorDTO input) =>
        new() { Id = input.Id, IsOwner = input.IsOwner.ToNullable() };

    /// <summary>
    /// Mapea un <see cref="Optional{T}"/> de <see cref="WithProfessorDTO"/> a un <see cref="WithProfessorMAPI"/> nullable.
    /// </summary>
    public WithProfessorMAPI? Map(Optional<WithProfessorDTO> input) =>
        input.Match<WithProfessorMAPI?>(Map, () => null);

    /// <summary>
    /// Mapea un <see cref="WithProfessorMAPI"/> nullable desde la API a un <see cref="Optional{T}"/> de <see cref="WithProfessorDTO"/>.
    /// </summary>
    public Optional<WithProfessorDTO> Map(WithProfessorMAPI? input) =>
        input is not null
            ? new WithProfessorDTO { Id = input.Id, IsOwner = input.IsOwner.ToOptional() }
            : Optional<WithProfessorDTO>.None();

    /// <summary>
    /// Mapea un <see cref="WithStudentDTO"/> a su representación para la API, <see cref="WithStudentMAPI"/>.
    /// </summary>
    public WithStudentMAPI Map(WithStudentDTO input) =>
        new() { Id = input.Id, Hidden = input.Hidden.ToNullable() };

    /// <summary>
    /// Mapea un <see cref="Optional{T}"/> de <see cref="WithStudentDTO"/> a un <see cref="WithStudentMAPI"/> nullable.
    /// </summary>
    public WithStudentMAPI? Map(Optional<WithStudentDTO> input) =>
        input.Match<WithStudentMAPI?>(Map, () => null);

    /// <summary>
    /// Mapea un <see cref="WithStudentMAPI"/> nullable desde la API a un <see cref="Optional{T}"/> de <see cref="WithStudentDTO"/>.
    /// </summary>
    public Optional<WithStudentDTO> Map(WithStudentMAPI? input) =>
        input is not null
            ? new WithStudentDTO { Id = input.Id, Hidden = input.Hidden.ToOptional() }
            : Optional<WithStudentDTO>.None();

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

    /// <summary>
    /// Mapea una entidad de dominio a la representación de la API.
    /// </summary>
    public PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI> Map(
        PaginatedQuery<ClassDomain, ClassCriteriaDTO> search
    ) =>
        new()
        {
            Page = search.Page,
            Criteria = Map(search.Criteria),
            TotalPages = search.TotalPages,
            Results = search.Results.Select(Map),
        };

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

    /// <summary>
    /// Mapea la representación de la API a una entidad de dominio.
    /// </summary>
    public DeleteClassDTO Map(string id, Executor ex) => new() { Id = id, Executor = ex };
}
