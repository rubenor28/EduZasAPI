using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

public class ClassTestProjector : IEFProjector<TestPerClass, ClassTestDomain>
{
    public Expression<Func<TestPerClass, ClassTestDomain>> Projection =>
        tpc =>
            new()
            {
                Id = new() { ClassId = tpc.ClassId, TestId = tpc.TestId },
                Visible = tpc.Visible,
                CreatedAt = tpc.CreatedAt,
            };

    private static readonly Lazy<Func<TestPerClass, ClassTestDomain>> _mapFunc = new(() =>
        new ClassTestProjector().Projection.Compile()
    );

    public ClassTestDomain Map(TestPerClass tpc) => _mapFunc.Value(tpc);
}
