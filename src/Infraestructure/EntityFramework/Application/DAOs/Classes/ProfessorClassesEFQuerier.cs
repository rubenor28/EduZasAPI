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
public class ProfessorClassesEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Class, ProfessorClassesSummaryDTO, ProfessorClassesSummaryCriteriaDTO> projector,
    int maxPageSize
)
    : EFQuerier<ProfessorClassesSummaryDTO, ProfessorClassesSummaryCriteriaDTO, Class>(
        ctx,
        projector,
        maxPageSize
    )
{
    /// <inheritdoc/>
    public override IQueryable<Class> BuildQuery(ProfessorClassesSummaryCriteriaDTO cr) =>
        _dbSet
            .AsNoTracking()
            .WhereStringQuery(cr.Subject, c => c.Subject)
            .WhereStringQuery(cr.Section, c => c.Section)
            .WhereStringQuery(cr.ClassName, c => c.ClassName)
            .WhereOptional(cr.Active, activity => c => c.Active == activity)
            .Where(c => c.ClassProfessors.Any(cp => cp.ProfessorId == cr.ProfessorId))
            .OrderByDescending(c =>
                c.ClassProfessors.Where(cp => cp.ProfessorId == cr.ProfessorId)
                    .Select(cp => cp.CreatedAt)
                    .FirstOrDefault()
            );
}
