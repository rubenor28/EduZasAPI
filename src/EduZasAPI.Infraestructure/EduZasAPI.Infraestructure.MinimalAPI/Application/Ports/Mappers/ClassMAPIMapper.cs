using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Classes;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Classes;

public static class ClassMAPIMapper
{
    public static Result<ClassCriteriaDTO, List<FieldErrorDTO>> ToDomain(this ClassCriteriaMAPI source)
    {
        var className = Optional<StringQueryDTO>.None();
        var subject = Optional<StringQueryDTO>.None();
        var section = Optional<StringQueryDTO>.None();

        var errs = new List<FieldErrorDTO>();
        StringQueryMAPIMapper.ParseStringQuery(source.ClassName, "className", className, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.Subject, "subject", subject, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.Section, "section", section, errs);

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

    public static ClassUpdateDTO ToDomain(this ClassUpdateMAPI source) => new ClassUpdateDTO
    {
        Id = source.Id,
        Active = source.Active,
        ClassName = source.ClassName,
        Section = source.Section.ToOptional(),
        Subject = source.Subject.ToOptional()
    };

    public static NewClassDTO ToDomain(this NewClassMAPI source) => new NewClassDTO
    {
        Id = source.Id,
        ClassName = source.ClassName,
        OwnerId = source.OwnerId,
        Subject = source.ClassName.ToOptional(),
        Section = source.Section.ToOptional(),
    };

    public static ProfessorClassRelationDTO ToDomain(this ProfessorClassRelationMAPI source) => new ProfessorClassRelationDTO
    {
        Id = new ClassUserRelationIdDTO
        {
            UserId = source.ProfessorId,
            ClassId = source.ClassId
        },
        IsOwner = source.IsOwner,
    };

    public static StudentClassRelationDTO ToDomain(this StudentClassRelationMAPI source) => new StudentClassRelationDTO
    {
        Id = new ClassUserRelationIdDTO { UserId = source.StudentId, ClassId = source.ClassId }
    };

    public static ProfessorClassRelationCriteriaDTO ToDomain(this ProfessorClassRelationCriteriaMAPI source) => new ProfessorClassRelationCriteriaDTO
    {
        ClassId = source.ClassId.ToOptional(),
        UserId = source.ProfessorId.ToOptional(),
        IsOwner = source.IsOwner.ToOptional(),
    };

    public static StudentClassRelationCriteriaDTO ToDomain(this StudentClassRelationCriteriaMAPI source) => new StudentClassRelationCriteriaDTO
    {
        ClassId = source.ClassId.ToOptional(),
        UserId = source.StudentId.ToOptional(),
    };

    public static PublicClassMAPI FromDomain(this PublicClassDTO source) => new PublicClassMAPI
    {
        Id = source.Id,
        Active = source.Active,
        ClassName = source.ClassName,
        Subject = source.Subject.ToNullable(),
        Section = source.Section.ToNullable()
    };
}
