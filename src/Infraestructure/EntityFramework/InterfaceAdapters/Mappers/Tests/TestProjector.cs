using System.Linq.Expressions;
using Application.DTOs.Tests;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

/// <summary>
/// Proyector de consultas para exámenes.
/// </summary>
public class TestProjector : IEFProjector<Test, TestDomain, TestCriteriaDTO>
{
    /// <inheritdoc/>
    public Expression<Func<Test, TestDomain>> GetProjection(TestCriteriaDTO criteria) =>
        t =>
            new()
            {
                Id = t.TestId,
                Active = t.Active,
                Title = t.Title,
                Content = t.Content,
                TimeLimitMinutes = t.TimeLimitMinutes,
                ProfessorId = t.ProfessorId,
                CreatedAt = t.CreatedAt,
                ModifiedAt = t.ModifiedAt,
            };
}
