using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

public class ClassTestMapper : IMapper<TestPerClass, ClassTestDomain>
{
    public ClassTestDomain Map(TestPerClass source) =>
        new()
        {
            TestId = source.TestId,
            ClassId = source.ClassId,
            Visible = source.Visible,
            CreatedAt = source.CreatedAt,
        };
}

