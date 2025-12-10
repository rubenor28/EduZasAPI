using System.Linq.Expressions;
using Application.DTOs.Classes;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Classes;

public class StudentClassesSummaryProjector
    : IEFProjector<Class, StudentClassesSummaryDTO, StudentClassesSummaryCriteriaDTO>
{
    public Expression<Func<Class, StudentClassesSummaryDTO>> GetProjection(
        StudentClassesSummaryCriteriaDTO criteria
    ) =>
        ef =>
            new(
                ClassId: ef.ClassId,
                Active: ef.Active ?? false,
                ClassName: ef.ClassName,
                Subject: ef.Subject,
                Section: ef.Section,
                Color: ef.Color ?? "#007bff",
                Hidden: ef.ClassStudents.Where(cp =>
                        cp.StudentId == criteria.StudentId && cp.ClassId == ef.ClassId
                    )
                    .Select(cp => cp.Hidden)
                    .FirstOrDefault()
            );
}
