using System.Linq.Expressions;
using Application.DTOs.Classes;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Classes;

public class ProfessorClassesSummaryProjector
    : IEFProjector<Class, ProfessorClassesSummaryDTO, ProfessorClassesSummaryCriteriaDTO>
{
    public Expression<Func<Class, ProfessorClassesSummaryDTO>> GetProjection(
        ProfessorClassesSummaryCriteriaDTO criteria
    ) =>
        ef =>
            new(
                ClassId: ef.ClassId,
                Active: ef.Active ?? false,
                ClassName: ef.ClassName,
                Subject: ef.Subject,
                Section: ef.Section,
                Color: ef.Color ?? "#007bff",
                Owner: ef.ClassProfessors.Where(cp =>
                        cp.ProfessorId == criteria.ProfessorId && cp.ClassId == ef.ClassId
                    )
                    .Select(cp => cp.IsOwner)
                    .FirstOrDefault()
                ?? false
            );
}
