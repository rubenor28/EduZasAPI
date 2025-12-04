using Application.DTOs.ClassTests;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

public class NewClassTestEFMapper : IMapper<ClassTestDTO, TestPerClass>
{
    public TestPerClass Map(ClassTestDTO nct) =>
        new()
        {
            TestId = nct.TestId,
            ClassId = nct.ClassId,
            Visible = nct.Visible,
        };
}
