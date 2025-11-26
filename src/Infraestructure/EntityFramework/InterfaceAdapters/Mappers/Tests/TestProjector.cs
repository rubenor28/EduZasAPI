using System.Linq.Expressions;
using Domain.Entities;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

public class TestProjector : IEFProjector<Test, TestDomain>
{
    public Expression<Func<Test, TestDomain>> Projection =>
        t =>
            new()
            {
                Id = t.TestId,
                Title = t.Title,
                Content = t.Content,
                TimeLimitMinutes = t.TimeLimitMinutes.ToOptional(),
                ProfessorId = t.ProfessorId,
                CreatedAt = t.CreatedAt,
                ModifiedAt = t.ModifiedAt,
            };

    private static readonly Lazy<Func<Test, TestDomain>> _mapFunc = new(() =>
        new TestProjector().Projection.Compile()
    );

    public TestDomain Map(Test t) => _mapFunc.Value(t);
}
