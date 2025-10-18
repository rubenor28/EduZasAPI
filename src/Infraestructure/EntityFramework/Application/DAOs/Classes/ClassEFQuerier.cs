using Application.DTOs.Classes;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Classes;

public class ClassEFQuerier(
    EduZasDotnetContext ctx,
    IMapper<Class, ClassDomain> domainMapper,
    int pageSize
) : EFQuerier<ClassDomain, ClassCriteriaDTO, Class>(ctx, domainMapper, pageSize)
{
    public override IQueryable<Class> BuildQuery(ClassCriteriaDTO cr) =>
        _dbSet
            .AsNoTracking()
            .WhereStringQuery(cr.Subject, c => c.Subject)
            .WhereStringQuery(cr.Section, c => c.Section)
            .WhereStringQuery(cr.ClassName, c => c.ClassName)
            .WhereOptional(cr.Active, activity => c => c.Active == activity)
            .WhereOptional(
                cr.WithProfessor,
                professor =>
                    c =>
                        c.ClassProfessors.Any(pl =>
                            pl.ProfessorId == professor.Id
                            && (
                                professor.IsOwner.IsNone || pl.IsOwner == professor.IsOwner.Unwrap()
                            )
                        )
            )
            .WhereOptional(
                cr.WithStudent,
                student =>
                    c =>
                        c.ClassStudents.Any(sl =>
                            sl.StudentId == student.Id
                            && (student.Hidden.IsNone || sl.Hidden == student.Hidden.Unwrap())
                        )
            );
}
