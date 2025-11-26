using Application.DTOs.ClassTests;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

public class NewClassTestEFMapper : IMapper<NewClassTestDTO, TestPerClass>
{
    public TestPerClass Map(NewClassTestDTO nct) =>
        new()
        {
            TestId = nct.TestId,
            ClassId = nct.ClassId,
            Visible = nct.Visible,
        };
}
