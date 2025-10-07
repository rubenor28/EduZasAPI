using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Classes;
using Microsoft.EntityFrameworkCore;
using EduZasAPI.Infraestructure.EntityFramework.Application.Common;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Classes;

public class ClassEntityFrameworkRepository :
  SimpleKeyEFRepository<string, ClassDomain, NewClassDTO, ClassUpdateDTO,DeleteClassDTO, ClassCriteriaDTO,  Class>
{
    public ClassEntityFrameworkRepository(EduZasDotnetContext ctx, ulong pageSize) : base(ctx, pageSize) { }

    /// <inheritdoc/>
    protected override string GetId(Class c) => c.ClassId;

    /// <inheritdoc/>
    protected override string GetId(ClassUpdateDTO c) => c.Id;

    /// <inheritdoc/>
    protected override Class NewToEF(NewClassDTO nc) => new Class
    {
        ClassId = nc.Id,
        ClassName = nc.ClassName,
        Color = nc.Color,
        Section = nc.Section.ToNullable(),
        Subject = nc.Subject.ToNullable()
    };

    /// <inheritdoc/>
    protected override void UpdateProperties(Class c, ClassUpdateDTO cu)
    {
        c.ClassId = cu.Id;
        c.ClassName = cu.ClassName;
        c.Active = cu.Active;
        c.Color = cu.Color;
        c.Subject = cu.Subject.ToNullable();
        c.Section = cu.Section.ToNullable();
    }

    /// <inheritdoc/>
    protected override ClassDomain MapToDomain(Class ef) => new ClassDomain
    {
        Id = ef.ClassId,
        Active = ef.Active ?? false,
        ClassName = ef.ClassName,
        Color = ef.Color ?? "#007bff",
        Subject = ef.Subject.ToOptional(),
        Section = ef.Section.ToOptional(),
        CreatedAt = ef.CreatedAt,
        ModifiedAt = ef.ModifiedAt
    };

    /// <inheritdoc/>
    protected override IQueryable<Class> QueryFromCriteria(ClassCriteriaDTO cr) =>
        _ctx.Classes
        .AsNoTracking().AsQueryable()
        .WhereStringQuery(cr.Subject, c => c.Subject)
        .WhereStringQuery(cr.Section, c => c.Section)
        .WhereStringQuery(cr.ClassName, c => c.ClassName)
        .WhereOptional(cr.Active, activity => c => c.Active == activity)
        .WhereOptional(cr.WithStudent, stId => c => c.ClassStudents.Any(cs => cs.StudentId == stId))
        .WhereOptional(cr.WithProfessor, pfId => c => c.ClassProfessors.Any(cpf => cpf.ProfessorId == pfId));
}
