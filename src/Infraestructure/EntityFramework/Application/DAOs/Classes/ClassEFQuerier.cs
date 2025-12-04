using Application.DTOs.Classes;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Classes;

public class ClassEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Class, ClassDomain, ClassCriteriaDTO> projector,
    int maxPageSize
) : EFQuerier<ClassDomain, ClassCriteriaDTO, Class>(ctx, projector, maxPageSize)
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
                            && (professor.IsOwner == null || pl.IsOwner == professor.IsOwner)
                        )
            )
            .WhereOptional(
                cr.WithStudent,
                student =>
                    c =>
                        c.ClassStudents.Any(sl =>
                            sl.StudentId == student.Id
                            && (student.Hidden == null || sl.Hidden == student.Hidden)
                        )
            )
            .OrderByDescending(c => c.CreatedAt);
}
