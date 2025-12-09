using Application.DTOs.ClassTests;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

public class NewClassTestEFMapper : IMapper<ClassTestDTO, TestPerClass>
{
    public TestPerClass Map(ClassTestDTO source) =>
        new()
        {
            TestId = source.TestId,
            ClassId = source.ClassId,
            Visible = source.Visible
        };
}