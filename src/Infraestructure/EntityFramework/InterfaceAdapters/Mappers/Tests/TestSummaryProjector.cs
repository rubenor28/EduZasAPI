using System.Linq.Expressions;
using Application.DTOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

public class TestSummaryProjector : IEFProjector<Test, TestSummary, TestCriteriaDTO>
{
    public Expression<Func<Test, TestSummary>> GetProjection(TestCriteriaDTO criteria) =>
        t =>
            new()
            {
                Id = t.TestId,
                Title = t.Title,
                Active = t.Active,
                ModifiedAt = t.ModifiedAt,
            };
}
