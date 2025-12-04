using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

public class ClassTestMapper : IMapper<TestPerClass, ClassTestDomain>
{
    public ClassTestDomain Map(TestPerClass tpc) =>
        new()
        {
            ClassId = tpc.ClassId,
            TestId = tpc.TestId,
            Visible = tpc.Visible,
            CreatedAt = tpc.CreatedAt,
        };
}
