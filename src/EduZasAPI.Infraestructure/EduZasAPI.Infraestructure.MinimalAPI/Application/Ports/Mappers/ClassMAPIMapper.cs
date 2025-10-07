using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Classes;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Classes;

/// <summary>
/// Proporciona métodos de extensión para mapear entre DTOs de la API mínima y DTOs de dominio para clases.
/// </summary>
public static class ClassMAPIMapper
{
    /// <summary>
    /// Convierte un objeto <see cref="ClassCriteriaMAPI"/> en un <see cref="ClassCriteriaDTO"/> de dominio.
    /// </summary>
    /// <param name="source">Instancia de <see cref="ClassCriteriaMAPI"/> a mapear.</param>
    /// <returns>
    /// Un <see cref="Result{T, E}"/> que contiene el <see cref="ClassCriteriaDTO"/> si la conversión
    /// fue exitosa, o una lista de <see cref="FieldErrorDTO"/> si se encontraron errores de formato.
    /// </returns>
    public static Result<ClassCriteriaDTO, List<FieldErrorDTO>> ToDomain(this ClassCriteriaMAPI source)
    {
        var className = Optional<StringQueryDTO>.None();
        var subject = Optional<StringQueryDTO>.None();
        var section = Optional<StringQueryDTO>.None();

        var errs = new List<FieldErrorDTO>();
        StringQueryMAPIMapper.ParseStringQuery(source.ClassName, "className", ref className, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.Subject, "subject", ref subject, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.Section, "section", ref section, errs);

        if (errs.Count > 0)
            return Result.Err<ClassCriteriaDTO, List<FieldErrorDTO>>(errs);


        return Result.Ok<ClassCriteriaDTO, List<FieldErrorDTO>>(new ClassCriteriaDTO
        {
            Page = source.Page,
            Active = source.Active.ToOptional(),
            WithStudent = source.WithStudent.ToOptional(),
            WithProfessor = source.WithProfessor.ToOptional(),
            Subject = subject,
            ClassName = className,
            Section = section
        });
    }

    /// <summary>
    /// Convierte una instancia de <see cref="ClassCriteriaDTO"/> en un objeto de infraestructura <see cref="ClassCriteriaMAPI"/>.
    /// </summary>
    /// <param name="source">Instancia de <see cref="ClassCriteriaDTO"/> a convertir.</param>
    /// <returns>Un <see cref="ClassCriteriaMAPI"/> con los valores correspondientes mapeados desde <paramref name="source"/>.</returns>
    public static ClassCriteriaMAPI FromDomain(this ClassCriteriaDTO source) => new ClassCriteriaMAPI
    {
        Page = source.Page,
        Active = source.Active.IsSome ? source.Active.Unwrap() : null,
        WithProfessor = source.WithProfessor.ToNullable(),
        WithStudent = source.WithStudent.IsSome ? source.WithStudent.Unwrap() : null,
        ClassName = source.ClassName.FromDomain(),
        Section = source.Section.FromDomain(),
        Subject = source.Subject.FromDomain(),
    };

    /// <summary>
    /// Convierte una instancia de <see cref="ClassUpdateMAPI"/> en un <see cref="ClassUpdateDTO"/> de dominio.
    /// </summary>
    /// <param name="source">Instancia de <see cref="ClassUpdateMAPI"/> a convertir.</param>
    /// <returns>Un <see cref="ClassUpdateDTO"/> con los valores correspondientes mapeados desde <paramref name="source"/>.</returns>
    public static ClassUpdateDTO ToDomain(this ClassUpdateMAPI source, ulong userId) => new ClassUpdateDTO
    {
        Id = source.Id,
        Active = source.Active,
        ClassName = source.ClassName,
        Color = source.Color,
        Section = source.Section.ToOptional(),
        Subject = source.Subject.ToOptional(),
        UserId = userId
    };

    /// <summary>
    /// Convierte una instancia de <see cref="NewClassMAPI"/> en un <see cref="NewClassDTO"/> de dominio.
    /// </summary>
    /// <param name="source">Instancia de <see cref="NewClassMAPI"/> a convertir.</param>
    /// <param name="ownerId">Identificador del propietario de la clase.</param>
    /// <returns>Un <see cref="NewClassDTO"/> con los valores correspondientes mapeados desde <paramref name="source"/>.</returns>
    public static NewClassDTO ToDomain(this NewClassMAPI source, ulong ownerId) => new NewClassDTO
    {
        Id = string.Empty,
        ClassName = source.ClassName,
        Color = source.Color,
        OwnerId = ownerId,
        Subject = source.Subject.ToOptional(),
        Section = source.Section.ToOptional(),
    };

    /// <summary>
    /// Convierte una instancia de <see cref="ProfessorClassRelationMAPI"/> en un <see cref="ProfessorClassRelationDTO"/> de dominio.
    /// </summary>
    /// <param name="source">Instancia de <see cref="ProfessorClassRelationMAPI"/> a convertir.</param>
    /// <returns>Un <see cref="ProfessorClassRelationDTO"/> con los valores correspondientes mapeados desde <paramref name="source"/>.</returns>
    public static ProfessorClassRelationDTO ToDomain(this ProfessorClassRelationMAPI source) => new ProfessorClassRelationDTO
    {
        Id = new ClassUserRelationIdDTO
        {
            UserId = source.ProfessorId,
            ClassId = source.ClassId
        },
        IsOwner = source.IsOwner,
    };

    /// <summary>
    /// Convierte una instancia de <see cref="StudentClassRelationMAPI"/> en un <see cref="StudentClassRelationDTO"/> de dominio.
    /// </summary>
    /// <param name="source">Instancia de <see cref="StudentClassRelationMAPI"/> a convertir.</param>
    /// <returns>Un <see cref="StudentClassRelationDTO"/> con los valores correspondientes mapeados desde <paramref name="source"/>.</returns>
    public static StudentClassRelationDTO ToDomain(this StudentClassRelationMAPI source) => new StudentClassRelationDTO
    {
        Id = new ClassUserRelationIdDTO { UserId = source.StudentId, ClassId = source.ClassId },
        Hidden = source.Hidden
    };

    /// <summary>
    /// Convierte una instancia de <see cref="ProfessorClassRelationCriteriaMAPI"/> en un <see cref="ProfessorClassRelationCriteriaDTO"/> de dominio.
    /// </summary>
    /// <param name="source">Instancia de <see cref="ProfessorClassRelationCriteriaMAPI"/> a convertir.</param>
    /// <returns>Un <see cref="ProfessorClassRelationCriteriaDTO"/> con los valores correspondientes mapeados desde <paramref name="source"/>.</returns>
    public static ProfessorClassRelationCriteriaDTO ToDomain(this ProfessorClassRelationCriteriaMAPI source) => new ProfessorClassRelationCriteriaDTO
    {
        ClassId = source.ClassId.ToOptional(),
        UserId = source.ProfessorId.ToOptional(),
        IsOwner = source.IsOwner.ToOptional(),
    };

    /// <summary>
    /// Convierte una instancia de <see cref="ProfessorClassRelationCriteriaDTO"/> en un objeto de infraestructura <see cref="ProfessorClassRelationCriteriaMAPI"/>.
    /// </summary>
    /// <param name="source">Instancia de <see cref="ProfessorClassRelationCriteriaDTO"/> a convertir.</param>
    /// <returns>Un <see cref="ProfessorClassRelationCriteriaMAPI"/> con los valores correspondientes mapeados desde <paramref name="source"/>.</returns>
    public static ProfessorClassRelationCriteriaMAPI FromDomain(this ProfessorClassRelationCriteriaDTO source) => new ProfessorClassRelationCriteriaMAPI
    {
        ClassId = source.ClassId.ToNullable(),
        ProfessorId = source.UserId.ToNullable(),
        IsOwner = source.IsOwner.ToNullable(),
    };

    /// <summary>
    /// Convierte una instancia de <see cref="StudentClassRelationCriteriaMAPI"/> en un <see cref="StudentClassRelationCriteriaDTO"/> de dominio.
    /// </summary>
    /// <param name="source">Instancia de <see cref="StudentClassRelationCriteriaMAPI"/> a convertir.</param>
    /// <returns>Un <see cref="StudentClassRelationCriteriaDTO"/> con los valores correspondientes mapeados desde <paramref name="source"/>.</returns>
    public static StudentClassRelationCriteriaDTO ToDomain(this StudentClassRelationCriteriaMAPI source) => new StudentClassRelationCriteriaDTO
    {
        ClassId = source.ClassId.ToOptional(),
        UserId = source.StudentId.ToOptional(),
    };

    /// <summary>
    /// Convierte una instancia de <see cref="ClassDomain"/> en un objeto de infraestructura <see cref="PublicClassMAPI"/>.
    /// </summary>
    /// <param name="source">Instancia de <see cref="ClassDomain"/> a convertir.</param>
    /// <returns>Un <see cref="PublicClassMAPI"/> con los valores correspondientes mapeados desde <paramref name="source"/>.</returns>
    public static PublicClassMAPI FromDomain(this ClassDomain source) => new PublicClassMAPI
    {
        Id = source.Id,
        Active = source.Active,
        ClassName = source.ClassName,
        Color = source.Color,
        Subject = source.Subject.ToNullable(),
        Section = source.Section.ToNullable()
    };

    /// <summary>
    /// Convierte una consulta paginada de dominio en una consulta paginada de la API mínima.
    /// </summary>
    /// <param name="source">Consulta paginada de dominio a convertir.</param>
    /// <returns>Una consulta paginada de la API mínima con los valores correspondientes mapeados.</returns>
    public static PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI> FromDomain(this PaginatedQuery<ClassDomain, ClassCriteriaDTO> source) => new PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI>
    {
        Page = source.Page,
        TotalPages = source.TotalPages,
        Criteria = source.Criteria.FromDomain(),
        Results = source.Results.Select(ClassMAPIMapper.FromDomain).ToList(),
    };
}
