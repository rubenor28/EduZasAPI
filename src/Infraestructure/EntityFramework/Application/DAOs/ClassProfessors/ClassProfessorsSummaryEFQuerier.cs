using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassProfessors;

public class ClassProfessorSummaryEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<User, ClassProfessorSummaryDTO, ClassProfessorSummaryCriteriaDTO> projector,
    int maxPageSize
)
    : EFQuerier<ClassProfessorSummaryDTO, ClassProfessorSummaryCriteriaDTO, User>(
        ctx,
        projector,
        maxPageSize
    )
{
    public override IQueryable<User> BuildQuery(ClassProfessorSummaryCriteriaDTO criteria) =>
        _dbSet
            .AsNoTracking()
            .Where(c => c.ClassProfessors.Any(cp => cp.ClassId == criteria.ClassId));
}
