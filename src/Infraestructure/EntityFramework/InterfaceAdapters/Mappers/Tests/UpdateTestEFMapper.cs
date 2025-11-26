using Application.DTOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

public class UpdateTestEFMapper : IUpdateMapper<TestUpdateDTO, Test>
{
    public void Map(TestUpdateDTO tu, Test t)
    {
        t.Title = tu.Title;
        t.Content = tu.Content;
        t.TimeLimitMinutes = tu.TimeLimitMinutes.ToNullable();
        t.ProfessorId = tu.ProfessorId;
    }
}
