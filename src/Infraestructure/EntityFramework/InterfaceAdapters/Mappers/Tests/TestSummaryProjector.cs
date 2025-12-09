using System.Linq.Expressions;
using Application.DTOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

/// <summary>
/// Proyector de consultas para resúmenes de exámenes.
/// </summary>
public class TestSummaryProjector : IEFProjector<Test, TestSummary, TestCriteriaDTO>
{
    /// <inheritdoc/>
    public Expression<Func<Test, TestSummary>> GetProjection(TestCriteriaDTO criteria) =>
        t =>
            new()
            {
                Id = t.TestId,
                Title = t.Title,
                Color = t.Color,
                Active = t.Active,
                ModifiedAt = t.ModifiedAt,
            };
}
