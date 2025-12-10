using Application.DTOs.Classes;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Classes;

/// <summary>
/// Implementación de consulta de clases usando EF.
/// </summary>
public class StudentClassesSummaryEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Class, StudentClassesSummaryDTO, StudentClassesSummaryCriteriaDTO> projector,
    int maxPageSize
)
    : EFQuerier<StudentClassesSummaryDTO, StudentClassesSummaryCriteriaDTO, Class>(
        ctx,
        projector,
        maxPageSize
    )
{
    /// <inheritdoc/>
    public override IQueryable<Class> BuildQuery(StudentClassesSummaryCriteriaDTO cr) =>
        _dbSet
            .AsNoTracking()
            .WhereStringQuery(cr.Subject, c => c.Subject)
            .WhereStringQuery(cr.Section, c => c.Section)
            .WhereStringQuery(cr.ClassName, c => c.ClassName)
            .WhereOptional(cr.Active, activity => c => c.Active == activity)
            .WhereOptional(cr.Hidden, hidden => c => c.ClassStudents.Any(cs => cs.Hidden == hidden))
            .Where(c => c.ClassStudents.Any(cp => cp.StudentId == cr.StudentId))
            .OrderByDescending(c =>
                c.ClassStudents.Where(cp => cp.StudentId == cr.StudentId)
                    .Select(cp => cp.CreatedAt)
                    .FirstOrDefault()
            );
}
