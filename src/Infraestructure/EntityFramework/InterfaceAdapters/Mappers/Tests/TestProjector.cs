using System.Linq.Expressions;
using Application.DTOs.Tests;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

public class TestProjector : IEFProjector<Test, TestDomain, TestCriteriaDTO>
{
    public Expression<Func<Test, TestDomain>> GetProjection(TestCriteriaDTO criteria) =>
        t =>
            new()
            {
                Id = t.TestId,
                Title = t.Title,
                Content = t.Content,
                TimeLimitMinutes = t.TimeLimitMinutes,
                ProfessorId = t.ProfessorId,
                CreatedAt = t.CreatedAt,
                ModifiedAt = t.ModifiedAt,
            };
}
